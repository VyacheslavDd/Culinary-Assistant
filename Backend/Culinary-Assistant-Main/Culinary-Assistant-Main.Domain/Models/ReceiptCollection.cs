using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models
{
	public class ReceiptCollection : Core.Base.Entity<Guid>
	{
		private readonly List<Receipt> _receipts = [];

		public Text Title { get; private set; }
		public string ReceiptCovers { get; private set; }
		public bool IsPrivate { get; private set; }
		public Guid UserId { get; private set; }
		public User User { get; private set; }
		public IReadOnlyCollection<Receipt> Receipts => _receipts;

		public static Result<ReceiptCollection> Create(ReceiptCollectionInModelDTO receiptCollectionInModelDTO)
		{
			var receiptCollection = new ReceiptCollection();
			var titleRes = receiptCollection.SetTitle(receiptCollectionInModelDTO.Title);
			if (titleRes.IsFailure) return Result.Failure<ReceiptCollection>(titleRes.Error);
			receiptCollection.IsPrivate = receiptCollectionInModelDTO.IsPrivate;
			receiptCollection.SetUserId(receiptCollectionInModelDTO.UserId);
			receiptCollection.SetCovers(null);
			return Result.Success(receiptCollection);
		}

		public Result SetTitle(string title)
		{
			var titleRes = Text.Create(title, 100);
			if (titleRes.IsFailure) return Result.Failure(titleRes.Error);
			Title = titleRes.Value;
			return Result.Success();
		}

		public void SetCovers(List<FilePath>? filePaths = null)
		{
			if (filePaths == null || filePaths.Count == 0)
			{
				ReceiptCovers = "";
				return;
			}
			var takeFilePaths = filePaths.Take(MiscellaneousConstants.ReceiptCollectionMaxCoversCount).ToList();
			ReceiptCovers = JsonSerializer.Serialize(takeFilePaths);
		}

		public void AddReceipts(List<Receipt> receipts)
		{
			_receipts.AddRange(receipts);
			var existingCovers = ReceiptCovers == "" ? [] : JsonSerializer.Deserialize<List<FilePath>>(ReceiptCovers);
			var addMoreCoversCount = MiscellaneousConstants.ReceiptCollectionMaxCoversCount - existingCovers.Count;
			if (addMoreCoversCount > 0)
			{
				var newCovers = receipts.Take(addMoreCoversCount)
										.Select(r => new FilePath(r.MainPictureUrl))
										.ToList();
				existingCovers.AddRange(newCovers);
				SetCovers(existingCovers);
			}
		}

		public void RemoveReceipts(List<Guid> receiptIds)
		{
			var hashSetIds = new HashSet<Guid>(receiptIds);
			List<string> coversToRemove = [];
			for (var i = _receipts.Count - 1; i >= 0; i--)
			{
				if (hashSetIds.Contains(_receipts[i].Id))
				{
					coversToRemove.Add(_receipts[i].MainPictureUrl);
					_receipts.Remove(_receipts[i]);
				}
			}
			DeleteCoversIfPresented(coversToRemove);
		}

		public void DeleteCoversIfPresented(List<string> filePaths)
		{
			var hashSetPaths = new HashSet<string>(filePaths);
			var covers = ReceiptCovers == "" ? [] : JsonSerializer.Deserialize<List<FilePath>>(ReceiptCovers);
			var updatedCovers = covers.Where(c => !hashSetPaths.Contains(c.Url)).ToList();
			SetCovers(updatedCovers);
		}
		
		public void UpdateCoverIfPresented(string oldFilePath, string newFilePath)
		{
			var covers = ReceiptCovers == "" ? [] : JsonSerializer.Deserialize<List<FilePath>>(ReceiptCovers);
			var updatedCovers = covers.Select(c =>
			{
				if (c.Url != oldFilePath) return c;
				c = new FilePath(newFilePath);
				return c;
			}).ToList();
			SetCovers(updatedCovers);
		}

		public void SetUserId(Guid userId)
		{
			UserId = userId;
		}

		public void SetPrivateState(bool isPrivate)
		{
			IsPrivate = isPrivate;
		}
	}
}
