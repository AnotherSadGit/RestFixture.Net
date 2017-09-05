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

using RestClient.Data;

namespace RestClient
{

	/// <summary>
	/// A Rest Client offers a simplified interface to an underlying implementation of an Http client.
	/// 
	/// A Rest Client is geared to operate of REST resources.
	/// </summary>
	public interface IRestClient
	{

		/// <summary>
		/// Gets or sets the base URL or host address.
		/// </summary>
        /// <remarks>This is the portion of the full Url not part of the
        /// resource type. For example if a resource type full Url is
        /// http://host:8888/domain/resourcetype and the resource type is
        /// /resourcetype, the base Url is http://host:8888/domain.
        /// It is meant to serve as a default value to be appended to compose the
        /// full Url when IRestClient.Execute(Data.RestRequest) is used.</remarks>
		string BaseUrlString {set;get;}

        /// <summary>
        /// Gets or sets the connection timeout value in milliseconds.
        /// </summary>
        /// <remarks>This is the time to set up the connection, not the time to set up the 
        /// connection and retrieve all the data from the server.</remarks>
        int ConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the read-write timeout value in milliseconds.
        /// </summary>
        /// <remarks>This is the time to retrieve data from the server, or send data to it, after 
        /// the connection has been established.</remarks>
        int ReadWriteTimeout { get; set; }

        /// <summary>
        /// Gets or sets information about a proxy server that must be used to connect to the 
        /// internet.
        /// </summary>
        ProxyInfo Proxy { get; set; }

        /// <summary>
        /// Gets or sets credentials needed to connect to a remote resource.
        /// </summary>
        Credentials Credentials { get; set; }

		/// <summary>
		/// Executes a rest request using the underlying Http client implementation.
		/// </summary>
		/// <param name="request">The request to be executed </param>
		/// <returns> the response of the rest request </returns>
        RestResponse Execute(Data.RestRequest requestDetails);

		/// <summary>
		/// Executes the rest request.
		/// 
		/// This method offers the possibility to override the base Url set on this client.
		/// </summary>
        /// <param name="baseAddress">The base Url.</param>
		/// <param name="request">The request to be executed </param>
		/// <returns> the response of the rest request.
		/// See <seealso cref="smartrics.rest.client.RestClient.BaseUrlString"/> </returns>
        RestResponse Execute(string baseAddress, RestRequest requestDetails);

	}
}