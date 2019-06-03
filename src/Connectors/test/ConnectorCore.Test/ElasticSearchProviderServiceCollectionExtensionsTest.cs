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

using System.Linq;

namespace Steeltoe.CloudFoundry.Connector.ElasticSearch.Test
{
    using System;
    using System.Collections;
    using Elasticsearch.Net;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    using Nest;
    using Steeltoe.CloudFoundry.Connector.Test;
    using Steeltoe.Common.HealthChecks;
    using Steeltoe.Extensions.Configuration.CloudFoundry;
    using Xunit;

    public class ElasticSearchProviderServiceCollectionExtensionsTest
    {
        public ElasticSearchProviderServiceCollectionExtensionsTest()
        {
            Environment.SetEnvironmentVariable("VCAP_APPLICATION", null);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", null);
        }

        [Fact]
        public void AddElasticSearch_ThrowsIfServiceCollectionNull()
        {
            // Arrange
            IServiceCollection services = null;
            IConfigurationRoot config = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => services.AddElasticSearch(config));
            Assert.Contains(nameof(services), ex.Message);

            var ex2 = Assert.Throws<ArgumentNullException>(() => services.AddElasticSearch(config, "foobar"));
            Assert.Contains(nameof(services), ex2.Message);
        }

        [Fact]
        public void AddElasticSearch_ThrowsIfConfigurationNull()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => services.AddElasticSearch(config));
            Assert.Contains(nameof(config), ex.Message);

            var ex2 = Assert.Throws<ArgumentNullException>(() => services.AddElasticSearch(config, "foobar"));
            Assert.Contains(nameof(config), ex2.Message);
        }

        [Fact]
        public void AddElasticSearch_ThrowsIfServiceNameNull()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = null;
            string serviceName = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => services.AddElasticSearch(config, serviceName));
            Assert.Contains(nameof(serviceName), ex.Message);
        }

        [Fact]
        public void AddElasticSearch_NoVCAPs_AddsElasticSearchClient()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder().Build();

            // Act
            services.AddElasticSearch(config);
            var service = services.BuildServiceProvider().GetService<ElasticClient>();

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void AddElasticSearch_WithServiceName_NoVCAPs_ThrowsConnectorException()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder().Build();

            // Act and Assert
            var ex = Assert.Throws<ConnectorException>(() => services.AddElasticSearch(config, "foobar"));
            Assert.Contains("foobar", ex.Message);
        }

        [Fact]
        public void AddElasticSearch_With_a9s_single_VCAPs_AddsElasticSearchClient()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", ElasticSearchTestHelpers.SingleBinding_a9s_SingleServer_VCAP);
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            // Act
            services.AddElasticSearch(config);
            var service = services.BuildServiceProvider().GetService<ElasticClient>();

            // Assert
            Assert.NotNull(service);
            var connSettings = service.ConnectionSettings;
            Assert.Single(connSettings.ConnectionPool.Nodes as IEnumerable);
            Assert.Equal("d8790b7-mongodb-0.node.dc1.a9s-mongodb-consul", connSettings.ConnectionPool.Nodes.First().Uri.Host);
            Assert.Equal(9200, connSettings.ConnectionPool.Nodes.First().Uri.Port);
//            Assert.Equal("a9s-brk-usr-377ad48194cbf0452338737d7f6aa3fb6cdabc24", connSettings.Credential.Username);
        }

        [Fact]
        public void AddElasticSearch_With_a9s_replicas_VCAPs_AddsElasticSearchClient()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", ElasticSearchTestHelpers.SingleBinding_a9s_WithReplicas_VCAP);
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            // Act
            services.AddElasticSearch(config);
            var service = services.BuildServiceProvider().GetService<ElasticClient>();

            // Assert
            Assert.NotNull(service);
            var connSettings = service.ConnectionSettings;
            Assert.Contains(new Node(new Uri("d5584e9-mongodb-0.node.dc1.a9s-mongodb-consul")), connSettings.ConnectionPool.Nodes);
            Assert.Contains(new Node(new Uri("d5584e9-mongodb-0.node.dc1.a9s-mongodb-consul")), connSettings.ConnectionPool.Nodes);
            Assert.Contains(new Node(new Uri("d5584e9-mongodb-0.node.dc1.a9s-mongodb-consul")), connSettings.ConnectionPool.Nodes);
            Assert.Equal("a9s-brk-usr-e74b9538ae5dcf04500eb0fc18907338d4610f30", connSettings.BasicAuthenticationCredentials.Username);
            Assert.Equal("a9s-brk-usr-e74b9538ae5dcf04500eb0fc18907338d4610f30", connSettings.BasicAuthenticationCredentials.Password);
        }

        [Fact]
        public void AddElasticSearchConnection_AddsElasticSearchHealthContributor()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            // Act
            services.AddElasticSearch(config);
            var healthContributor = services.BuildServiceProvider().GetService<IHealthContributor>() as ElasticSearchHealthContributor;

            // Assert
            Assert.NotNull(healthContributor);
        }
    }
}
