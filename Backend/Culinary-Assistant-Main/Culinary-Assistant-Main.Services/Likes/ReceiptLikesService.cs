using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Like;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Likes.Abstract;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Likes
{
	public class ReceiptLikesService(ILikesRepository<ReceiptLike, Receipt> likesRepository, IReceiptsRepository receiptsRepository, IUsersRepository usersRepository)
		: LikesService<ReceiptLike, Receipt>(likesRepository, usersRepository)
	{
		private readonly IReceiptsRepository _receiptsRepository = receiptsRepository;

		private readonly Action<Receipt> _onLike = (Receipt receipt) => receipt.AddPopularity();

		public override async Task<Result<Guid>> AddAsync(LikeInDTO likeInDTO)
		{
			return await AddAsync(likeInDTO, _receiptsRepository, _onLike);
		}
	}
}
