using System;
using System.Threading.Tasks;
using Bogus;
using ElasticSearchNESTSample.Models;
using ElasticSearchNESTSample.Services;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearchNESTSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IElasticSearchService _elasticSearchService;
        private readonly IElasticClient _elasticClient;

        public HomeController(IElasticSearchService elasticSearchService, IElasticClient elasticClient)
        {
            _elasticSearchService = elasticSearchService;
            _elasticClient = elasticClient;
        }

        public async Task<IActionResult> Index()
        {
            // TODO:Hamid: need settings later
            // var indexSettings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 1 };
            // c.InitializeUsing(indexSettings)

            if (_elasticClient.Indices.Exists("ht-index").Exists)
            {
                await _elasticSearchService.DeleteIndexAsync();
            }

            var createIndexResponse = await _elasticClient.Indices
                .CreateAsync("ht-index", c =>
                    c.Map<Content>(a => a.AutoMap())
                );

            // Avatar
            var avatarsToIndex = new Faker<Avatar>()
                .RuleFor(r => r.Id, r => r.Random.Guid())
                .RuleFor(r => r.FirstName, r => r.Random.Word())
                .RuleFor(r => r.LastName, r => r.Random.Word())
                .RuleFor(r => r.Country, r => r.Random.Word())
                .RuleFor(r => r.CurrentPosition, r => r.Random.Word())
                .RuleFor(r => r.Email, r => r.Random.Word())
                .RuleFor(r => r.PhoneNumber, r => r.Random.Word())
                .Generate(100);

            string[] textStrings =
            {
                "<p>Chicago Cubs Baseball</p>",
                "<html><body><p>St. Louis Cardinals Baseball</p></body></html>",
                "St. Louis Blues Hockey",
                "The Chicago Bears Football",
                "The quick fox jumped over the lazy dog"
            };

            var bulkInsertResult = await _elasticSearchService.BulkInsertAsync(textStrings);

            var searchQueryResult = await _elasticSearchService.SearchQueryAsync();

            string matchPhrase = "The quick";

            var matchPhraseResult = await _elasticSearchService.GetMatchPhraseAsync(matchPhrase);

            var filterResult = await _elasticSearchService.FilterAsync();

            return View();
        }
    }
}
