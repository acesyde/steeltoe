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

using System;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.Services;
using Steeltoe.Common.HealthChecks;

namespace Steeltoe.CloudFoundry.Connector.ElasticSearch
{
    public class ElasticSearchHealthContributor : IHealthContributor
    {
        public static IHealthContributor GetElasticSearchHealthContributor(IConfiguration configuration, ILogger<ElasticSearchHealthContributor> logger = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Type mongoDbImplementationType = ElasticSearchTypeLocator.ElasticSearchClient;
            var info = configuration.GetSingletonServiceInfo<ElasticSearchServiceInfo>();

            ElasticSearchConnectorOptions connectorOptions = new ElasticSearchConnectorOptions(configuration);
            ElasticSearchConnectorFactory factory = new ElasticSearchConnectorFactory(info, connectorOptions, mongoDbImplementationType);
            return new ElasticSearchHealthContributor(factory, logger);
        }

        private readonly ILogger<ElasticSearchHealthContributor> _logger;
        private readonly object _client;

        public string Id => "ElasticSearch";

        public ElasticSearchHealthContributor(ElasticSearchConnectorFactory factory, ILogger<ElasticSearchHealthContributor> logger = null)
        {
            _logger = logger;
            _client = factory.Create(null);
        }

        public HealthCheckResult Health()
        {
            _logger?.LogTrace("Checking ElasticSearch connection health");
            var result = new HealthCheckResult();
            try
            {
                var databases = ConnectorHelpers.Invoke(ElasticSearchTypeLocator.ListDatabasesMethod, _client, new object[] { new CancellationTokenSource(5000) });

                if (databases == null)
                {
                    throw new ConnectorException("Failed to open ElasticSearch connection!");
                }

                result.Details.Add("status", HealthStatus.UP.ToString());
                result.Status = HealthStatus.UP;
                _logger?.LogTrace("ElasticSearch connection is up!");
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                {
                    e = e.InnerException;
                }

                _logger?.LogError("ElasticSearch connection is down! {HealthCheckException}", e.Message);
                result.Details.Add("error", e.GetType().Name + ": " + e.Message);
                result.Details.Add("status", HealthStatus.DOWN.ToString());
                result.Status = HealthStatus.DOWN;
                result.Description = e.Message;
            }

            return result;
        }
    }
}
