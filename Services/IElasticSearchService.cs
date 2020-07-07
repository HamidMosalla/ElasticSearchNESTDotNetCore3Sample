using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticSearchNESTSample.Models;
using Nest;

namespace ElasticSearchNESTSample.Services
{
    public interface IElasticSearchService
    {
        Task<ISearchResponse<Avatar>> SearchQueryAsync(int id);
        Task<ISearchResponse<Avatar>> GetMatchPhraseAsync(string matchPhrase);
        Task<List<ISearchResponse<Avatar>>> BulkMatchAsync(string[] matchTerms);
        Task<ISearchResponse<Avatar>> FilterAsync();
        Task<IndexResponse> IndexAsync(Avatar avatar);
        Task<BulkResponse> BulkIndexAsync(IReadOnlyCollection<Avatar> avatars);
        Task<IReadOnlyCollection<IndexResponse>> BulkIndexExperimentalAsync(IReadOnlyCollection<Avatar> contents);
        Task<DeleteIndexResponse> DeleteIndexAsync();
    }
}
