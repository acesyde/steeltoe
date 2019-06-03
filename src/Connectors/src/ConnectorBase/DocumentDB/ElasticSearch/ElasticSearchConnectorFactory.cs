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
using Steeltoe.CloudFoundry.Connector.Services;

namespace Steeltoe.CloudFoundry.Connector.ElasticSearch
{
    public class ElasticSearchConnectorFactory
    {
        private readonly ElasticSearchServiceInfo _info;
        private readonly ElasticSearchConnectorOptions _config;
        private readonly ElasticSearchProviderConfigurer _configurer = new ElasticSearchProviderConfigurer();

        public ElasticSearchConnectorFactory()
        {
        }

        public ElasticSearchConnectorFactory(ElasticSearchServiceInfo sinfo, ElasticSearchConnectorOptions config, Type type)
        {
            _info = sinfo;
            _config = config ?? throw new ArgumentNullException(nameof(config));
            ConnectorType = type;
        }

        protected Type ConnectorType { get; set; }

        public virtual object Create(IServiceProvider provider)
        {
            var connectionOptions = _configurer.Configure(_info, _config);
            object result = null;

            if (connectionOptions != null)
            {
                result = CreateConnection(connectionOptions.ToElasticSearchOptions());
            }

            if (result == null)
            {
                throw new ConnectorException($"Unable to create instance of '{ConnectorType}'");
            }

            return result;
        }

        public virtual object CreateConnection(object options)
        {
            return ConnectorHelpers.CreateInstance(ConnectorType, new object[] { options });
        }
    }
}
