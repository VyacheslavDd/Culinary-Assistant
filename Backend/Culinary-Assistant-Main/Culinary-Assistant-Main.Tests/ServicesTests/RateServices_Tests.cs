using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Infrastructure.Repositories.Abstract;
using Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates;
using Culinary_Assistant_Main.Services.ReceiptRates;
using Culinary_Assistant_Main.Tests.Common;
using Culinary_Assistant_Main.Tests.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	public class RateServices_Tests
	{
		private CulinaryAppContext _context;
		private Guid _userId;
		private IRateService<ReceiptRate, Receipt> _receiptRatesService;
		private IRateService<ReceiptCollectionRate, ReceiptCollection> _collectionRatesService;

		[SetUp]
		public async Task Setup()
		{
			_context = DbContextMocker.CreateInMemoryAppContext();
			_context.Users.AddRange(UsersData.Users);
			_context.Receipts.AddRange(ReceiptsData.Receipts);
			_context.ReceiptCollections.AddRange(ReceiptCollectionsData.ReceiptCollections);
			await _context.SaveChangesAsync();
			_userId = _context.Users.First().Id;
			var receiptRatesProducerServiceMock = new Mock<IRatingMessageProducerService<Receipt>>();
			var collectionRatesProducerServiceMock = new Mock<IRatingMessageProducerService<ReceiptCollection>>();
			var receiptRatesRepository = new ReceiptRatesRepository(_context);
			var collectionRatesRepository = new ReceiptCollectionRatesRepository(_context);
			var receiptsRepository = new ReceiptsRepository(_context);
			var collectionsRepository = new ReceiptCollectionsRepository(_context);
			var usersRepository = new UsersRepository(_context);
			_receiptRatesService = new ReceiptRateService(receiptRatesProducerServiceMock.Object, receiptRatesRepository, receiptsRepository, usersRepository);
			_collectionRatesService = new CollectionRateService(collectionRatesProducerServiceMock.Object, collectionRatesRepository, collectionsRepository, usersRepository);
		}

		[TearDown]
		public void Teardown()
		{
			_context.Dispose();
		}

		[Test]
		public async Task CanGetRate()
		{
			(var receiptId, var collectionId) = await GetEntitiesIdsAsync();
			await AddRateAsync(_receiptRatesService, receiptId, 8);
			await AddRateAsync(_collectionRatesService, collectionId, 8);
			var receiptGetRes = await GetRateAsync(_receiptRatesService, receiptId);
			var collectionGetRes = await GetRateAsync(_collectionRatesService, collectionId);
			Assert.Multiple(() =>
			{
				Assert.That(receiptGetRes.IsSuccess, Is.True);
				Assert.That(collectionGetRes.IsSuccess, Is.True);
			});
		}

		[Test]
		public async Task CanAddRate()
		{
			(var receiptId, var collectionId) = await GetEntitiesIdsAsync();
			var receiptRes = await AddRateWithCheckAsync(_receiptRatesService, receiptId, 8);
			var collectionRes = await AddRateWithCheckAsync(_collectionRatesService, collectionId, 6);
			Assert.Multiple(() =>
			{
				Assert.That(receiptRes.IsSuccess, Is.True);
				Assert.That(collectionRes.IsSuccess, Is.True);
			});
		}

		[Test]
		public async Task CanGetAllRates()
		{
			(var receiptId, var collectionId) = await GetEntitiesIdsAsync();
			await AddRateAsync(_receiptRatesService, receiptId, 8);
			await AddRateAsync(_collectionRatesService, collectionId, 8);
			var receiptsRates = await _receiptRatesService.GetAllRatesForEntityAsync(receiptId);
			var collectionRates = await _collectionRatesService.GetAllRatesForEntityAsync(collectionId);
			Assert.Multiple(() =>
			{
				Assert.That(receiptsRates, Has.Count.EqualTo(1));
				Assert.That(collectionRates, Has.Count.EqualTo(1));
			});
		}

		[Test]
		public async Task CanDoRepeatRating()
		{
			(var receiptId, var collectionId) = await GetEntitiesIdsAsync();
			await AddRateAsync(_receiptRatesService, receiptId, 8);
			await AddRateAsync(_collectionRatesService, collectionId, 8);
			var repeatReceiptRateRes = await AddRateWithCheckAsync(_receiptRatesService, receiptId, 4);
			var repeatCollectionRateRes = await AddRateWithCheckAsync(_collectionRatesService, collectionId, 5);
			Assert.Multiple(() =>
			{
				Assert.That(repeatReceiptRateRes.IsSuccess, Is.True);
				Assert.That(repeatCollectionRateRes.IsSuccess, Is.True);
				Assert.That(repeatCollectionRateRes.Value.Rating, Is.EqualTo(5));
				Assert.That(repeatReceiptRateRes.Value.Rating, Is.EqualTo(4));
			});
		}

		[Test]
		public async Task CannotAddBadRating()
		{
			(var receiptId, var collectionId) = await GetEntitiesIdsAsync();
			var receiptRes = await AddRateWithCheckAsync(_receiptRatesService, receiptId, 11);
			var collectionRes = await AddRateWithCheckAsync(_collectionRatesService, collectionId, 0);
			Assert.Multiple(() =>
			{
				Assert.That(receiptRes.IsFailure, Is.True);
				Assert.That(collectionRes.IsFailure, Is.True);
			});
		}

		[Test]
		public async Task DeletingUser_DoesNotBreak_Ratings()
		{
			(var receiptId, var collectionId) = await GetEntitiesIdsAsync();
			await AddRateAsync(_receiptRatesService, receiptId, 8);
			await AddRateAsync(_collectionRatesService, collectionId, 8);
			var user = await _context.Users.FirstAsync(u => u.Id == _userId);
			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			var receiptRates = await _receiptRatesService.GetAllRatesForEntityAsync(receiptId);
			var collectionRates = await _collectionRatesService.GetAllRatesForEntityAsync(collectionId);
			Assert.Multiple(() =>
			{
				Assert.That(receiptRates, Has.Count.EqualTo(1));
				Assert.That(collectionRates, Has.Count.EqualTo(1));
			});
		}

		private async Task<Result<T>> AddRateWithCheckAsync<T, TRated>(IRateService<T, TRated> rateService, Guid entityId, int rate) where T: Rate<T, TRated>, new() where TRated: Core.Base.Entity<Guid>
		{
			var res = await AddRateAsync(rateService, entityId, rate);
			if (res.IsFailure) return Result.Failure<T>(res.Error);
			var rateRes = await GetRateAsync(rateService, entityId);
			return rateRes;
		}

		private async Task<Result> AddRateAsync<T, TRated>(IRateService<T, TRated> rateService, Guid entityId, int rate) where T : Rate<T, TRated>, new() where TRated : Core.Base.Entity<Guid>
		{
			var rateModelDTO = new RateModelDTO(_userId, entityId, rate);
			var res = await rateService.AddOrUpdateAsync(rateModelDTO);
			if (!res.IsSuccess) return Result.Failure("Не удалось добавить оценку");
			return Result.Success();
		}

		private async Task<Tuple<Guid, Guid>> GetEntitiesIdsAsync()
		{
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			var collectionId = await CommonUtils.GetReceiptCollectionGuidByNameAsync(_context, "First");
			return Tuple.Create(receiptId, collectionId);
		}

		private async Task<Result<T>> GetRateAsync<T, TRated>(IRateService<T, TRated> rateService, Guid entityId) where T : Rate<T, TRated>, new() where TRated : Core.Base.Entity<Guid>
		{
			var rateElem = await rateService.GetAsync(_userId, entityId);
			if (rateElem == null) return Result.Failure<T>("Оценка не найдена");
			return Result.Success(rateElem);
		}
	}
}
