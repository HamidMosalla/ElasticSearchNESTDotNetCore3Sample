using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticSearchNESTSample.Models;
using Nest;

namespace ElasticSearchNESTSample.Services
{
    public interface IElasticSearchService
    {
        Task<ISearchResponse<Content>> SearchQueryAsync();
        Task<ISearchResponse<Content>> GetMatchPhraseAsync(string matchPhrase);
        Task<List<ISearchResponse<Content>>> BulkMatchAsync(string matchPhrase);
        Task<ISearchResponse<Content>> FilterAsync();
        Task<IReadOnlyCollection<IndexResponse>> BulkInsertAsync(IReadOnlyCollection<string> contents);
        Task<DeleteIndexResponse> DeleteIndexAsync();
    }
}