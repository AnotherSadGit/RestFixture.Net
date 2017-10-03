using RestClient;
using RestClient.Data;
using RestFixture.Net.Support;
using RestFixture.Net.TableElements;
using RestFixture.Net.TypeAdapters;

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
	/// <summary>
	/// Factory of all dependencies the rest fixture needs.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class PartsFactory
	{

		private readonly BodyTypeAdapterFactory bodyTypeAdapterFactory;

        public PartsFactory(IRunnerVariablesProvider variablesProvider, Support.Config config)
		{
			this.bodyTypeAdapterFactory = new BodyTypeAdapterFactory(variablesProvider, config);
		}

		/// <summary>
		/// Builds a rest client configured with the given config implementation.
		/// </summary>
		/// <param name="config">
		///            the configuration for the rest client to build </param>
		/// <returns> the rest client </returns>
		public virtual IRestClient buildRestClient(Config config)
		{
            IRestClient client = (new RestClientBuilder()).createRestClient(config);
            return client;
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
        public virtual ICellFormatter<T> buildCellFormatter<T>(Runner runner)
		{
			if (Runner.SLIM.Equals(runner))
			{
                return new SlimFormatter() as ICellFormatter<T>;
			}
			if (Runner.FIT.Equals(runner))
			{
                return new FitFormatter() as ICellFormatter<T>;
			}

		    string errorMessage = string.Format("Runner {0} not supported", runner);
            throw new System.InvalidOperationException(errorMessage);
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