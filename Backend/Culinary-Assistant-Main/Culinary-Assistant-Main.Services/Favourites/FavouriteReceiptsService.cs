using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.DTO.Favourite;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Favourites
{
	public class FavouriteReceiptsService(IUsersRepository usersRepository, IReceiptsRepository receiptsRepository, IFavouriteReceiptsRepository favouriteReceiptsRepository) : IFavouriteReceiptsService
	{
		private readonly IFavouriteReceiptsRepository _favouriteReceiptsRepository = favouriteReceiptsRepository;
		private readonly IUsersRepository _usersRepository = usersRepository;
		private readonly IReceiptsRepository _receiptsRepository = receiptsRepository;

		public async Task<Result<Guid>> AddAsync(FavouriteInDTO favouriteInDTO)
		{
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == favouriteInDTO.UserId);
			if (user == null) return Result.Failure<Guid>("Несуществующий пользователь не может добавить рецепт в избранное");
			var receipt = await _receiptsRepository.GetBySelectorAsync(r => r.Id == favouriteInDTO.ReceiptId);
			if (receipt == null) return Result.Failure<Guid>("Нельзя добавить в избранное несуществующий рецепт");
			var alreadyFavourited = (await GetAsync(favouriteInDTO.UserId, favouriteInDTO.ReceiptId)) != null;
			if (alreadyFavourited) return Result.Failure<Guid>("Рецепт уже добавлен в избранное");
			var model = ReceiptFavourite.Create(favouriteInDTO).Value;
			var res = await _favouriteReceiptsRepository.AddAsync(model);
			return Result.Success(res);
		}

		public async Task ApplyFavouritesInfoToReceiptsDataAsync<T>(ClaimsPrincipal user, List<T> receiptsData) where T : IFavouritedDTO
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(user);
			var foundFavouritesData = await _favouriteReceiptsRepository.GetAllBySelector(rf => rf.UserId == userId)
				.Select(rf => rf.ReceiptId).ToHashSetAsync();
			if (foundFavouritesData.Count == 0) return;
			foreach (var receipt in receiptsData)
				if (foundFavouritesData.Contains(receipt.Id)) receipt.IsFavourited = true;
		}

		public async Task<List<ReceiptFavourite>> GetAllAsync()
		{
			return await _favouriteReceiptsRepository.GetAll().ToListAsync();
		}

		public async Task<Result<List<Receipt>>> GetAllReceiptsFavouritedForUserAsync(Guid userId, CancellationToken cancellationToken = default)
		{
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == userId, cancellationToken);
			if (user == null) return Result.Failure<List<Receipt>>("Попытка получить избранные рецепты для несуществующего пользователя");
			var favouritedInfo = await _favouriteReceiptsRepository.GetAllBySelector(rf => rf.UserId == userId, cancellationToken).ToListAsync(cancellationToken);
			foreach (var info in favouritedInfo)
				await _favouriteReceiptsRepository.LoadReferenceAsync(info, rf => rf.Receipt);
			var favouritedReceipts = favouritedInfo.Select(fi => fi.Receipt).ToList();
			return Result.Success(favouritedReceipts);

		}

		public async Task<ReceiptFavourite?> GetAsync(Guid userId, Guid receiptId, CancellationToken cancellationToken = default)
		{
			return await _favouriteReceiptsRepository.GetBySelectorAsync(rf => rf.UserId == userId && rf.ReceiptId == receiptId, cancellationToken);
		}

		public async Task<Result<string>> RemoveAsync(Guid userId, Guid receiptId)
		{
			var res = await _favouriteReceiptsRepository.NotBulkDeleteAsync(rf => rf.UserId == userId && rf.ReceiptId == receiptId);
			return res;
		}
	}
}
