using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Favourite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models
{
	public class ReceiptFavourite : Core.Base.Entity<Guid>
	{
		public Guid UserId { get; private set; }
		public Guid ReceiptId { get; private set; }
		public User User { get; private set; }
		public Receipt Receipt { get; private set; }

		public static Result<ReceiptFavourite> Create(FavouriteInDTO favouriteInDTO)
		{
			var favouriteInfo = new ReceiptFavourite();
			favouriteInfo.SetUserId(favouriteInDTO.UserId);
			favouriteInfo.SetReceiptId(favouriteInDTO.ReceiptId);
			return Result.Success(favouriteInfo);
		}

		public void SetUserId(Guid userId)
		{
			UserId = userId;
		}

		public void SetReceiptId(Guid receiptId)
		{
			ReceiptId = receiptId;
		}
	}
}
