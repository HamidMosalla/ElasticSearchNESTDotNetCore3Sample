using System.Collections.Generic;
using System.Threading.Tasks;
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
                    c.Map<Avatar>(a => a.AutoMap())
                );

            var avatars = new List<Avatar>
            {
                new Avatar
                {
                    Id = 1,
                    FirstName = "Hamid",
                    LastName = "Mosalla",
                    Email = "Xellarix@gmail.com",
                    CurrentPosition = "developer",
                    Country = "France",
                    PhoneNumber = "3234234234"
                },
                new Avatar
                {
                    Id = 2,
                    FirstName = "Jimmy",
                    LastName = "Late",
                    Email = "jlate@gmail.com",
                    CurrentPosition = "architect",
                    Country = "France",
                    PhoneNumber = "5557657645"
                },
                new Avatar
                {
                    Id = 3,
                    FirstName = "Malfo",
                    LastName = "Kiev",
                    Email = "mkiev@gmail.com",
                    CurrentPosition = "manager",
                    Country = "France",
                    PhoneNumber = "987767685"
                }
            };

            var bulkInsertResult = await _elasticSearchService.BulkInsertAsync(avatars);

            var searchQueryResult = await _elasticSearchService.SearchQueryAsync();

            string[] matchTerms =
            {
                "Hamid",
                "Jimmy"
            };

            var bulkMatchResult = await _elasticSearchService.BulkMatchAsync(matchTerms);

            string matchPhrase = "developer";

            var matchPhraseResult = await _elasticSearchService.GetMatchPhraseAsync(matchPhrase);

            var filterResult = await _elasticSearchService.FilterAsync();

            return View();
        }
    }
}
