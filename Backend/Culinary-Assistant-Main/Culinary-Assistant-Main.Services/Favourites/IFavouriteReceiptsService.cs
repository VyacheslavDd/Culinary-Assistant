using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.DTO.Favourite;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Favourites
{
	public interface IFavouriteReceiptsService
	{
		Task<Result<List<Receipt>>> GetAllReceiptsFavouritedForUserAsync(Guid userId, CancellationToken cancellationToken = default);
		Task<List<ReceiptFavourite>> GetAllAsync();
		Task<Result<Guid>> AddAsync(FavouriteInDTO favouriteInDTO);
		Task<Result<string>> RemoveAsync(Guid userId, Guid receiptId);
		Task<ReceiptFavourite?> GetAsync(Guid userId, Guid receiptId, CancellationToken cancellationToken = default);
		Task ApplyFavouritesInfoToReceiptsDataAsync<T>(ClaimsPrincipal user, List<T> receiptsData) where T : IFavouritedDTO;
	}
}
