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
namespace RestFixture.Net
{

	using HttpClient = org.apache.commons.httpclient.HttpClient;
	using HttpURL = org.apache.commons.httpclient.HttpURL;
	using URI = org.apache.commons.httpclient.URI;
	using URIException = org.apache.commons.httpclient.URIException;

	using RestClient = smartrics.rest.client.RestClient;
	using RestClientImpl = smartrics.rest.client.RestClientImpl;
	using RestRequest = smartrics.rest.client.RestRequest;
	using Runner = smartrics.rest.fitnesse.fixture.RestFixture.Runner;
	using BodyTypeAdapter = smartrics.rest.fitnesse.fixture.support.BodyTypeAdapter;
	using BodyTypeAdapterFactory = smartrics.rest.fitnesse.fixture.support.BodyTypeAdapterFactory;
	using CellFormatter = smartrics.rest.fitnesse.fixture.support.CellFormatter;
	using Config = smartrics.rest.fitnesse.fixture.support.Config;
	using ContentType = smartrics.rest.fitnesse.fixture.support.ContentType;
	using HttpClientBuilder = smartrics.rest.fitnesse.fixture.support.HttpClientBuilder;

	/// <summary>
	/// Factory of all dependencies the rest fixture needs.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class PartsFactory
	{

		private readonly BodyTypeAdapterFactory bodyTypeAdapterFactory;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public PartsFactory(final RunnerVariablesProvider variablesProvider, smartrics.rest.fitnesse.fixture.support.Config config)
		public PartsFactory(RunnerVariablesProvider variablesProvider, Config config)
		{
			this.bodyTypeAdapterFactory = new BodyTypeAdapterFactory(variablesProvider, config);
		}

		/// <summary>
		/// Builds a rest client configured with the given config implementation.
		/// </summary>
		/// <param name="config">
		///            the configuration for the rest client to build </param>
		/// <returns> the rest client </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public smartrics.rest.client.RestClient buildRestClient(final smartrics.rest.fitnesse.fixture.support.Config config)
		public virtual RestClient buildRestClient(Config config)
		{
			HttpClient httpClient = (new HttpClientBuilder()).createHttpClient(config);
			return new RestClientImplAnonymousInnerClass(this, httpClient, config);
		}

		private class RestClientImplAnonymousInnerClass : RestClientImpl
		{
			private readonly PartsFactory outerInstance;

			private Config config;

			public RestClientImplAnonymousInnerClass(PartsFactory outerInstance, HttpClient httpClient, Config config) : base(httpClient)
			{
				this.outerInstance = outerInstance;
				this.config = config;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected org.apache.commons.httpclient.URI createUri(String uriString, boolean escaped) throws org.apache.commons.httpclient.URIException
			protected internal override URI createUri(string uriString, bool escaped)
			{
				bool useNewHttpUriFactory = config.getAsBoolean("http.client.use.new.http.uri.factory", false);
				if (useNewHttpUriFactory)
				{
					return new HttpURL(uriString);
				}
				return base.createUri(uriString, escaped);
			}

			public override string getMethodClassnameFromMethodName(string mName)
			{
				bool useOverriddenHttpMethodImpl = config.getAsBoolean("http.client.use.new.http.uri.factory", false);
				if (useOverriddenHttpMethodImpl)
				{
					return string.Format("smartrics.rest.fitnesse.fixture.support.http.{0}Method", mName);
				}
				return base.getMethodClassnameFromMethodName(mName);
			}
		}

		/// <summary>
		/// Builds a empty rest request.
		/// </summary>
		/// <returns> the rest request. </returns>
		public virtual RestRequest buildRestRequest()
		{
			return new RestRequest();
		}

		/// <summary>
		/// Builds the appropriate formatter for a type of runner on this
		/// RestFixture.
		/// </summary>
		/// <param name="runner">
		///            the runner used to execute this RestFixture </param>
		/// <returns> a formatter instance of CellFormatter </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public smartrics.rest.fitnesse.fixture.support.CellFormatter<?> buildCellFormatter(smartrics.rest.fitnesse.fixture.RestFixture.Runner runner)
		public virtual CellFormatter<object> buildCellFormatter(Runner runner)
		{
			if (runner == null)
			{
				throw new System.ArgumentException("Runner is null");
			}
			if (Runner.SLIM.Equals(runner))
			{
				return new SlimFormatter();
			}
			if (Runner.FIT.Equals(runner))
			{
				return new FitFormatter();
			}
			throw new System.InvalidOperationException("Runner " + runner.name() + " not supported");
		}

		/// <summary>
		/// returns a <seealso cref="smartrics.rest.fitnesse.fixture.support.BodyTypeAdapter"/>
		/// for the content type in input.
		/// </summary>
		/// <param name="ct">
		///            the content type </param>
		/// <param name="charset">
		///            the charset the body is encoded as </param>
		/// <returns> the
		///         <seealso cref="smartrics.rest.fitnesse.fixture.support.BodyTypeAdapter"/> </returns>
		public virtual BodyTypeAdapter buildBodyTypeAdapter(ContentType ct, string charset)
		{
			return bodyTypeAdapterFactory.getBodyTypeAdapter(ct, charset);
		}
	}

}