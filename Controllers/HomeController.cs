using System;
using System.Threading.Tasks;
using ElasticSearchNESTSample.Models;
using ElasticSearchNESTSample.Services;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearchNESTSample.Controllers
{
    public class HomeController : Controller
    {
        private IElasticSearchService _elasticSearchService;
        private IElasticClient _elasticClient;

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

            if (_elasticClient.Indices.Exists("GoroIndex").Exists)
            {
                await _elasticSearchService.DeleteIndexAsync();
            }

            var createIndexResponse = _elasticClient.Indices
                .Create("GoroIndex", c =>
                    c.Map<Content>(a => a.AutoMap())
                );

            _elasticSearchService.Insert();

            _elasticSearchService.SearchQuery();

            _elasticSearchService.GetMatchPhrase();

            _elasticSearchService.Filter();

            return View();
        }
    }
}
