using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Services.ReceiptCollections;
using Culinary_Assistant_Main.Services.ReceiptsCollections;
using Culinary_Assistant_Main.Services.Seed;
using Culinary_Assistant_Main.Services.Users;
using Culinary_Assistant_Main.Tests.Common;
using Culinary_Assistant_Main.Tests.Data;
using Culinary_Assistant.Core.Filters;
using Moq;
using Microsoft.EntityFrameworkCore;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant_Main.Services.Likes;
using Culinary_Assistant.Core.DTO.Like;

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	[TestFixture]
	public class ReceiptCollectionsService_Tests
	{
		private CulinaryAppContext _context;
		private IReceiptCollectionsService _receiptCollectionsService;
		private ILikesService<ReceiptCollectionLike, ReceiptCollection> _collectionsLikesService;
		private Guid _adminId;
		private List<Guid> _receiptIds;

		[SetUp]
		public async Task SetUp()
		{
			_context = DbContextMocker.CreateInMemoryAppContext();
			var logger = CommonUtils.MockLogger();
			var usersRepository = new UsersRepository(_context);
			var receiptsService = CommonUtils.MockReceiptsService(_context, usersRepository, logger);
			var receiptCollectionsRepository = new ReceiptCollectionsRepository(_context);
			var receiptCollectionLikesRepository = new ReceiptCollectionLikesRepository(_context);
			var elasticService = new Mock<IElasticReceiptsCollectionsService>();
			elasticService.Setup(es => es.GetReceiptsCollectionsIdsAsync(It.IsAny<string>()))
				.Returns(Task.FromResult(Result.Success<List<Guid>>([Guid.Empty])));
			var usersService = new UsersService(usersRepository, logger);
			var redisService = CommonUtils.MockRedisService();
			_collectionsLikesService = new ReceiptCollectionLikesService(receiptCollectionLikesRepository, usersRepository, receiptCollectionsRepository);
			_receiptCollectionsService = new ReceiptCollectionsService(receiptCollectionsRepository, elasticService.Object, redisService, _collectionsLikesService, receiptsService, usersService, logger);
			var seedService = new SeedService(usersRepository, logger);
			_adminId = await seedService.CreateAdministratorUserAsync();
			await CreateReceiptCollectionsAsync();
		}

		[TearDown]
		public void TearDown()
		{
			_context.Dispose();
		}

		[Test]
		public async Task GetAllAsync_WithoutFilter_ShowsAllCollections()
		{
			var receiptCollections = await _receiptCollectionsService.GetAllAsync();
			Assert.That(receiptCollections, Has.Count.EqualTo(3));
		}

		[Test]
		public async Task GetAllAsync_WithFilter_DoesNotShowPrivateCollections()
		{
			var collectionsResponse = await _receiptCollectionsService.GetAllByFilterAsync(new ReceiptCollectionsFilter());
			Assert.That(collectionsResponse.Value.Data, Has.Count.EqualTo(2));
		}

		[Test]
		public async Task GetAllAsync_WithFilter_DoesCorrectPagination()
		{
			var collectionsResponse = (await _receiptCollectionsService.GetAllByFilterAsync(new ReceiptCollectionsFilter(Limit: 1, Page: 2))).Value;
			Assert.Multiple(() =>
			{
				Assert.That(collectionsResponse.PagesCount, Is.EqualTo(2));
				Assert.That(collectionsResponse.EntitiesCount, Is.EqualTo(2));
				Assert.That(collectionsResponse.Data, Has.Count.EqualTo(1));
				Assert.That(collectionsResponse.Data[0].Title.Value, Is.EqualTo("Second"));
			});
		}

		[Test]
		public async Task GetAllAsync_WithFilter_ReturnsEmptyList_WhenExceededPagination()
		{
			var collectionsResponse = (await _receiptCollectionsService.GetAllByFilterAsync(new ReceiptCollectionsFilter(Limit: 2, Page: 2))).Value;
			Assert.That(collectionsResponse.Data, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task GetAllAsync_WithUserIdFilter_ReturnsCollectionsOfUser()
		{
			var collections = await _receiptCollectionsService.GetAllAsync();
			var userId = Guid.NewGuid();
			collections[0].SetUserId(userId);
			await _context.SaveChangesAsync();
			var collectionsByUserId = await _receiptCollectionsService.GetAllByFilterAsync(new ReceiptCollectionsFilter(UserId: userId));
			Assert.That(collectionsByUserId.Value.EntitiesCount, Is.EqualTo(1));
		}

		[Test]
		public async Task GetByGuidAsync_Works_And_LoadsReceiptsAndUser()
		{
			var collectionId = await GetReceiptCollectionGuid(2);
			var collection = await _receiptCollectionsService.GetByGuidAsync(collectionId);
			Assert.Multiple(() =>
			{
				Assert.That(collection, Is.Not.Null);
				Assert.That(collection.User, Is.Not.Null);
				Assert.That(collection.User.Id, Is.EqualTo(_adminId));
				Assert.That(collection.Receipts, Has.Count.EqualTo(3));
			});
		}

		[Test]
		public async Task CreateAsync_CreatesEmptyCollection()
		{
			var receiptCollectionDto = new ReceiptCollectionInModelDTO("Fourth", false, Color.Red, _adminId, null);
			var res = await _receiptCollectionsService.CreateWithNameCheckAsync(receiptCollectionDto);
			var collections = await _receiptCollectionsService.GetAllAsync();
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(collections, Has.Count.EqualTo(4));
			});
		}

		[Test]
		public async Task CreateAsync_CreatesCollectionWithReceipts_AndMakeRelations()
		{
			var receiptCollectionDto = new ReceiptCollectionInModelDTO("Fourth", false, Color.Red, _adminId, [_receiptIds[1]]);
			var res = await _receiptCollectionsService.CreateWithNameCheckAsync(receiptCollectionDto);
			var receiptEntry = await _context.Receipts.FirstOrDefaultAsync(r => r.Id == _receiptIds[1]);
			await _context.Receipts.Entry(receiptEntry).Collection(r => r.ReceiptCollections).LoadAsync();
			var fourthReceiptCollection = receiptEntry.ReceiptCollections.FirstOrDefault(rc => rc.Title.Value == "Fourth");
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(fourthReceiptCollection, Is.Not.Null);
			});
		}

		[Test]
		public async Task CreateAsync_Failures_WithNotExistingUser()
		{
			var receiptCollectionDTO = new ReceiptCollectionInModelDTO("Fourth", false, Color.Red, Guid.Empty, null);
			var res = await _receiptCollectionsService.CreateWithNameCheckAsync(receiptCollectionDTO);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CreateAsync_Failures_WithWrongDTOData()
		{
			var receiptCollectionDTO = new ReceiptCollectionInModelDTO("", false, Color.Red, _adminId, null);
			var res = await _receiptCollectionsService.CreateWithNameCheckAsync(receiptCollectionDTO);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task UpdateAsync_UpdatesReceiptCollection()
		{
			var collectionId = await GetReceiptCollectionGuid(2);
			var updateCollectionDTO = new ReceiptCollectionUpdateDTO("MEOW", null, Color.Orange);
			var res = await _receiptCollectionsService.NotBulkUpdateAsync(collectionId, updateCollectionDTO);
			var collection = await _receiptCollectionsService.GetByGuidAsync(collectionId);
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(collection.Title.Value, Is.EqualTo("MEOW"));
				Assert.That(collection.IsPrivate, Is.False);
				Assert.That(collection.Color, Is.EqualTo(Color.Orange));
			});
		}

		[Test]
		public async Task MakingCollectionPrivate_RemovesItsLikes()
		{
			var collectionId = await GetReceiptCollectionGuid(2);
			await _collectionsLikesService.AddAsync(new LikeInDTO(_adminId, collectionId));
			var likeBefore = await _collectionsLikesService.GetAsync(_adminId, collectionId);
			var res = await _receiptCollectionsService.NotBulkUpdateAsync(collectionId, new ReceiptCollectionUpdateDTO(null, true, null));
			var likeAfter = await _collectionsLikesService.GetAsync(_adminId, collectionId);
			Assert.Multiple(() =>
			{
				Assert.That(likeBefore, Is.Not.Null);
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(likeAfter, Is.Null);
			});
		}

		[Test]
		public async Task UpdateAsync_Failures_WithNotExistingCollection()
		{
			var collectionId = Guid.Empty;
			var updateDTO = new ReceiptCollectionUpdateDTO("XD", true, null);
			var res = await _receiptCollectionsService.NotBulkUpdateAsync(collectionId, updateDTO);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task UpdateAsync_Failures_WithWrongDTO()
		{
			var collectionId = await GetReceiptCollectionGuid(2);
			var updateDTO = new ReceiptCollectionUpdateDTO("    ", true, null);
			var res = await _receiptCollectionsService.NotBulkUpdateAsync(collectionId, updateDTO);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task DeleteAsync_DeletesCollection()
		{
			var collectionId = await GetReceiptCollectionGuid(2);
			var res = await _receiptCollectionsService.NotBulkDeleteAsync(collectionId);
			var receipts = await _context.Receipts.ToListAsync();
			foreach (var receipt in receipts)
				await _context.Receipts.Entry(receipt).Collection(r => r.ReceiptCollections).LoadAsync();
			var allReceiptsDoesNotHaveThirdCollection = receipts.All(r => r.ReceiptCollections.FirstOrDefault(rc => rc.Id == collectionId) == null);
			var thirdCollection = await _receiptCollectionsService.GetByGuidAsync(collectionId);
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(thirdCollection, Is.Null);
				Assert.That(allReceiptsDoesNotHaveThirdCollection, Is.True);
			});
		}

		[Test]
		public async Task CollectionsDataUpdates_OnReceiptRemoved()
		{
			var receiptEntry = _context.Receipts.First(r => r.Id == _receiptIds[0]);
			_context.Receipts.Remove(receiptEntry);
			await _context.SaveChangesAsync();
			var receiptCollections = await _receiptCollectionsService.GetAllAsync();
			foreach (var collection in receiptCollections)
				await _context.ReceiptCollections.Entry(collection).Collection(rc => rc.Receipts).LoadAsync();
			var allCollectionsDoesNotHaveReceipt = receiptCollections.All(rc => rc.Receipts.FirstOrDefault(r => r.Id == _receiptIds[0]) == null);
			Assert.That(allCollectionsDoesNotHaveReceipt, Is.True);
		}

		[Test]
		public async Task AddReceipts_DoesAddThemToCollection_IgnoringDoubles()
		{
			var collectionId = await GetReceiptCollectionGuid(1);
			var res = await _receiptCollectionsService.AddReceiptsAsyncUsingReceiptCollectionId(collectionId, [_receiptIds[0], _receiptIds[2]]);
			var collection = await _receiptCollectionsService.GetByGuidAsync(collectionId);
			var firstReceiptsCount = collection.Receipts.Where(r => r.Id == _receiptIds[0]).Count();
			var thirdReceipt = collection.Receipts.FirstOrDefault(r => r.Id == _receiptIds[2]);
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(collection.Receipts, Has.Count.EqualTo(2));
				Assert.That(firstReceiptsCount, Is.EqualTo(1));
				Assert.That(thirdReceipt, Is.Not.Null);
			});
		}

		[Test]
		public async Task AddReceipts_Failures_OnAddingReceipts_ToNotExistingCollection()
		{
			var collectionId = Guid.Empty;
			var res = await _receiptCollectionsService.AddReceiptsAsyncUsingReceiptCollectionId(collectionId, [_receiptIds[0], _receiptIds[2]]);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task RemoveReceipts_DoesRemoveThemFromCollection_IgnoringNotPresented()
		{
			var collectionId = await GetReceiptCollectionGuid(2);
			var res = await _receiptCollectionsService.RemoveReceiptsAsync(collectionId, [_receiptIds[0], _receiptIds[2], Guid.Empty]);
			var secondReceipt = _context.Receipts.First(r => r.Id == _receiptIds[0]);
			await _context.Receipts.Entry(secondReceipt).Collection(r => r.ReceiptCollections).LoadAsync();
			var collectionOnReceipt = secondReceipt.ReceiptCollections.FirstOrDefault(rc => rc.Id == collectionId);
			var collection = await _receiptCollectionsService.GetByGuidAsync(collectionId);
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(collectionOnReceipt, Is.Null);
				Assert.That(collection.Receipts, Has.Count.EqualTo(1));
			});
		}

		[Test]
		public async Task RemoveReceipts_Failures_OnRemovingReceipts_FromNotExistingCollection()
		{
			var collectionId = Guid.Empty;
			var res = await _receiptCollectionsService.RemoveReceiptsAsync(collectionId, [_receiptIds[0], _receiptIds[2]]);
			Assert.That(res.IsFailure, Is.True);
		}

		private async Task CreateReceiptCollectionsAsync()
		{
			var receipts = ReceiptsData.Receipts;
			_receiptIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
			for (int i = 0; i <  _receiptIds.Count; i++)
				receipts[i].SetReceiptId(_receiptIds[i]);
			await _context.Receipts.AddRangeAsync(receipts);
			List<ReceiptCollectionInModelDTO> receiptCollectionsDTO =
				[
					new("First", true, Color.Red, _adminId, null),
					new("Second", false, Color.Red, _adminId, [_receiptIds[0]]),
					new("Third", false, Color.Red, _adminId, [_receiptIds[0], _receiptIds[1], _receiptIds[2]])
				];
			foreach (var receiptCollectionDTO in receiptCollectionsDTO)
				await _receiptCollectionsService.CreateWithNameCheckAsync(receiptCollectionDTO);
		}

		private async Task<Guid> GetReceiptCollectionGuid(int index)
		{
			var collections = await _receiptCollectionsService.GetAllAsync();
			var collectionId = collections[index].Id;
			return collectionId;
		}
	}
}
