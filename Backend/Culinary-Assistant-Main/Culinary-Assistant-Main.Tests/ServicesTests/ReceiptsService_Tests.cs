
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.Seed;
using Culinary_Assistant_Main.Services.Users;
using Culinary_Assistant_Main.Tests.Common;
using Culinary_Assistant_Main.Tests.Data;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Shared.Serializable;
using Microsoft.EntityFrameworkCore;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Moq;
using Microsoft.Extensions.Options;
using Culinary_Assistant.Core.Options;
using Minio;
using Elastic.Clients.Elasticsearch;

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	[TestFixture]
	public class ReceiptsService_Tests
	{
		private CulinaryAppContext _culinaryAppContext;
		private Guid _userId;
		private IReceiptsService _receiptsService;

		private readonly Func<Guid, ReceiptInDTO> _getFineReceiptInDTO = (Guid userId) => new("Пища", "Описание", [Tag.Vegetarian], Category.Dinner, CookingDifficulty.Easy,
					50, [new Ingredient("Морковь", 3, Measure.Piece), new Ingredient("Свекла", 2, Measure.Piece)],
					[new CookingStep(1, "w", "Один"), new CookingStep(2, "w", "Два")],
					[new FilePath("https://placehold.co/600x400")], userId);

		private readonly Func<Guid, ReceiptInDTO> _getWrongReceiptInDTO = (Guid userId) => new("", "Описание", [Tag.Vegetarian], Category.Breakfast, CookingDifficulty.Easy,
					50, [new Ingredient("Морковь", -30, Measure.Piece), new Ingredient("Свекла", 2, Measure.Piece)],
					[new CookingStep(4, "w", "Один"), new CookingStep(2, "w", "Два")],
					[new FilePath("https://placehold.co/600x400")], userId);

		[SetUp]
		public async Task SetUp()
		{
			_culinaryAppContext = DbContextMocker.CreateInMemoryAppContext();
			var usersRepository = new UsersRepository(_culinaryAppContext);
			var logger = CommonUtils.MockLogger();
			var seedService = new SeedService(usersRepository, logger);
			_userId = await seedService.CreateAdministratorUserAsync();
			_receiptsService = CommonUtils.MockReceiptsService(_culinaryAppContext, usersRepository, logger);
		}

		[TearDown]
		public void TearDown()
		{
			_culinaryAppContext.Dispose();
		}

		[Test]
		public async Task GetAllAsync_With_DefaultFilter_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter());
			Assert.That(receipts.EntitiesCount, Is.EqualTo(3));
		}

		[Test]
		public async Task GetAllAsync_With_DefaultFilter_By_CollectionRequirement_WorksCorrectly()
		{
			await AddReceiptsToDbContextAsync();
			var receipts = await _receiptsService.GetAllAsync();
			List<Guid> receiptIds = [receipts[0].Id, receipts[2].Id];
			var filteredReceipts = await _receiptsService.GetAllAsync(new ReceiptsFilter(), collectionReceiptIds: receiptIds);
			Assert.Multiple(() =>
			{
				Assert.That(filteredReceipts.IsSuccess, Is.True);
				Assert.That(filteredReceipts.Value.Data.Select(r => r.Id), Is.EquivalentTo(receiptIds));
			});
		}

		[Test]
		public async Task GetAllAsync_With_Limit_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(Limit: 2));
			Assert.That(receipts.Data, Has.Count.EqualTo(2));
		}

		[Test]
		public async Task GetAllAsync_GetsCorrectPagesCount()
		{
			for (var i = 0; i < 3; i++)
				await AddReceiptsToDbContextAsync();
			var filter = new ReceiptsFilter(Limit: 2);
			var receipts = await _receiptsService.GetAllAsync(filter);
			Assert.Multiple(() =>
			{
				Assert.That(receipts.Value.EntitiesCount, Is.EqualTo(9));
				Assert.That(receipts.Value.PagesCount, Is.EqualTo(5));
			});
		}

		[Test]
		public async Task GetAllAsync_With_Page_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(Limit: 1, Page: 3));
			Assert.That(receipts.Data[0].Title.Value, Is.EqualTo("Название"));
		}

		[Test]
		public async Task GetAllAsync_ByDifficulty_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(CookingDifficulties: [CookingDifficulty.Hard]));
			Assert.That(receipts.Data.All(r => r.CookingDifficulty == CookingDifficulty.Hard), Is.True);
		}

		[Test]
		public async Task GetAllAsync_ByCookingTime_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(CookingTimeTo: 60));
			Assert.That(receipts.Data.All(r => r.CookingTime <= 60), Is.True);
		}

		[Test]
		public async Task GetAllAsync_ByCategory_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(Categories: [Category.Soup]));
			Assert.That(receipts.Data.All(r => r.Category == Category.Soup), Is.True);
		}

		[Test]
		public async Task GetAllAsync_ByTags_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(Tags: [Tag.Vegetarian]));
			Assert.Multiple(() =>
			{
				Assert.That(receipts.EntitiesCount, Is.EqualTo(1));
				Assert.That(receipts.Data[0].Title.Value, Is.EqualTo("Название"));
			});
		}

		[Test]
		public async Task GetAllAsync_WithComplexFilter_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(Limit: 2, SearchByTitle: "а", Categories: [Category.Dinner]));
			Assert.Multiple(() =>
			{
				Assert.That(receipts.EntitiesCount, Is.EqualTo(1));
				Assert.That(receipts.Data[0].Title.Value, Is.EqualTo("Салат"));
			});
		}

		[Test]
		public async Task GetAllAsync_WithSorting_WorksCorrectly()
		{
			await _culinaryAppContext.SaveChangesAsync();
			var sortedReceipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(SortOption: ReceiptSortOption.ByCookingTime, IsAscendingSorting: false));
			var receiptsData = sortedReceipts.Data;
			Assert.Multiple(() =>
			{
				Assert.That(receiptsData[0].Title.Value, Is.EqualTo("Суп"));
				Assert.That(receiptsData[1].Title.Value, Is.EqualTo("Салат"));
				Assert.That(receiptsData[2].Title.Value, Is.EqualTo("Название"));
			});
		}

		[Test]
		public async Task GetAllAsync_WithUserId_WorksCorrectly()
		{
			await AddReceiptsToDbContextAsync();
			var receipts = await _receiptsService.GetAllAsync();
			var userId = Guid.NewGuid();
			receipts[0].SetUserId(userId);
			await _culinaryAppContext.SaveChangesAsync();
			var receiptsByUserIdFilter = await _receiptsService.GetAllAsync(new ReceiptsFilter(UserId: userId));
			Assert.That(receiptsByUserIdFilter.Value.EntitiesCount, Is.EqualTo(1));
		}

		[Test]
		public async Task CreateAsync_WorksCorrectly_WithGoodData()
		{
			var res = await _receiptsService.CreateAsync(_getFineReceiptInDTO(_userId));
			var receipt = await _receiptsService.GetByGuidAsync(res.Value);
			Assert.Multiple(() =>
			{
				Assert.That(receipt, Is.Not.Null);
				Assert.That(receipt.Nutrients.Calories, Is.Positive);
				Assert.That(receipt.Nutrients.Proteins, Is.Positive);
				Assert.That(receipt.Nutrients.Fats, Is.Positive);
				Assert.That(receipt.Nutrients.Carbohydrates, Is.Positive);
			});
		}

		[Test]
		public async Task CreateAsync_Fails_WhenNotExistingUser()
		{
			var res = await _receiptsService.CreateAsync(_getFineReceiptInDTO(new Guid()));
			Assert.That(res.IsFailure, Is.True);
		}


		[Test]
		public async Task CreateAsync_Fails_WhenBadRequest()
		{
			var res = await _receiptsService.CreateAsync(_getWrongReceiptInDTO(_userId));
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task UpdateAsync_WorksCorrectly()
		{
			var receipt = await GetFirstReceiptAsync();
			var updateDTO = new UpdateReceiptDTO(Description: "IDK");
			await _receiptsService.NotBulkUpdateAsync(receipt.Id, updateDTO);
			var updatedReceipt = await _receiptsService.GetByGuidAsync(receipt.Id);
			Assert.That(updatedReceipt.Description.Value, Is.EqualTo("IDK"));
		}

		[Test]
		public async Task UpdateAsync_Fails_WhenNotExistingReceipt()
		{
			var result = await _receiptsService.NotBulkUpdateAsync(_userId, new UpdateReceiptDTO());
			Assert.That(result.IsFailure, Is.True);
		}

		[Test]
		public async Task UpdateAsync_Fails_WhenBadData()
		{
			var receipt = await GetFirstReceiptAsync();
			var updateDTO = new UpdateReceiptDTO(Title: "");
			var result = await _receiptsService.NotBulkUpdateAsync(receipt.Id, updateDTO);
			Assert.That(result.IsFailure, Is.True);
		}

		[Test]
		public async Task GetByGuid_WorksCorrectlyAndLoadsUser()
		{
			var receiptGuid = await _receiptsService.CreateAsync(_getFineReceiptInDTO(_userId));
			var receipt = await _receiptsService.GetByGuidAsync(receiptGuid.Value);
			Assert.Multiple(() =>
			{
				Assert.That(receipt.Title.Value, Is.EqualTo("Пища"));
				Assert.That(receipt.User.Id, Is.EqualTo(_userId));
			});
		}

		[Test]
		public async Task GetByGuid_ReturnsNull_WithNotExistingReceipt()
		{
			var receipt = await _receiptsService.GetByGuidAsync(new Guid("01960773-8d6d-7f2c-b9db-f1bc5b4859a4"));
			Assert.That(receipt, Is.Null);
		}

		[Test]
		public async Task DeleteAsync_WorksCorrectly_When_HasEntitiesToRemove()
		{
			await AddReceiptsToDbContextAsync();
			var receipt = (await _receiptsService.GetAllAsync())[0];
			var res = await _receiptsService.NotBulkDeleteAsync(receipt.Id);
			var receipts = await _receiptsService.GetAllAsync();
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(res.Value, Does.Contain("1"));
				Assert.That(receipts, Has.Count.EqualTo(2));
			});
		}

		[Test]
		public async Task DeleteAsync_WorksCorrectly_When_HasNoEntitiesToRemove()
		{
			await AddReceiptsToDbContextAsync();
			var res = await _receiptsService.NotBulkDeleteAsync(Guid.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(res.Value, Does.Contain("0"));
			});
		}

		private async Task AddReceiptsToDbContextAsync()
		{
			var receipts = ReceiptsData.Receipts;
			receipts.ForEach(r => r.SetUserId(_userId));
			await _culinaryAppContext.Receipts.AddRangeAsync(receipts);
			await _culinaryAppContext.SaveChangesAsync();
		}

		private async Task<EntitiesResponseWithCountAndPages<Receipt>> GetReceiptsWithFilterAsync(ReceiptsFilter filter)
		{
			await AddReceiptsToDbContextAsync();
			var receipts = await _receiptsService.GetAllAsync(filter);
			return receipts.Value;
		}

		private async Task<Receipt> GetFirstReceiptAsync()
		{
			await AddReceiptsToDbContextAsync();
			return (await _receiptsService.GetAllAsync()).First();
		}
	}
}
