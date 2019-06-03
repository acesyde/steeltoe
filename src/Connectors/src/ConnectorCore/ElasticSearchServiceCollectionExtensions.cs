// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Steeltoe.CloudFoundry.Connector.ElasticSearch
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Services;
    using Steeltoe.Common.HealthChecks;

    public static class ElasticSearchServiceCollectionExtensions
    {
        /// <summary>
        /// Add an ElasticSearch to a ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="config">App configuration</param>
        /// <param name="contextLifetime">Lifetime of the service to inject</param>
        /// <param name="logFactory">logger factory</param>
        /// <param name="healthChecksBuilder">Microsoft HealthChecksBuilder</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration config, ServiceLifetime contextLifetime = ServiceLifetime.Singleton, ILoggerFactory logFactory = null, IHealthChecksBuilder healthChecksBuilder = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            ElasticSearchServiceInfo info = config.GetSingletonServiceInfo<ElasticSearchServiceInfo>();

            DoAdd(services, info, config, contextLifetime, healthChecksBuilder);

            return services;
        }

        /// <summary>
        /// Add an ElasticSearch to a ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="config">App configuration</param>
        /// <param name="serviceName">cloud foundry service name binding</param>
        /// <param name="contextLifetime">Lifetime of the service to inject</param>
        /// <param name="logFactory">logger factory</param>
        /// <param name="healthChecksBuilder">Microsoft HealthChecksBuilder</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration config, string serviceName, ServiceLifetime contextLifetime = ServiceLifetime.Singleton, ILoggerFactory logFactory = null, IHealthChecksBuilder healthChecksBuilder = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            ElasticSearchServiceInfo info = config.GetRequiredServiceInfo<ElasticSearchServiceInfo>(serviceName);

            DoAdd(services, info, config, contextLifetime, healthChecksBuilder);

            return services;
        }

        private static void DoAdd(IServiceCollection services, ElasticSearchServiceInfo info, IConfiguration config, ServiceLifetime contextLifetime, IHealthChecksBuilder healthChecksBuilder)
        {
            Type elasticSearchClient = ElasticSearchTypeLocator.ElasticClient;
            var elasticSearchConnectorOptions = new ElasticSearchConnectorOptions(config);
            var clientFactory = new ElasticSearchConnectorFactory(info, elasticSearchConnectorOptions, elasticSearchClient);
            services.Add(new ServiceDescriptor(ElasticSearchTypeLocator.IElasticClient, clientFactory.Create, contextLifetime));
            services.Add(new ServiceDescriptor(elasticSearchClient, clientFactory.Create, contextLifetime));
            if (healthChecksBuilder == null)
            {
                services.Add(new ServiceDescriptor(typeof(IHealthContributor), ctx => new ElasticSearchHealthContributor(clientFactory, ctx.GetService<ILogger<ElasticSearchHealthContributor>>()), ServiceLifetime.Singleton));
            }
            else
            {
                healthChecksBuilder.AddElasticsearch(clientFactory.CreateConnectionString());
            }

            Type type = ConnectorHelpers.FindType(ElasticSearchTypeLocator.Assemblies, ElasticSearchTypeLocator.ElasticSearchConnectionInfo);
            var urlFactory = new ElasticSearchConnectorFactory(info, elasticSearchConnectorOptions, type);
            services.Add(new ServiceDescriptor(type, urlFactory.Create, contextLifetime));
        }
    }
}
