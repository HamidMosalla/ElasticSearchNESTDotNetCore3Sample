using System;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ElasticSearchNESTSample.ServiceCollectionExtensions
{
    public static class ElasticSearchExtensions
    {
        public static IServiceCollection RegisterElasticEndpoint(this IServiceCollection services)
        {
            var node = new Uri("http://localhost:9200");

            var settings = new ConnectionSettings(node);

            var elasticClient = new ElasticClient(settings);

            settings.DefaultIndex("GoroIndex");

            services.AddTransient<IElasticClient>(e => elasticClient);

            return services;
        }
    }
}
