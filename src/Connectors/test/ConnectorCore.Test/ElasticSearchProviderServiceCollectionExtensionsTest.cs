using System;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Nest;
using Steeltoe.CloudFoundry.Connector.Test;
using Steeltoe.Common.HealthChecks;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.ElasticSearch.Test
{
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
            var service = services.BuildServiceProvider().GetService<MongoClient>();

            // Assert
            Assert.NotNull(service);
            var connSettings = service.Settings;
            Assert.Single((IEnumerable) connSettings.Servers);
            Assert.Equal("d8790b7-mongodb-0.node.dc1.a9s-mongodb-consul", connSettings.Server.Host);
            Assert.Equal(27017, connSettings.Server.Port);
            Assert.Equal("a9s-brk-usr-377ad48194cbf0452338737d7f6aa3fb6cdabc24", connSettings.Credential.Username);
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
            //Assert.Contains(new MongoServerAddress("d5584e9-mongodb-0.node.dc1.a9s-mongodb-consul", 27017), connSettings.Servers);
            //Assert.Contains(new MongoServerAddress("d5584e9-mongodb-1.node.dc1.a9s-mongodb-consul", 27017), connSettings.Servers);
            //Assert.Contains(new MongoServerAddress("d5584e9-mongodb-2.node.dc1.a9s-mongodb-consul", 27017), connSettings.Servers);
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
