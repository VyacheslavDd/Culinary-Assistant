
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

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	[TestFixture]
	public class ReceiptsService_Tests
	{
		private CulinaryAppContext _culinaryAppContext;
		private Guid _userId;
		private IReceiptsService _receiptsService;

		private readonly Func<Guid, ReceiptInDTO> _getFineReceiptInDTO = (Guid userId) => new("Пища", "Описание", [Tag.Vegetarian], Category.Any, CookingDifficulty.Easy,
					50, [new Ingredient("Морковь", 3, Measure.Piece), new Ingredient("Свекла", 2, Measure.Piece)],
					[new CookingStep(1, "Один"), new CookingStep(2, "Два")],
					[new FilePath("https://placehold.co/600x400")], userId);

		private readonly Func<Guid, ReceiptInDTO> _getWrongReceiptInDTO = (Guid userId) => new("", "Описание", [Tag.Vegetarian], Category.Any, CookingDifficulty.Easy,
					50, [new Ingredient("Морковь", -30, Measure.Piece), new Ingredient("Свекла", 2, Measure.Piece)],
					[new CookingStep(4, "Один"), new CookingStep(2, "Два")],
					[new FilePath("https://placehold.co/600x400")], userId);

		[SetUp]
		public async Task SetUp()
		{
			_culinaryAppContext = DbContextMocker.CreateInMemoryAppContext();
			var usersRepository = new UsersRepository(_culinaryAppContext);
			var receiptsRepository = new ReceiptsRepository(_culinaryAppContext);
			var logger = CommonUtils.MockLogger();
			var seedService = new SeedService(usersRepository, logger);
			_userId = await seedService.CreateAdministratorUserAsync();
			var usersService = new UsersService(usersRepository, logger);
			var rabbitMqOptionsMock = new Mock<IOptions<RabbitMQOptions>>();
			rabbitMqOptionsMock.Setup(o => o.Value).Returns(new RabbitMQOptions() { HostName = "" });
			var producerService = new Mock<IFileMessagesProducerService>();
			producerService.Setup(ps => ps.SendRemoveImagesMessageAsync(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
			var minioClientFactoryMock = new Mock<IMinioClientFactory>();
			_receiptsService = new ReceiptsService(usersService, producerService.Object, minioClientFactoryMock.Object, receiptsRepository, logger);
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
				Assert.That(receipts.EntitiesCount, Is.EqualTo(9));
				Assert.That(receipts.PagesCount, Is.EqualTo(5));
			});
		}

		[Test]
		public async Task GetAllAsync_With_Page_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(Limit: 1, Page: 3));
			Assert.That(receipts.Data[0].Title.Value, Is.EqualTo("Название"));
		}

		[Test]
		public async Task GetAllAsync_ByTitle_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(SearchByTitle: "а"));
			Assert.That(receipts.Data.Select(r => r.Title.Value), Is.EquivalentTo(new List<string> { "Название", "Салат" }));
		}

		[Test]
		public async Task GetAllAsync_ByDifficulty_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(CookingDifficulty: CookingDifficulty.Hard));
			Assert.That(receipts.Data.All(r => r.CookingDifficulty == CookingDifficulty.Hard), Is.True);
		}

		[Test]
		public async Task GetAllAsync_ByCategory_WorksCorrectly()
		{
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(Category: Category.Soups));
			Assert.That(receipts.Data.All(r => r.Category == Category.Soups), Is.True);
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
			var receipts = await GetReceiptsWithFilterAsync(new ReceiptsFilter(Limit: 2, SearchByTitle: "а", Category: Category.Dinner));
			Assert.Multiple(() =>
			{
				Assert.That(receipts.EntitiesCount, Is.EqualTo(1));
				Assert.That(receipts.Data[0].Title.Value, Is.EqualTo("Салат"));
			});
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
			return receipts;
		}

		private async Task<Receipt> GetFirstReceiptAsync()
		{
			await AddReceiptsToDbContextAsync();
			return (await _receiptsService.GetAllAsync()).First();
		}
	}
}
