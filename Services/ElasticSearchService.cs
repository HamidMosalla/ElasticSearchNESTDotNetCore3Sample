using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchNESTSample.Models;
using Nest;

namespace ElasticSearchNESTSample.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticSearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public Task<ISearchResponse<Content>> SearchQueryAsync()
        {
            return _elasticClient.SearchAsync<Content>(s =>
                s.From(0).Size(10000).Query(q => q.Term(t => t.ContentId, 2)));

            /*
            GET ht-index/content/_search
            {
              "query": {
                "match":{
                  "contentText":"Louis"
                }
              }
            }
             */
        }

        public Task<ISearchResponse<Content>> GetMatchPhraseAsync(string matchPhrase)
        {
            return _elasticClient.SearchAsync<Content>(s => s
                    .From(0)
                    .Size(10000)
                    .Query(q =>
                        q.MatchPhrase(mq =>
                            mq.Field(f => f.ContentText).Query(matchPhrase))));
        }

        public async Task<List<ISearchResponse<Content>>> BulkMatchAsync(string matchPhrase)
        {
            // we need to use bulk method of the client instead
            string[] matchTerms =
            {
                "The quick",
                "Football",
                "Hockey",
                "Chicago Bears",
                "St. Louis"
            };

            var matchTasks = matchTerms.Select(m => _elasticClient.SearchAsync<Content>(s =>
                s
                    .From(0)
                    .Size(10000)
                    .Query(q => q.Match(mq => mq.Field(f => f.ContentText).Query(m)))));

            return (await Task.WhenAll(matchTasks)).ToList();
        }

        public Task<ISearchResponse<Content>> FilterAsync()
        {
            return _elasticClient.SearchAsync<Content>(s =>
                s
                    .From(0)
                    .Size(10000)
                    .Query(q => q
                        .Bool(b => b
                            .Filter(filter =>
                                filter.Range(m => m.Field(fld => fld.ContentId).GreaterThanOrEquals(4)))
                        )
                    ));
        }

        public async Task<IReadOnlyCollection<IndexResponse>> BulkInsertAsync(IReadOnlyCollection<string> contents)
        {
            var tasks = contents.Select((value, index) => _elasticClient.IndexAsync(new Content
            {
                ContentId = index,
                PostDate = DateTime.Now,
                ContentText = value
            }, i => i.Index("ht-index"))).ToList();

            return await Task.WhenAll(tasks);

            // To confirm you added data from "Content", you can type this in GET contentindex/_search
        }

        public Task<DeleteIndexResponse> DeleteIndexAsync()
        {
            return _elasticClient.Indices.DeleteAsync("ht-index");
        }
    }

}
