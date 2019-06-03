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

using Steeltoe.CloudFoundry.Connector.ElasticSearch;

namespace Steeltoe.CloudFoundry.ConnectorAutofac
{
    using System;
    using Autofac;
    using Autofac.Builder;
    using Common.HealthChecks;
    using Connector;
    using Connector.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public static class ElasticSearchContainerBuilderExtensions
    {
        /// <summary>
        /// Adds ElasticSearch classes (ElasticClient, IElasticClient and ElasticSearchUrl) to your Autofac Container
        /// </summary>
        /// <param name="container">Your Autofac Container Builder</param>
        /// <param name="config">Application configuration</param>
        /// <param name="serviceName">Cloud Foundry service name binding</param>
        /// <returns>the RegistrationBuilder for (optional) additional configuration</returns>
        public static IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> RegisterElasticSearchConnection(this ContainerBuilder container, IConfiguration config, string serviceName = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            ElasticSearchServiceInfo info = serviceName == null
                ? config.GetSingletonServiceInfo<ElasticSearchServiceInfo>()
                : config.GetRequiredServiceInfo<ElasticSearchServiceInfo>(serviceName);

            var mongoOptions = new ElasticSearchConnectorOptions(config);
            var clientFactory = new ElasticSearchConnectorFactory(info, mongoOptions, ElasticSearchTypeLocator.ElasticClient);
            var urlFactory = new ElasticSearchConnectorFactory(info, mongoOptions, ElasticSearchTypeLocator.ElasticSearchUrl);

            container.Register(c => urlFactory.Create(null)).As(ElasticSearchTypeLocator.ElasticSearchUrl);
            container.Register(c => new ElasticSearchHealthContributor(clientFactory, c.ResolveOptional<ILogger<ElasticSearchHealthContributor>>())).As<IHealthContributor>();

            return container.Register(c => clientFactory.Create(null)).As(ElasticSearchTypeLocator.IElasticClient, ElasticSearchTypeLocator.ElasticClient);
        }
    }
}
