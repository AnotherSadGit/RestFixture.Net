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
namespace RestClient
{

	/// <summary>
	/// A Rest Client offers a simplified interface to an underlying implementation of an Http client.
	/// 
	/// A Rest Client is geared to operate of REST resources.
	/// </summary>
	public interface RestClient
	{

		/// <summary>
		/// Sets the base URL.
		/// It is the portion of the full Url not part of the
		/// resource type. For example if a resource type full Url is
		/// {@code http://host:8888/domain/resourcetype} and the resource type is
		/// {@code /resourcetype}, the base Url is {@code http://host:8888/domain}.
		/// It is meant to serve as a default value to be appended to compose the
		/// full Url when
		/// <seealso cref="smartrics.rest.client.RestClient#execute(RestRequest)"/>
		/// is used.
		/// </summary>
		/// <param name="bUrl">
		///            a string with the base Url.
		/// See <seealso cref="smartrics.rest.client.RestClient#execute(RestRequest)"/> </param>
		string BaseUrl {set;get;}


		/// <summary>
		/// Executes a rest request using the underlying Http client implementation.
		/// </summary>
		/// <param name="request">
		///            the request to be executed </param>
		/// <returns> the response of the rest request </returns>
		RestResponse execute(RestRequest request);

		/// <summary>
		/// Executes the rest request.
		/// 
		/// This method offers the possibility to override the base Url set on this client.
		/// </summary>
		/// <param name="baseUrl">
		///            the base Url </param>
		/// <param name="request">
		///            the request to be executed </param>
		/// <returns> the response of the rest request.
		/// See <seealso cref="smartrics.rest.client.RestClient#setBaseUrl(java.lang.String)"/> </returns>
		RestResponse execute(string baseUrl, RestRequest request);

	}
}