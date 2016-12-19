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
namespace RestFixture.Net.Support
{

	using Credentials = org.apache.commons.httpclient.Credentials;
	using HostConfiguration = org.apache.commons.httpclient.HostConfiguration;
	using HttpClient = org.apache.commons.httpclient.HttpClient;
	using UsernamePasswordCredentials = org.apache.commons.httpclient.UsernamePasswordCredentials;
	using AuthScope = org.apache.commons.httpclient.auth.AuthScope;
	using HostParams = org.apache.commons.httpclient.@params.HostParams;
	using HttpClientParams = org.apache.commons.httpclient.@params.HttpClientParams;


	/// <summary>
	/// Helper builder class for an apache <seealso cref="HttpClient"/> that uses data in the
	/// <seealso cref="Config"/> to configure the object.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class HttpClientBuilder
	{
		/// <summary>
		/// default value of the socket timeout: 3000ms.
		/// </summary>
		public const int? DEFAULT_SO_TO = 3000;
		/// <summary>
		/// default value of the proxy port: 80.
		/// </summary>
		public const int? DEFAULT_PROXY_PORT = 80;

		/// <param name="config"> the <seealso cref="Config"/> containing the client configuration paramteres. </param>
		/// <returns> an instance of an <seealso cref="HttpClient"/>. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.httpclient.HttpClient createHttpClient(final Config config)
		public virtual HttpClient createHttpClient(Config config)
		{
			HttpClient client = createConfiguredClient(config);
			if (config != null)
			{
				configureHost(config, client);
				configureCredentials(config, client);
			}
			return client;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private org.apache.commons.httpclient.HttpClient createConfiguredClient(final Config config)
		private HttpClient createConfiguredClient(Config config)
		{
			HttpClientParams @params = new HttpClientParams();
			@params.SoTimeout = DEFAULT_SO_TO;
			if (config != null)
			{
				@params.SoTimeout = config.getAsInteger("http.client.connection.timeout", DEFAULT_SO_TO);
			}
			HttpClient client = new HttpClient(@params);
			return client;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void configureHost(final Config config, org.apache.commons.httpclient.HttpClient client)
		private void configureHost(Config config, HttpClient client)
		{
			HostConfiguration hostConfiguration = client.HostConfiguration;
			string proxyHost = config.get("http.proxy.host");
			if (!string.ReferenceEquals(proxyHost, null))
			{
				int proxyPort = config.getAsInteger("http.proxy.port", DEFAULT_PROXY_PORT);
				hostConfiguration.setProxy(proxyHost, proxyPort);
			}
			HostParams hostParams = new HostParams();
			hostConfiguration.Params = hostParams;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void configureCredentials(final Config config, org.apache.commons.httpclient.HttpClient client)
		private void configureCredentials(Config config, HttpClient client)
		{
			string username = config.get("http.basicauth.username");
			string password = config.get("http.basicauth.password");
			if (!string.ReferenceEquals(username, null) && !string.ReferenceEquals(password, null))
			{
				Credentials defaultcreds = new UsernamePasswordCredentials(username, password);
				client.State.setCredentials(AuthScope.ANY, defaultcreds);
			}
		}
	}

}