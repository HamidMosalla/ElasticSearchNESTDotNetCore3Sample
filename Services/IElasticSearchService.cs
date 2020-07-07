using System.Threading.Tasks;
using Nest;

namespace ElasticSearchNESTSample.Services
{
    public interface IElasticSearchService
    {
        void SearchQuery();
        void GetMatchPhrase();
        void Filter();
        void Insert();
        Task<DeleteIndexResponse> DeleteIndexAsync();
    }
}