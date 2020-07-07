using System;
using System.Threading.Tasks;
using ElasticSearchNESTSample.Models;
using ElasticSearchNESTSample.Services;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearchNESTSample.Controllers
{
    // check out this handy site
    // https://hassantariqblog.wordpress.com/category/back-end-stuff/elastic-search/

    // and the .net NEST lib is in.  https://github.com/elastic/elasticsearch-net

    // Create properties from the Post class
    // https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/fluent-mapping.html

    // after creating this, you can issue
    // GET /myindex    in the command area.
    // https://www.elastic.co/guide/en/elasticsearch/reference/current/indices-get-index.html

    public class HomeController : Controller
    {
        public static Uri node;
        public static ConnectionSettings settings;
        public static ElasticClient client;
        private readonly ElasticSearchService _elasticSearchService = new ElasticSearchService();

        public async Task<IActionResult> Index()
        {

            node = new Uri("http://localhost:9200");
            settings = new ConnectionSettings(node);
            settings.DefaultIndex("contentidx");
            client = new ElasticClient(settings);

            var indexSettings = new IndexSettings();
            indexSettings.NumberOfReplicas = 1;
            indexSettings.NumberOfShards = 1;

            if (client.Indices.Exists("contentidx").Exists)
            {
                await _elasticSearchService.DeleteIndexAsync();
            }

            var createIndexResponse = client.Indices
                .Create("contentidx", c =>
                    c.Map<Content>(a => a.AutoMap())
                );

            _elasticSearchService.Insert();

            _elasticSearchService.SearchQuery();

            _elasticSearchService.GetMatchPhrase();

            _elasticSearchService.Filter();

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
