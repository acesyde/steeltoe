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

namespace Steeltoe.CloudFoundry.Connector.ElasticSearch
{
    /// <summary>
    /// Assemblies and types used for interacting with ElasticSearch
    /// </summary>
    public static class ElasticSearchTypeLocator
    {
        /// <summary>
        /// List of supported ElasticSearch assemblies
        /// </summary>
        public static string[] Assemblies = new string[] { "Nest" };

        /// <summary>
        /// List of supported ElasticSearch client interface types
        /// </summary>
        public static string[] ConnectionInterfaceTypeNames = new string[] { "Nest.IElasticClient" };

        /// <summary>
        /// List of supported ElasticSearch client types
        /// </summary>
        public static string[] ConnectionTypeNames = new string[] { "Nest.ElasticClient" };

        /// <summary>
        /// Class for describing ElasticSearch connection information
        /// </summary>
        public static string[] ElasticSearchConnectionInfo = new string[] { "Nest.ConnectionSettings" };

        /// <summary>
        /// Gets IElasticClient from Nest Library
        /// </summary>
        /// <exception cref="ConnectorException">When type is not found</exception>
        public static Type IElasticClient => ConnectorHelpers.FindTypeOrThrow(Assemblies, ConnectionInterfaceTypeNames, "IElasticClient", "an NEST driver");

        /// <summary>
        /// Gets ElasticClient from Nest Library
        /// </summary>
        /// <exception cref="ConnectorException">When type is not found</exception>
        public static Type ElasticClient => ConnectorHelpers.FindTypeOrThrow(Assemblies, ConnectionTypeNames, "ElasticClient", "an NEST driver");

        /// <summary>
        /// Gets ElasticSearchUrl from Nest Library
        /// </summary>
        /// <exception cref="ConnectorException">When type is not found</exception>
        public static Type ElasticSearchUrl => ConnectorHelpers.FindTypeOrThrow(Assemblies, ElasticSearchConnectionInfo, "ElasticSearchUrl", "an NEST driver");

        /// <summary>
        /// Gets a method that lists databases available in a ElasticSearchClient
        /// </summary>
        public static MethodInfo ListDatabasesMethod => FindMethodOrThrow(ElasticClient, "ListDatabases", new Type[] { typeof(CancellationToken) });

        private static MethodInfo FindMethodOrThrow(Type type, string methodName, Type[] parameters = null)
        {
            var returnType = ConnectorHelpers.FindMethod(type, methodName, parameters);
            if (returnType == null)
            {
                throw new ConnectorException("Unable to find required ElasticSearch type or method, are you missing a NEST Nuget package?");
            }

            return returnType;
        }
    }
}
