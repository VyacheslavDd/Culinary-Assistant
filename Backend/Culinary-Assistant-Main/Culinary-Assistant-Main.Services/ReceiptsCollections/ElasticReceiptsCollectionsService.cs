using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.Options;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptsCollections
{
	public class ElasticReceiptsCollectionsService(ILogger logger, IOptions<ElasticSearchOptions> options) : IElasticReceiptsCollectionsService
	{
		private readonly ILogger _logger = logger;
		private readonly ElasticsearchClient _elasticSearchClient = new(new Uri(options.Value.Url));

		public async Task CreateReceiptsCollectionsIndexAsync()
		{
			var indexExists = await _elasticSearchClient.Indices.ExistsAsync(MiscellaneousConstants.ReceiptsCollectionsElasticSearchIndex);
			if (indexExists.Exists) return;
			var response = await _elasticSearchClient.Indices.CreateAsync(MiscellaneousConstants.ReceiptsCollectionsElasticSearchIndex, c =>
			{
				c.Settings(s =>
				{
					s.Analysis(a =>
					{
						a.TokenFilters(tf => tf
								.NGram("ngram_filter", ngtf => ngtf
								.MinGram(4).MaxGram(5)))
						.Analyzers(anl => anl.Custom("ngram_analyzer", na => na.Tokenizer("standard").Filter(["lowercase", "ngram_filter"])));
					});
				})
				.Mappings(m =>
				{
					m.Properties<ReceiptCollectionIndexDTO>(conf =>
					{
						conf.Text(p => p.Title, rc => rc.Analyzer("russian").Fields(f => f.Text("ngrams", n => n.Analyzer("ngram_analyzer"))));
					});
				});
			});
			if (!response.IsValidResponse)
				_logger.Error("Не удалось создать индекс для коллекций рецептов");
		}

		public async Task<Result<List<Guid>>> GetReceiptsCollectionsIdsAsync(string searchByTitle)
		{
			var response = await _elasticSearchClient.SearchAsync<ReceiptCollectionIndexDTO>(MiscellaneousConstants.ReceiptsCollectionsElasticSearchIndex, c =>
			{
				c.Query(q =>
				{
					q.MultiMatch(m => m.Fields(Fields.FromFields(["title", "title.ngrams"])).Query(searchByTitle));
				})
				.From(0)
				.Size(1000);
			});
			if (!response.IsValidResponse)
				return CSharpFunctionalExtensions.Result.Failure<List<Guid>>("Не удалось осуществить поиск коллекций по индексам");
			var guids = response.Documents.Select(d => d.Id).ToList();
			return CSharpFunctionalExtensions.Result.Success(guids);
		}

		public async Task DeleteReceiptCollectionFromIndexAsync(Guid collectionId)
		{
			var response = await _elasticSearchClient.DeleteAsync<ReceiptCollectionIndexDTO>(MiscellaneousConstants.ReceiptsCollectionsElasticSearchIndex, collectionId);
			if (!response.IsValidResponse)
				_logger.Error("Не удалось удалить коллекцию рецептов {@id} из индекса", collectionId);
		}


		public async Task IndexReceiptCollectionAsync(string title, Guid collectionId)
		{
			var response = await _elasticSearchClient.IndexAsync(new ReceiptCollectionIndexDTO(collectionId, title),
				MiscellaneousConstants.ReceiptsCollectionsElasticSearchIndex, collectionId);
			if (!response.IsValidResponse)
				_logger.Error("Не удалось добавить коллекцию рецептов {@id} в индекс", collectionId);
		}

		public async Task ReindexReceiptCollectionAsync(string title, Guid collectionId)
		{
			var response = await _elasticSearchClient.UpdateAsync<ReceiptCollectionIndexDTO, ReceiptCollectionIndexDTO>(MiscellaneousConstants.ReceiptsCollectionsElasticSearchIndex,
				collectionId, u => u.Doc(new ReceiptCollectionIndexDTO(collectionId, title)));
			if (!response.IsValidResponse)
				_logger.Error("Не удалось обновить коллекцию рецептов {@id} в индексе", collectionId);
		}
	}
}
