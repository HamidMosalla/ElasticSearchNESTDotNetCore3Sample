using System;
using System.Threading.Tasks;
using ElasticSearchNESTSample.Controllers;
using ElasticSearchNESTSample.Models;
using Nest;

namespace ElasticSearchNESTSample.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        public void SearchQuery()
        {
            var result = HomeController.client.Search<Content>(s =>
                s.From(0).Size(10000).Query(q => q.Term(t => t.ContentId, 2)));
            /*
            GET contentidx/content/_search
            {
              "query": {
                "match":{
                  "contentText":"Louis"
                }
              }
            }
             */

            string[] matchTerms =
            {
                "The quick",  // will find two entries.  Two with "the" and one with "quick"(but that has "the" as well with a score of 2)
                "Football",
                "Hockey",
                "Chicago Bears",
                "St. Louis"
            };

            // Match terms would come from what the user typed in
            foreach (var term in matchTerms)
            {
                result = HomeController.client.Search<Content>(s =>
                    s
                        .From(0)
                        .Size(10000)
                        .Query(q => q.Match(mq => mq.Field(f => f.ContentText).Query(term))));
                // print out the result.
            }
        }

        public void GetMatchPhrase()
        {
            // Exact phrase matching
            string[] matchPhrases =
            {
                "The quick",
                "Louis Blues",
                "Chicago Bears"
            };

            // Match terms would come from what the user typed in
            foreach (var phrase in matchPhrases)
            {
                var result = HomeController.client.Search<Content>(s =>
                    s
                        .From(0)
                        .Size(10000)
                        .Query(q => q.MatchPhrase(mq => mq.Field(f => f.ContentText).Query(phrase))));
                // print out the result.
            }
        }

        public void Filter()
        {
            var result = HomeController.client.Search<Content>(s =>
                s
                    .From(0)
                    .Size(10000)
                    .Query(q => q
                        .Bool(b => b
                            .Filter(filter => filter.Range(m => m.Field(fld => fld.ContentId).GreaterThanOrEquals(4)))
                        )
                    ));
            // print out the result.            
        }

        public void Insert()
        {
            // Insert data

            string[] contentText =
            {
                "<p>Chicago Cubs Baseball</p>",
                "<html><body><p>St. Louis Cardinals Baseball</p></body></html>",
                "St. Louis Blues Hockey",
                "The Chicago Bears Football",
                "The quick fox jumped over the lazy dog"
            };

            int idx = 1;
            foreach (var text in contentText)
            {
                var simulatedContentFromDB = new Content()
                {
                    ContentId = idx++,
                    PostDate = DateTime.Now,
                    ContentText = text
                };
                // this will insert
                // See https://hassantariqblog.wordpress.com/2016/09/21/elastic-search-insert-documents-in-index-using-nest-in-net/
                HomeController.client.Index(simulatedContentFromDB, i => i.Index("contentidx"));
            }

            // To confirm you added data from "Content", you can type this in
            // GET contentindex/_search
        }

        public Task<DeleteIndexResponse> DeleteIndexAsync()
        {
            return HomeController.client.Indices.DeleteAsync("contentidx");
        }
    }

}