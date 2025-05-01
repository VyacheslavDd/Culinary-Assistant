using Culinary_Assistant.Core.DTO.Favourite;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Services.Favourites;
using Culinary_Assistant_Main.Tests.Common;
using Culinary_Assistant_Main.Tests.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	[TestFixture]
	public class FavouriteReceiptsService_Tests
	{
		private CulinaryAppContext _context;
		private IFavouriteReceiptsService _favouriteReceiptsService;

		[SetUp]
		public async Task Setup()
		{
			_context = DbContextMocker.CreateInMemoryAppContext();
			await _context.Users.AddRangeAsync(UsersData.Users);
			await _context.Receipts.AddRangeAsync(ReceiptsData.Receipts);
			await _context.SaveChangesAsync();
			var usersRepository = new UsersRepository(_context);
			var receiptsRepository = new ReceiptsRepository(_context);
			var favouriteReceiptsRepository = new FavouriteReceiptsRepository(_context);
			_favouriteReceiptsService = new FavouriteReceiptsService(usersRepository, receiptsRepository, favouriteReceiptsRepository);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Dispose();
		}

		[Test]
		public async Task CanAddReceipt_ToFavourites()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			var oneMoreReceiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Салат");
			var res = await _favouriteReceiptsService.AddAsync(new FavouriteInDTO(userId, receiptId));
			var secondRes = await _favouriteReceiptsService.AddAsync(new FavouriteInDTO(userId, oneMoreReceiptId));
			var favourites = await _favouriteReceiptsService.GetAllReceiptsFavouritedForUserAsync(userId);
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(secondRes.IsSuccess, Is.True);
				Assert.That(favourites.Value, Has.Count.EqualTo(2));
			});
		}

		[Test]
		public async Task DifferentUsers_CanAddReceipts_ToFavourites()
		{
			var firstUserId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var secondUserId = await CommonUtils.GetUserGuidByLoginAsync(_context, "myworstlogin");
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			var resFirst = await _favouriteReceiptsService.AddAsync(new FavouriteInDTO(firstUserId, receiptId));
			var resSecond = await _favouriteReceiptsService.AddAsync(new FavouriteInDTO(secondUserId, receiptId));
			var firstUserFavourites = await _favouriteReceiptsService.GetAllReceiptsFavouritedForUserAsync(firstUserId);
			var secondUserFavourites = await _favouriteReceiptsService.GetAllReceiptsFavouritedForUserAsync(secondUserId);
			Assert.Multiple(() =>
			{
				Assert.That(resFirst.IsSuccess, Is.True);
				Assert.That(resSecond.IsSuccess, Is.True);
				Assert.That(firstUserFavourites.Value, Has.Count.EqualTo(1));
				Assert.That(secondUserFavourites.Value, Has.Count.EqualTo(1));
			});
		}

		[Test]
		public async Task CanGet_FavouritedInfo()
		{
			var guids = await AddFavouriteReceiptAndGetUserAndReceiptDataAsync();
			var favouriteInfo = await _favouriteReceiptsService.GetAsync(guids.Item1, guids.Item2);
			Assert.That(favouriteInfo, Is.Not.Null);
		}

		[Test]
		public async Task DeletingReceipts_DeletesFavouritesInfo()
		{
			await AddFavouriteReceiptAndGetUserAndReceiptDataAsync();
			var receipts = await _context.Receipts.ToListAsync();
			_context.Receipts.RemoveRange(receipts);
			await _context.SaveChangesAsync();
			var favouritesInfo = await _favouriteReceiptsService.GetAllAsync();
			Assert.That(favouritesInfo, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task CanDelete_FavouritesInfo()
		{
			var guids = await AddFavouriteReceiptAndGetUserAndReceiptDataAsync();
			var res = await _favouriteReceiptsService.RemoveAsync(guids.Item1, guids.Item2);
			var favourites = await _favouriteReceiptsService.GetAllAsync();
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(favourites, Has.Count.EqualTo(0));
			});
		}

		[Test]
		public async Task CannotAddToFavourites_WithWrongData()
		{
			var userId = Guid.Empty;
			var receiptId = Guid.Empty;
			var res = await _favouriteReceiptsService.AddAsync(new FavouriteInDTO(userId, receiptId));
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotGetFavourites_ForNotExistingUser()
		{
			var userId = Guid.Empty;
			var favouritesResult = await _favouriteReceiptsService.GetAllReceiptsFavouritedForUserAsync(userId);
			Assert.That(favouritesResult.IsFailure, Is.True);
		}

		private async Task<Tuple<Guid, Guid>> AddFavouriteReceiptAndGetUserAndReceiptDataAsync()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			await _favouriteReceiptsService.AddAsync(new FavouriteInDTO(userId, receiptId));
			return Tuple.Create(userId, receiptId);
		}
	}
}
