namespace ElasticSearchNESTSample.Services
{
    public interface IElasticSearchService
    {
        void TestTermQuery();
        void TestMatchPhrase();
        void TestFilter();
        void TestInsert();
        void TestDeleteIndex();
    }
}