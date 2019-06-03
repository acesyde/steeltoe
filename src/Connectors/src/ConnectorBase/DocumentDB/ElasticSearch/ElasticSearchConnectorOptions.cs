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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector.Services;

namespace Steeltoe.CloudFoundry.Connector.ElasticSearch
{
    public class ElasticSearchConnectorOptions : AbstractServiceConnectorOptions
    {
        private const string SERVER = "localhost";
        private const int PORT = 9200;
        private const string ELASTICSEARCH_CLIENT_SECTION_PREFIX = "elasticsearch:client";
        private readonly bool _cloudFoundryConfigFound = false;

        public ElasticSearchConnectorOptions()
        {
        }

        public ElasticSearchConnectorOptions(IConfiguration config)
            : base('&', '=')
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var section = config.GetSection(ELASTICSEARCH_CLIENT_SECTION_PREFIX);
            section.Bind(this);

            Options = config
                .GetSection(ELASTICSEARCH_CLIENT_SECTION_PREFIX + ":options")
                .GetChildren()
                .ToDictionary(x => x.Key, x => x.Value);

            _cloudFoundryConfigFound = config.HasCloudFoundryServiceConfigurations();
        }

        public string ConnectionString { get; set; }

        public string Server { get; set; } = SERVER;

        public int Port { get; set; } = PORT;

        public string Username { get; set; }

        public string Password { get; set; }

        internal string Uri { get; set; }

        internal Dictionary<string, string> Options { get; set; }
        
        public bool Ssl { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(ConnectionString) && !_cloudFoundryConfigFound)
            {
                // Connection string was provided and VCAP_SERVICES wasn't found, just use the connectionstring
                return ConnectionString;
            }
            else if (Uri != null)
            {
                // VCAP_SERVICES provided a URI, the ElasticSearch driver can just use that
                return Uri;
            }
            else
            {
                // build a ElasticSearch connection string
                StringBuilder sb = new StringBuilder();

                sb.Append(Ssl
                    ? ElasticSearchServiceInfo.ELASTICSEARCH_SECURE_SCHEME
                    : ElasticSearchServiceInfo.ELASTICSEARCH_SCHEME);

                sb.Append("://");

                AddColonDelimitedPair(sb, Username, Password, '@');
                AddColonDelimitedPair(sb, Server, Port.ToString());

                if (Options != null && Options.Any())
                {
                    sb.Append("?");
                    foreach (var o in Options)
                    {
                        AddKeyValue(sb, o.Key, o.Value);
                    }
                }

                return sb.ToString().TrimEnd('&');
            }
        }
    }
}
