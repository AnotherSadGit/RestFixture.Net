using System.Text;

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

	using HostConfiguration = org.apache.commons.httpclient.HostConfiguration;
	using HttpHost = org.apache.commons.httpclient.HttpHost;
	using HttpMethod = org.apache.commons.httpclient.HttpMethod;
	using HttpURL = org.apache.commons.httpclient.HttpURL;
	using URI = org.apache.commons.httpclient.URI;
	using URIException = org.apache.commons.httpclient.URIException;
	using HttpMethodParams = org.apache.commons.httpclient.@params.HttpMethodParams;

	/// <summary>
	/// Builds URIs with query strings.
	/// 
	/// @author 702161900
	/// 
	/// </summary>
	internal class URIBuilder
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.httpclient.URI getURI(String scheme, String host, int port, String path, String queryString, org.apache.commons.httpclient.params.HttpMethodParams params) throws org.apache.commons.httpclient.URIException
		public virtual URI getURI(string scheme, string host, int port, string path, string queryString, HttpMethodParams @params)
		{
			HttpHost httphost = new HttpHost(host, port);
			StringBuilder buffer = new StringBuilder();
			if (httphost != null)
			{
				buffer.Append(httphost.Protocol.Scheme);
				buffer.Append("://");
				buffer.Append(httphost.HostName);
				int p = httphost.Port;
				if (p != -1 && p != httphost.Protocol.DefaultPort)
				{
					buffer.Append(":");
					buffer.Append(p);
				}
			}
			buffer.Append(path);
			if (queryString != null)
			{
				buffer.Append('?');
				buffer.Append(queryString);
			}
			string charset = @params.UriCharset;
			return new HttpURL(buffer.ToString(), charset);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void setURI(org.apache.commons.httpclient.HttpMethodBase m, org.apache.commons.httpclient.URI uri) throws org.apache.commons.httpclient.URIException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void setURI(org.apache.commons.httpclient.HttpMethodBase m, URI uri)
		{
			HostConfiguration conf = m.HostConfiguration;
			if (uri.AbsoluteURI)
			{
				conf.Host = new HttpHost(uri);
				m.HostConfiguration = conf;
			}
			m.Path = uri.Path != null ? uri.EscapedPath : "/";
			m.QueryString = uri.Query;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.apache.commons.httpclient.URI newURI(org.apache.commons.httpclient.HttpMethod m, org.apache.commons.httpclient.HostConfiguration conf) throws org.apache.commons.httpclient.URIException
		public static URI newURI(HttpMethod m, HostConfiguration conf)
		{
			string scheme = conf.Protocol.Scheme;
			string host = conf.Host;
			int port = conf.Port;
			return (new URIBuilder()).getURI(scheme, host, port, m.Path, m.QueryString, m.Params);
		}
	}

}