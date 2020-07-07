using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticSearchNESTSample.Models;
using Nest;

namespace ElasticSearchNESTSample.Services
{
    public interface IElasticSearchService
    {
        Task<ISearchResponse<Avatar>> SearchQueryAsync();
        Task<ISearchResponse<Avatar>> GetMatchPhraseAsync(string matchPhrase);
        Task<List<ISearchResponse<Avatar>>> BulkMatchAsync(string[] matchTerms);
        Task<ISearchResponse<Avatar>> FilterAsync();
        Task<IReadOnlyCollection<IndexResponse>> BulkInsertAsync(IReadOnlyCollection<Avatar> contents);
        Task<DeleteIndexResponse> DeleteIndexAsync();
    }
}