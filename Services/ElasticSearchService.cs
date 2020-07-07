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

        public async Task<CreateIndexResponse> CreateIndex(string indexName)
        {
            // TODO:Hamid: need settings later
            // var indexSettings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 1 };
            // c.InitializeUsing(indexSettings)

            if (_elasticClient.Indices.Exists(indexName).Exists)
            {
                await DeleteIndexAsync();
            }

            return await _elasticClient.Indices
                .CreateAsync(indexName, c =>
                    c.Map<Avatar>(a => a.AutoMap())
                );
        }

        public Task<ISearchResponse<Avatar>> SearchQueryAsync(int id)
        {
            return _elasticClient.SearchAsync<Avatar>(s =>
                s.From(0).Size(10000).Query(q => q.Term(t => t.Id, id)));

            /*
                POST ht-index/_search
                {
                   "query":{
                      "match":{
                         "firstName":"Hamid"
                      }
                   }
                }
            */
        }

        public Task<ISearchResponse<Avatar>> GetMatchPhraseAsync(string matchPhrase)
        {
            return _elasticClient.SearchAsync<Avatar>(s => s
                    .From(0)
                    .Size(10000)
                    .Query(q =>
                        q.MatchPhrase(mq =>
                            mq.Field(f => f.CurrentPosition).Query(matchPhrase))));
        }

        public async Task<List<ISearchResponse<Avatar>>> BulkMatchAsync(string[] matchTerms)
        {
            // we need to use bulk method of the client instead, this is just a sample
            var matchTasks = matchTerms.Select(m => _elasticClient.SearchAsync<Avatar>(term =>
                term.From(0)
                    .Size(10000)
                    .Query(q => q.Match(mq => mq.Field(f => f.FirstName).Query(m)))));

            return (await Task.WhenAll(matchTasks)).ToList();
        }

        public Task<MultiSearchResponse> MultiSearchAsync(string[] matchTerms)
        {
            var multiSearchRequest = new MultiSearchRequest
            {
                Operations = matchTerms.ToDictionary(a => a, a => new SearchRequest<Avatar>
                {
                    Query = new MatchQuery
                    {
                        Field = "FirstName",
                        Query = a
                    },
                    //new TermQuery
                    //{
                    //    Field = "FirstName",
                    //    Value = value
                    //},
                } as ISearchRequest)
            };

            return _elasticClient.MultiSearchAsync(multiSearchRequest);
        }

        public Task<ISearchResponse<Avatar>> FilterAsync()
        {
            return _elasticClient.SearchAsync<Avatar>(s =>
                s
                    .From(0)
                    .Size(10000)
                    .Query(q => q
                        .Bool(b => b
                            .Filter(filter =>
                                filter.Range(m => m.Field(fld => fld.Id).GreaterThanOrEquals(3)))
                        )
                    ));
        }

        public Task<IndexResponse> IndexAsync(Avatar avatar)
        {
            return _elasticClient.IndexAsync(avatar, i => i.Index("ht-index"));

            // To confirm you added data from Avatars, you can type this in: GET /ht-index/_search
        }

        public Task<BulkResponse> BulkIndexAsync(IReadOnlyCollection<Avatar> avatars)
        {
            return _elasticClient.IndexManyAsync(avatars, "ht-index");

            // To confirm you added data from Avatars, you can type this in: GET /ht-index/_search
        }

        public async Task<IReadOnlyCollection<IndexResponse>> BulkIndexExperimentalAsync(IReadOnlyCollection<Avatar> contents)
        {
            var tasks = contents.Select((value, index) => _elasticClient.IndexAsync(value, i => i.Index("ht-index"))).ToList();

            return await Task.WhenAll(tasks);

            // To confirm you added data from Avatars, you can type this in: GET /ht-index/_search
        }

        public Task<DeleteIndexResponse> DeleteIndexAsync()
        {
            return _elasticClient.Indices.DeleteAsync("ht-index");
        }
    }

}
