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
namespace RestFixture.Net.Support.Http
{

	using URI = org.apache.commons.httpclient.URI;
	using URIException = org.apache.commons.httpclient.URIException;

	/// <summary>
	/// Head method, enhanced with support of query parameters.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class HeadMethod : org.apache.commons.httpclient.methods.HeadMethod
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public org.apache.commons.httpclient.URI getURI() throws org.apache.commons.httpclient.URIException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual URI URI
		{
			get
			{
				return URIBuilder.newURI(this, base.HostConfiguration);
			}
			set
			{
				base.URI = value;
			}
		}


	}

}