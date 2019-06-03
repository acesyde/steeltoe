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

using System.Net;
using Steeltoe.CloudFoundry.Connector.Services;

namespace Steeltoe.CloudFoundry.Connector.ElasticSearch
{
    public class ElasticSearchProviderConfigurer
    {
        public string Configure(ElasticSearchServiceInfo si, ElasticSearchConnectorOptions configuration)
        {
            UpdateConfiguration(si, configuration);
            return configuration.ToString();
        }

        public void UpdateConfiguration(ElasticSearchServiceInfo si, ElasticSearchConnectorOptions configuration)
        {
            if (si == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(si.Uri))
            {
                configuration.Uri = si.Uri;

                // the rest of this is unlikely to really be needed when a uri is available
                // but the properties are here, so let's go ahead and set them just in case
                configuration.Port = si.Port;
                if (configuration.UrlEncodedCredentials)
                {
                    configuration.Username = WebUtility.UrlDecode(si.UserName);
                    configuration.Password = WebUtility.UrlDecode(si.Password);
                }
                else
                {
                    configuration.Username = si.UserName;
                    configuration.Password = si.Password;
                }

                if (si.Scheme == ElasticSearchServiceInfo.ELASTICSEARCH_SECURE_SCHEME)
                {
                    configuration.Ssl = true;
                }

                configuration.Server = si.Host;
            }
        }
    }
}
