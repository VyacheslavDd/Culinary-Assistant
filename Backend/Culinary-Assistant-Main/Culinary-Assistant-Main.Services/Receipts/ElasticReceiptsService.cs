using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant_Main.Domain.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Receipts
{
	public class ElasticReceiptsService(IOptions<ElasticSearchOptions> options, ILogger logger) : IElasticReceiptsService
	{

		private readonly ILogger _logger = logger;
		private readonly ElasticsearchClient _elasticsearchClient = new(new ElasticsearchClientSettings(new Uri(options.Value.Url)));

		private readonly Func<Receipt, Guid, ReceiptIndexDto> _receiptToIndexDTO = (Receipt receipt, Guid id) =>
		{
			var receiptIngredients = string.Join(" ", JsonSerializer.Deserialize<List<Ingredient>>(receipt.Ingredients).Select(i => i.Name));
			var receiptDto = new ReceiptIndexDto(id, receipt.Title.Value, receiptIngredients, receipt.CookingTime);
			return receiptDto;
		};

		public async Task CreateReceiptsIndexAsync()
		{
			var indexExists = await _elasticsearchClient.Indices.ExistsAsync(MiscellaneousConstants.ReceiptsElasticSearchIndex);
			if (!indexExists.Exists)
			{
				var createIndexResponse = await _elasticsearchClient.Indices.CreateAsync(MiscellaneousConstants.ReceiptsElasticSearchIndex, c => c
					.Settings(s => s
							.Analysis(a => a
							.TokenFilters(tf => tf
								.NGram("ngram_filter", ngtf => ngtf
								.MinGram(4).MaxGram(5)))
							.Analyzers(an => an
							.Custom("ngram_analyzer", na => na.Tokenizer("standard").Filter(["lowercase", "ngram_filter"])))))
					.Mappings(m => m
							.Properties<ReceiptIndexDto>(p => p
							.Text(r => r.Title, p => p.Analyzer("russian").Fields(f => f.Text("ngrams", n => n.Analyzer("ngram_analyzer"))).Boost(2.0))
							.Text(r => r.Ingredients, p => p.Analyzer("russian").Fields(f => f.Text("ngrams", n => n.Analyzer("ngram_analyzer"))))
							.IntegerNumber(r => r.CookingTime))));
				if (!createIndexResponse.IsValidResponse)
					_logger.Error("Не удалось создать индекс для рецептов.");
			}
		}

		public async Task<CSharpFunctionalExtensions.Result<List<Guid>>> GetReceiptIdsBySearchParametersAsync(ReceiptsFilterForElasticSearch receiptsFilterForElasticSearch)
		{
			var hasTitleSearch = receiptsFilterForElasticSearch.TitleQuery != "";
			List<Action<QueryDescriptor<ReceiptIndexDto>>> searchActions = hasTitleSearch ?
				[
				 (QueryDescriptor<ReceiptIndexDto> descriptor) =>
				 descriptor.MultiMatch(mq => mq.Fields(Fields.FromFields(["title", "title.ngrams"])).Query(receiptsFilterForElasticSearch.TitleQuery).Boost(2.0f))
				] : [];
			foreach (var ingredientsQuery in receiptsFilterForElasticSearch.IngredientsQuery)
			{
				searchActions.Add((QueryDescriptor<ReceiptIndexDto> descriptor) =>
				descriptor.MultiMatch(mq => mq.Fields(Fields.FromFields(["ingredients", "ingredients.ngrams"])).Query(ingredientsQuery).Boost(1.0f)));
			}
			var response = await _elasticsearchClient.SearchAsync<ReceiptIndexDto>(MiscellaneousConstants.ReceiptsElasticSearchIndex, c =>
			{
			    c.Query(q =>
				{
					q.Bool(b =>
					{
						b.Must([..searchActions]);
					});
				})
				.From(0)
				.Size(1000);
			});
			if (!response.IsValidResponse)
			{
				var message = "Ошибка выполнения запроса поиска рецептов";
				_logger.Error(response.ElasticsearchServerError?.Error?.Reason);
				return CSharpFunctionalExtensions.Result.Failure<List<Guid>>(message);
			}
			var guids = response.Documents.Select(r => r.Id).ToList();
			return CSharpFunctionalExtensions.Result.Success(guids);
		}

		public async Task IndexReceiptAsync(Receipt receipt, Guid receiptId)
		{
			var receiptDto = _receiptToIndexDTO(receipt, receiptId);
			var response = await _elasticsearchClient.IndexAsync(receiptDto, MiscellaneousConstants.ReceiptsElasticSearchIndex, receiptId);
			if (!response.IsValidResponse)
				_logger.Error("Не удалось проиндексировать рецепт: {@receipt}", receipt);
		}

		public async Task ReindexReceiptAsync(Receipt receipt)
		{
			var receiptDto = _receiptToIndexDTO(receipt, receipt.Id);
			var response = await _elasticsearchClient.UpdateAsync<ReceiptIndexDto, ReceiptIndexDto>(MiscellaneousConstants.ReceiptsElasticSearchIndex, receipt.Id, u => u.Doc(receiptDto));
			if (!response.IsValidResponse)
				_logger.Error("Не удалось переиндексировать рецепт: {@receipt}", receipt);
		}

		public async Task RemoveReceiptIndexAsync(Receipt receipt)
		{
			var response = await _elasticsearchClient.DeleteAsync<ReceiptIndexDto>(MiscellaneousConstants.ReceiptsElasticSearchIndex, receipt.Id);
			if (!response.IsValidResponse)
				_logger.Error("Не удалось удалить индекс для рецепта: {@receipt}", receipt);
		}
	}
}
