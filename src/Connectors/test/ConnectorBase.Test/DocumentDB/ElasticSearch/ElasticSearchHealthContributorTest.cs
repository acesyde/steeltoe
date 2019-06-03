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

namespace Steeltoe.CloudFoundry.Connector.ElasticSearch.Test
{
    using System;
    using System.Collections.Generic;
    using Common.HealthChecks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Services;
    using Xunit;

    public class ElasticSearchHealthContributorTest
    {
        private readonly Type _implementationType = ElasticSearchTypeLocator.ElasticClient;

        [Fact]
        public void GetElasticSearchContributor_ReturnsContributor()
        {
            var appsettings = new Dictionary<string, string>
            {
                ["elasticsearch:client:server"] = "localhost",
                ["elasticsearch:client:port"] = "9200",
            };

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(appsettings);
            var config = configurationBuilder.Build();
            var contrib = ElasticSearchHealthContributor.GetElasticSearchHealthContributor(config);
            Assert.NotNull(contrib);
            var status = contrib.Health();
            Assert.Equal(HealthStatus.DOWN, status.Status);
        }

        [Fact]
        public void Not_Connected_Returns_Down_Status()
        {
            // arrange
            ElasticSearchConnectorOptions config = new ElasticSearchConnectorOptions();
            var info = new ElasticSearchServiceInfo("MyId", "http://localhost:9200");
            var logrFactory = new LoggerFactory();
            var connFactory = new ElasticSearchConnectorFactory(info, config, _implementationType);
            var h = new ElasticSearchHealthContributor(connFactory, logrFactory.CreateLogger<ElasticSearchHealthContributor>());

            // act
            var status = h.Health();

            // assert
            Assert.Equal(HealthStatus.DOWN, status.Status);
            Assert.Equal("Failed to open ElasticSearch connection!", status.Description);
        }

        [Fact(Skip = "Integration test - Requires local ElasticSearch server")]
        public void Is_Connected_Returns_Up_Status()
        {
            // arrange
            var config = new ElasticSearchConnectorOptions();
            var info = new ElasticSearchServiceInfo("MyId", "http://localhost:9200");
            var logrFactory = new LoggerFactory();
            var connFactory = new ElasticSearchConnectorFactory(info, config, _implementationType);
            var h = new ElasticSearchHealthContributor(connFactory, logrFactory.CreateLogger<ElasticSearchHealthContributor>());

            // act
            var status = h.Health();

            // assert
            Assert.Equal(HealthStatus.UP, status.Status);
        }
    }
}