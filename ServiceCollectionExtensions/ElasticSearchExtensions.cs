using System;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ElasticSearchNESTSample.ServiceCollectionExtensions
{
    public static class ElasticSearchExtensions
    {
        public static IServiceCollection RegisterElasticEndpoint(this IServiceCollection services, string indexName)
        {
            var node = new Uri("http://localhost:9200");

            var settings = new ConnectionSettings(node).PrettyJson();

            var elasticClient = new ElasticClient(settings);

            settings.DefaultIndex(indexName);

            services.AddTransient<IElasticClient>(e => elasticClient);

            return services;
        }
    }
}
