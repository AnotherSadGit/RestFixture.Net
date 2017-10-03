/*  Copyright 2017 Simon Elms
 *
  *  This file is part of RestFixture.Net, a .NET port of the original Java 
 *  RestFixture written by Fabrizio Cannizzo and others.
 *
 *  RestFixture.Net is free software:
 *  You can redistribute it and/or modify it under the terms of the
 *  GNU Lesser General Public License as published by the Free Software Foundation,
 *  either version 3 of the License, or (at your option) any later version.
 *
 *  RestFixture.Net is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with RestFixture.Net.  If not, see <http://www.gnu.org/licenses/>.
 */

using RestClient;

namespace RestFixture.Net.Support
{

	/// <summary>
	/// Helper builder class for a <seealso cref="RestClient"/> that uses data in the
	/// <seealso cref="Config"/> to configure the object.
	/// </summary>
	public class RestClientBuilder
	{
		/// <summary>
		/// default value of the read-write timeout: 3000ms.
		/// </summary>
		public const int DEFAULT_READWRITE_TIMEOUT = 3000;
		/// <summary>
		/// default value of the proxy port: 80.
		/// </summary>
		public const int DEFAULT_PROXY_PORT = 80;

		/// <param name="config"> the <seealso cref="Config"/> containing the client configuration paramters.</param>
		/// <returns> an instance of an <seealso cref="RestClient"/>.</returns>
		public virtual IRestClient createRestClient(Config config)
		{
            IRestClient client = createConfiguredClient(config);
			if (config != null)
			{
				configureProxy(config, client);
				configureCredentials(config, client);
			}
			return client;
		}

        private IRestClient createConfiguredClient(Config config)
		{
            IRestClient client = new RestClientImpl();
            client.ReadWriteTimeout = DEFAULT_READWRITE_TIMEOUT;
			if (config != null)
			{
                client.ReadWriteTimeout 
                    = config.getAsInteger("http.client.connection.timeout", DEFAULT_READWRITE_TIMEOUT);
			}
            return client;
		}

        private void configureProxy(Config config, IRestClient client)
		{
			string proxyHost = config.get("http.proxy.host");
			if (proxyHost != null)
			{
				int proxyPort = config.getAsInteger("http.proxy.port", DEFAULT_PROXY_PORT);
                string proxyUserName = config.get("http.proxy.username");
                string proxyPassword = config.get("http.proxy.password");
                string proxyDomain = config.get("http.proxy.domain");
                ProxyInfo proxyInfo 
                    = new ProxyInfo(proxyHost, proxyPort, proxyUserName, proxyPassword, proxyDomain);
			    client.Proxy = proxyInfo;
			}
		}

        private void configureCredentials(Config config, IRestClient client)
		{
			string username = config.get("http.basicauth.username");
			string password = config.get("http.basicauth.password");
			if (username != null && password != null)
			{
                Credentials defaultCredentials = new Credentials(username, password);
			    client.Credentials = defaultCredentials;
			}
		}
	}

}