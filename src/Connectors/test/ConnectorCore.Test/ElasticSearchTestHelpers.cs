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
    public static class ElasticSearchTestHelpers
    {
        /// <summary>
        /// Sample VCAP_SERVICES entry for a9s ElasticSearch for PCF
        /// </summary>
        public static string SingleBinding_a9s_SingleServer_VCAP = @"
        {
            'a9s-elasticsearch5': [
                {
                    'credentials': {
                        'dns_servers': [
                        '172.28.10.32',
                        '172.28.11.11',
                        '172.28.12.23'
                            ],
                        'host_ips': [
                        '172.28.25.13'
                            ],
                        'host': [
                        'EXAMPLE_HOST'
                            ],
                        'password': 'EXAMPLE_USER',
                        'username': 'EXAMPLE_PASSWORD',
                        'scheme': 'http',
                        'port': 9200
                    },
                    'label': 'a9s-elasticsearch5',
                    'name': 'my-elasticsearch-service',
                    'plan': 'elasticsearch-cluster-small',
                    'tags': [
                    'searchengine'
                        ]
                }
                ]
        }";

        /// <summary>
        /// Sample VCAP_SERVICES entry for a9s ElasticSearch with replicas
        /// </summary>
        public static string SingleBinding_a9s_WithReplicas_VCAP = @"
{
            'a9s-elasticsearch5': [
                {
                    'credentials': {
                        'dns_servers': [
                        '172.28.10.32',
                        '172.28.11.11',
                        '172.28.12.23'
                            ],
                        'host_ips': [
                        '172.28.25.13',
                        '172.28.25.14'
                            ],
                        'host': [
                        'EXAMPLE_HOST',
                        'EXAMPLE_HOST_2'
                            ],
                        'password': 'EXAMPLE_USER',
                        'username': 'EXAMPLE_PASSWORD',
                        'scheme': 'http',
                        'port': 9200
                    },
                    'label': 'a9s-elasticsearch5',
                    'name': 'my-elasticsearch-service',
                    'plan': 'elasticsearch-cluster-small',
                    'tags': [
                    'searchengine'
                        ]
                }
                ]
        }";
    }
}
