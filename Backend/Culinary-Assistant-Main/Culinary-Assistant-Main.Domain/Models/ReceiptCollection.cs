using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.ValueTypes;
using Culinary_Assistant_Main.Domain.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models
{
	public class ReceiptCollection : Core.Base.Entity<Guid>, IRateable
	{
		private readonly List<Receipt> _receipts = [];
		private readonly List<ReceiptCollectionLike> _likes = [];
		private readonly List<ReceiptCollectionRate> _rates = [];

		public Text Title { get; private set; }
		public bool IsPrivate { get; private set; }
		public Guid UserId { get; private set; }
		public User User { get; private set; }
		public int Popularity { get; private set; }
		public double Rating { get; private set; }
		public Color Color { get; private set; }
		public DateTime CreatedAt { get; private set; }
		public DateTime UpdatedAt { get; private set; }
		public IReadOnlyCollection<Receipt> Receipts => _receipts;
		public IReadOnlyCollection<ReceiptCollectionLike> Likes => _likes;
		public IReadOnlyCollection<ReceiptCollectionRate> Rates => _rates;

		public static Result<ReceiptCollection> Create(ReceiptCollectionInModelDTO receiptCollectionInModelDTO)
		{
			var receiptCollection = new ReceiptCollection();
			var titleRes = receiptCollection.SetTitle(receiptCollectionInModelDTO.Title);
			if (titleRes.IsFailure) return Result.Failure<ReceiptCollection>(titleRes.Error);
			receiptCollection.IsPrivate = receiptCollectionInModelDTO.IsPrivate;
			receiptCollection.SetUserId(receiptCollectionInModelDTO.UserId);
			receiptCollection.SetColor(receiptCollectionInModelDTO.Color);
			receiptCollection.ActualizeUpdatedAtField();
			receiptCollection.CreatedAt = DateTime.UtcNow;
			return Result.Success(receiptCollection);
		}

		public Result SetTitle(string title)
		{
			var titleRes = Text.Create(title, 100);
			if (titleRes.IsFailure) return Result.Failure(titleRes.Error);
			Title = titleRes.Value;
			return Result.Success();
		}

		public void AddReceipts(List<Receipt> receipts)
		{
			_receipts.AddRange(receipts);
		}

		public void RemoveReceipts(List<Guid> receiptIds)
		{
			var hashSetIds = new HashSet<Guid>(receiptIds);
			for (var i = _receipts.Count - 1; i >= 0; i--)
			{
				if (hashSetIds.Contains(_receipts[i].Id))
				{
					_receipts.Remove(_receipts[i]);
				}
			}
		}

		public void SetUserId(Guid userId)
		{
			UserId = userId;
		}

		public void SetPrivateState(bool isPrivate)
		{
			IsPrivate = isPrivate;
		}

		public void AddPopularity()
		{
			Popularity += 1;
		}

		public void SetColor(Color color)
		{
			Color = color;
		}

		public void ActualizeUpdatedAtField()
		{
			UpdatedAt = DateTime.UtcNow;
		}

		public void ClearLikes()
		{
			_likes.Clear();
		}

		public void SetRating(double rating)
		{
			Rating = Math.Round(rating, MiscellaneousConstants.RoundRatingToDigits);
		}
	}
}
