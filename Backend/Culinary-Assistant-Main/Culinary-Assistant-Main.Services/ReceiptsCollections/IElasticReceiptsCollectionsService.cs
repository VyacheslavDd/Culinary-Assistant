using CSharpFunctionalExtensions;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptsCollections
{
	public interface IElasticReceiptsCollectionsService
	{
		Task CreateReceiptsCollectionsIndexAsync();
		Task<Result<List<Guid>>> GetReceiptsCollectionsIdsAsync(string searchByTitle);
		Task IndexReceiptCollectionAsync(string title, Guid receiptCollectionId);
		Task ReindexReceiptCollectionAsync(string title, Guid receiptCollectionId);
		Task DeleteReceiptCollectionFromIndexAsync(Guid receiptCollectionId);
	}
}
