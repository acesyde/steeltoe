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
    using Xunit;

    public class ElasticSearchTypeLocatorTest
    {
        [Fact]
        public void Property_Can_Locate_ConnectionTypes()
        {
            // arrange -- handled by including a compatible NEST NuGet package

            // act
            var interfaceType = ElasticSearchTypeLocator.IElasticClient;
            var implementationType = ElasticSearchTypeLocator.ElasticClient;
            var elasticSearchUrl = ElasticSearchTypeLocator.ElasticSearchConnectionSettings;

            // assert
            Assert.NotNull(interfaceType);
            Assert.NotNull(implementationType);
            Assert.NotNull(elasticSearchUrl);
        }

        [Fact]
        public void Throws_When_ConnectionType_NotFound()
        {
            // arrange
            var types = ElasticSearchTypeLocator.ConnectionInterfaceTypeNames;
            ElasticSearchTypeLocator.ConnectionInterfaceTypeNames = new string[] { "something-Wrong" };

            // act
            var exception = Assert.Throws<ConnectorException>(() => ElasticSearchTypeLocator.IElasticClient);

            // assert
            Assert.Equal("Unable to find IElasticClient, are you missing an NEST driver?", exception.Message);

            // reset
            ElasticSearchTypeLocator.ConnectionInterfaceTypeNames = types;
        }
    }
}