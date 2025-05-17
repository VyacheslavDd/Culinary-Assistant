using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Services.Feedbacks;
using Culinary_Assistant_Main.Tests.Common;
using Culinary_Assistant_Main.Tests.Data;
using Culinary_Assistant.Core.DTO.Feedback;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using Minio;
using Minio.DataModel.Args;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Culinary_Assistant.Core.Filters;
using Microsoft.EntityFrameworkCore;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Fabrics;
using Culinary_Assistant.Core.Enums;

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	[TestFixture]
	public class FeedbacksService_Tests
	{
		private CulinaryAppContext _context;
		private IFeedbacksService _feedbacksService;

		[SetUp]
		public async Task SetUp()
		{
			_context = DbContextMocker.CreateInMemoryAppContext();
			await PrepareTestDataAsync();
			var minioClientFactory = CommonUtils.MockMinioClientFactory();
			var ratesRepository = new ReceiptRatesRepository(_context);
			var logger = CommonUtils.MockLogger();
			var usersRepository = new UsersRepository(_context);
			var receiptsRepository = new ReceiptsRepository(_context);
			var feedbacksRepository = new FeedbacksRepository(_context);
			_feedbacksService = new FeedbacksService(ratesRepository, usersRepository, receiptsRepository, minioClientFactory, feedbacksRepository, logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Dispose();
		}

		[Test]
		public async Task CanGetAllFeedbacks_FromAllReceipts()
		{
			var feedbacks = await _feedbacksService.GetAllAsync();
			Assert.That(feedbacks, Has.Count.EqualTo(6));
		}

		[Test]
		public async Task CanGetAllFeedbacks_OfReceipt_Correctly()
		{
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			var feedbacks = await _feedbacksService.GetAllAsync(receiptId, new FeedbacksFilter());
			Assert.That(feedbacks.EntitiesCount, Is.EqualTo(2));
		}

		[Test]
		public async Task PaginationWorks_When_GettingFeedbacks_OfReceipt()
		{
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Салат");
			var feedbacks = await _feedbacksService.GetAllAsync(receiptId, new FeedbacksFilter(Page: 4, Limit: 1));
			Assert.Multiple(() =>
			{
				Assert.That(feedbacks.Data, Has.Count.EqualTo(1));
				Assert.That(feedbacks.Data[0].Text.Value, Is.EqualTo("hellothere1"));
			});
		}

		[Test]
		public async Task CanCreateFeedback_WithCorrectData()
		{
			var userTask = CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptTask = CommonUtils.GetReceiptGuidByNameAsync(_context, "Салат");
			var guids = await Task.WhenAll(userTask, receiptTask);
			var res = await _feedbacksService.CreateAsync(new FeedbackInDTO(guids[0], guids[1], "hellothere34"));
			var receiptFeedbacks = await _feedbacksService.GetAllAsync(guids[1], new FeedbacksFilter());
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(receiptFeedbacks.EntitiesCount, Is.EqualTo(5));
			});
		}

		[Test]
		public async Task CanGetFeedbackByGuid()
		{
			var feedback = await _context.Feedbacks.FirstAsync(f => f.Text.Value == "hellothere1");
			var receiptRate = RateFactory.Create<ReceiptRate, Receipt>(new RateModelDTO(feedback.UserId, feedback.ReceiptId, 5));
			await _context.ReceiptRates.AddAsync(receiptRate.Value);
			await _context.SaveChangesAsync();
			var feedbackByService = await _feedbacksService.GetByGuidAsync(feedback.Id);
			Assert.Multiple(() =>
			{
				Assert.That(feedbackByService, Is.Not.Null);
				Assert.That(feedbackByService.Text.Value, Is.EqualTo("hellothere1"));
				Assert.That(feedbackByService.UserLogin, Is.EqualTo("mybestlogin"));
				Assert.That(feedbackByService.Rate, Is.EqualTo(5));
			});
		}

		[Test]
		public async Task CanUpdateFeedback_WithCorrectData()
		{
			var feedback = await _context.Feedbacks.FirstAsync(f => f.Text.Value == "hellothere1");
			await _feedbacksService.NotBulkUpdateAsync(feedback.Id, new FeedbackUpdateDTO("hellothere76"));
			var updatedFeedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Text.Value == "hellothere76");
			Assert.That(updatedFeedback, Is.Not.Null);
		}

		[Test]
		public async Task CanRemoveFeedback()
		{
			var feedback = await _context.Feedbacks.FirstAsync(f => f.Text.Value == "hellothere1");
			await _feedbacksService.NotBulkDeleteAsync(feedback.Id);
			var feedbackNew = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Text.Value == "hellothere1");
			Assert.That(feedbackNew, Is.Null);
		}

		[Test]
		public async Task CannotUpdate_NotExistingFeedback()
		{
			var res = await _feedbacksService.NotBulkUpdateAsync(Guid.Empty, new FeedbackUpdateDTO("hellothere76"));
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotUpdate_WithBadText()
		{
			var feedback = await _context.Feedbacks.FirstAsync(f => f.Text.Value == "hellothere1");
			var res = await _feedbacksService.NotBulkUpdateAsync(feedback.Id, new FeedbackUpdateDTO("h"));
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotCreateFeedback_ForNotExistingReceipt()
		{
			var userId = await CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptId = Guid.Empty;
			var res = await _feedbacksService.CreateAsync(new FeedbackInDTO(userId, receiptId, "hellothere34"));
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotCreateFeedback_ForNotExistingUser()
		{
			var userId = Guid.Empty;
			var receiptId = await CommonUtils.GetReceiptGuidByNameAsync(_context, "Салат");
			var res = await _feedbacksService.CreateAsync(new FeedbackInDTO(userId, receiptId, "hellothere34"));
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotCreateFeedback_WithBadText()
		{
			var userTask = CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var receiptTask = CommonUtils.GetReceiptGuidByNameAsync(_context, "Салат");
			var guids = await Task.WhenAll(userTask, receiptTask);
			var res = await _feedbacksService.CreateAsync(new FeedbackInDTO(guids[0], guids[1], "h"));
			Assert.That(res.IsFailure, Is.True);
		}

		private async Task PrepareTestDataAsync()
		{
			var receipts = ReceiptsData.Receipts;
			var users = UsersData.Users;
			var feedbacks = FeedbacksData.Feedbacks;
			await Task.WhenAll(_context.AddRangeAsync(receipts), _context.AddRangeAsync(users));
			await _context.SaveChangesAsync();
			var userTask = CommonUtils.GetUserGuidByLoginAsync(_context, "mybestlogin");
			var firstReceiptTask = CommonUtils.GetReceiptGuidByNameAsync(_context, "Салат");
			var secondReceiptTask = CommonUtils.GetReceiptGuidByNameAsync(_context, "Суп");
			var guids = await Task.WhenAll(userTask, firstReceiptTask, secondReceiptTask);
			feedbacks.ForEach(f =>
			{
				f.SetUserId(guids[0]);
				var order = int.Parse(f.Text.Value[^1].ToString());
				f.SetReceiptId(order <= 4 ? guids[1] : guids[2]);
			});
			await _context.AddRangeAsync(feedbacks);
			await _context.SaveChangesAsync();
		}
	}
}
