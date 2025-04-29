using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Like;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Likes.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Likes
{
	public class ReceiptCollectionLikesService(ILikesRepository<ReceiptCollectionLike, ReceiptCollection> likesRepository, IUsersRepository usersRepository,
		IReceiptCollectionsRepository receiptCollectionsRepository) : LikesService<ReceiptCollectionLike, ReceiptCollection>(likesRepository, usersRepository)
	{
		private readonly IReceiptCollectionsRepository _receiptCollectionsRepository = receiptCollectionsRepository;

		private readonly Action<ReceiptCollection> _onLike = (ReceiptCollection receiptCollection) => receiptCollection.AddPopularity();

		public override async Task<Result<Guid>> AddAsync(LikeInDTO likeInDTO)
		{
			return await AddAsync(likeInDTO, _receiptCollectionsRepository, _onLike);
		}
	}
}
