using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Like;
using Culinary_Assistant_Main.Domain.Fabrics;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Services.Likes;
using Culinary_Assistant_Main.Tests.Common;
using Culinary_Assistant_Main.Tests.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	[TestFixture]
	public class LikeServices_Tests
	{
		private CulinaryAppContext _context;
		private ILikesService<ReceiptLike, Receipt> _receiptLikesService;
		private ILikesService<ReceiptCollectionLike, ReceiptCollection> _receiptCollectionLikesService;

		[SetUp]
		public async Task SetupAsync()
		{
			_context = DbContextMocker.CreateInMemoryAppContext();
			await _context.Receipts.AddRangeAsync(ReceiptsData.Receipts);
			await _context.Users.AddRangeAsync(UsersData.Users);
			await _context.SaveChangesAsync();
			var bestUserGuid = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var collectionsData = ReceiptCollectionsData.ReceiptCollections;
			collectionsData.ForEach(cd => cd.SetUserId(bestUserGuid));
			await _context.ReceiptCollections.AddRangeAsync(collectionsData);
			await _context.SaveChangesAsync();
			var receiptsRepository = new ReceiptsRepository(_context);
			var usersRepository = new UsersRepository(_context);
			var receiptCollectionsRepository = new ReceiptCollectionsRepository(_context);
			var receiptLikesRepository = new ReceiptLikesRepository(_context);
			var receiptCollectionsLikeRepository = new ReceiptCollectionLikesRepository(_context);
			_receiptLikesService = new ReceiptLikesService(receiptLikesRepository, receiptsRepository, usersRepository);
			_receiptCollectionLikesService = new ReceiptCollectionLikesService(receiptCollectionsLikeRepository, usersRepository, receiptCollectionsRepository);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Dispose();
		}

		[Test]
		public async Task CanSuccessfully_GetLikeWithServices()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			var collectionId = await CommonUtils.GetReceiptCollectionGuidByNameAsync(_context, "First");
			await _context.ReceiptLikes.AddAsync(LikeFactory.Create<ReceiptLike, Receipt>(new LikeInDTO(userId, receiptId)).Value);
			await _context.ReceiptCollectionLikes.AddAsync(LikeFactory.Create<ReceiptCollectionLike, ReceiptCollection>(new LikeInDTO(userId, collectionId)).Value);
			await _context.SaveChangesAsync();
			var receiptLike = await _receiptLikesService.GetAsync(userId, receiptId);
			var collectionLike = await _receiptCollectionLikesService.GetAsync(userId, collectionId);
			Assert.Multiple(() =>
			{
				Assert.That(receiptLike, Is.Not.Null);
				Assert.That(collectionLike, Is.Not.Null);
			});
		}

		[Test]
		public async Task CanSuccessfully_PutLikeOnReceipt()
		{
			(var res, var likes, var receipt) = await PutLikeAndGetTestingDataAsync(CommonUtils.GetReceiptGuidByNameAsync, _receiptLikesService, "Суп", _context.ReceiptLikes, _context.Receipts,
				r => r.Title.Value == "Суп");
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(likes, Is.EqualTo(1));
			});
		}

		[Test]
		public async Task CanSuccessfully_PutLikeOnReceiptCollection()
		{
			(var res, var likes, var receipt) = await PutLikeAndGetTestingDataAsync(CommonUtils.GetReceiptCollectionGuidByNameAsync, _receiptCollectionLikesService, "First",
				_context.ReceiptCollectionLikes, _context.ReceiptCollections, r => r.Title.Value == "First");
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(likes, Is.EqualTo(1));
			});
		}

		[Test]
		public async Task CanSuccessfully_PutFewLikes()
		{
			var firstUserId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var secondUserId = await CommonUtils.GetUserGuidByLoginAsync(_context, "myworstlogin");
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			await _receiptLikesService.AddAsync(new LikeInDTO(firstUserId, receiptId));
			await _receiptLikesService.AddAsync(new LikeInDTO(secondUserId, receiptId));
			var entity = await _context.Receipts.FirstAsync(r => r.Title.Value == "Суп");
			_context.Entry(entity).Collection(rl => rl.Likes);
			Assert.That(entity.Likes.Count, Is.EqualTo(2));
		}

		[Test]
		public async Task CanSuccessfully_PutLikesOnDifferentEntities()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var firstReceiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			var secondReceiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Салат");
			var collectionId = await CommonUtils.GetReceiptCollectionGuidByNameAsync(_context, "First");
			var resFirst = await _receiptLikesService.AddAsync(new LikeInDTO(userId, firstReceiptId));
			var resSecond = await _receiptLikesService.AddAsync(new LikeInDTO(userId, secondReceiptId));
			var resThird = await _receiptCollectionLikesService.AddAsync(new LikeInDTO(userId, collectionId));
			Assert.Multiple(() =>
			{
				Assert.That(resFirst.IsSuccess, Is.True);
				Assert.That(resSecond.IsSuccess, Is.True);
				Assert.That(resThird.IsSuccess, Is.True);
			});
		}

		[Test]
		public async Task CanSuccessfully_RemoveLikes()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var firstReceiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			await _receiptLikesService.AddAsync(new LikeInDTO(userId, firstReceiptId));
			await _receiptLikesService.RemoveAsync(userId, firstReceiptId);
			var like = await _receiptLikesService.GetAsync(userId, firstReceiptId);
			Assert.That(like, Is.Null);
		}

		[Test]
		public async Task CanSuccessfully_RemoveEntities_ConnectedWithLikes()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			await _receiptLikesService.AddAsync(new LikeInDTO(userId, receiptId));
			var user = await _context.Users.FirstAsync(u => u.Login.Value == "mybestlogin");
			var receipt = await _context.Receipts.FirstAsync(r => r.Title.Value == "Суп");
			_context.Users.Remove(user);
			_context.Receipts.Remove(receipt);
			await _context.SaveChangesAsync();
			Assert.Pass();
		}

		[Test]
		public async Task CannotPutLike_WhenUser_DoesNotExist()
		{
			var userId = Guid.Empty;
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			var res = await _receiptLikesService.AddAsync(new LikeInDTO(userId, receiptId));
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotPutLike_OnPrivateCollection()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var collection = await _context.ReceiptCollections.FirstAsync(rc => rc.Title.Value == "First");
			var collectionId = collection.Id;
			collection.SetPrivateState(true);
			await _context.SaveChangesAsync();
			var res = await _receiptCollectionLikesService.AddAsync(new LikeInDTO(userId, collectionId));
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotPutLike_WhenReceipt_DoesNotExist()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptId = Guid.Empty;
			var res = await _receiptLikesService.AddAsync(new LikeInDTO(userId, receiptId));
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotPutLike_Twice()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			var collectionId = await CommonUtils.GetReceiptCollectionGuidByNameAsync(_context, "First");
			var receiptFirstLikeRes = await _receiptLikesService.AddAsync(new LikeInDTO(userId, receiptId));
			var receiptSecondLikeRes = await _receiptLikesService.AddAsync(new LikeInDTO(userId, receiptId));
			var collectionFirstLikeRes = await _receiptCollectionLikesService.AddAsync(new LikeInDTO(userId, collectionId));
			var collectionSecondLikeRes = await _receiptCollectionLikesService.AddAsync(new LikeInDTO(userId, collectionId));
			Assert.Multiple(() =>
			{
				Assert.That(receiptFirstLikeRes.IsSuccess, Is.True);
				Assert.That(collectionFirstLikeRes.IsSuccess, Is.True);
				Assert.That(receiptSecondLikeRes.IsFailure, Is.True);
				Assert.That(collectionSecondLikeRes.IsFailure, Is.True);
			});
		}

		private async Task<Tuple<Result<Guid>, int, TLiked>> PutLikeAndGetTestingDataAsync<T, TLiked>(Func<CulinaryAppContext, string, Task<Guid>> getEntityId, ILikesService<T, TLiked> likesService,
			string entityName, DbSet<T> dbSetLikes, DbSet<TLiked> dbSetEntities, Expression<Func<TLiked, bool>> searchExpression)
			where T: Like<TLiked> where TLiked: Core.Base.Entity<Guid>
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptId = await getEntityId(_context, entityName);
			var res = await likesService.AddAsync(new LikeInDTO(userId, receiptId));
			var likes = dbSetLikes.Count();
			var entity = await dbSetEntities.FirstAsync(searchExpression);
			return Tuple.Create(res, likes, entity);
		}
	}
}
