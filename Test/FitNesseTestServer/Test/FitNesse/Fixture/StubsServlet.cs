using System;
using System.Collections.Generic;
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
namespace FitNesseTestServer.Test.FitNesse.Fixture
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static smartrics.rest.test.fitnesse.fixture.ServletUtils.sanitiseUri;



	using Header = org.apache.commons.httpclient.Header;
	using Log = org.apache.commons.logging.Log;
	using RestResponse = smartrics.rest.client.RestResponse;

	public class StubsServlet : HttpServlet
	{
		private const long serialVersionUID = 5557300437355123426L;
		private static readonly Log LOG = LogFactory.getLog(typeof(StubsServlet));
		public const string CONTEXT_ROOT = "/stubs";
		private static RestResponse nextResponse;

		public StubsServlet()
		{
		}

		/// <summary>
		/// starts with /responses, takes the rest and uses as uri
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void service(javax.servlet.http.HttpServletRequest req, javax.servlet.http.HttpServletResponse resp) throws javax.servlet.ServletException, java.io.IOException
		protected internal virtual void service(HttpServletRequest req, HttpServletResponse resp)
		{
			string method = req.Method;
			string uri = sanitiseUri(req.RequestURI);
			if (method.Equals("POST") && uri.EndsWith("/responses", StringComparison.Ordinal))
			{
				nextResponse = new RestResponse();
				System.IO.Stream @is = req.InputStream;
				string line = HttpParser.readLine(@is, Charset.defaultCharset().name());
				string[] incipit = line.Split(" ", true);
				nextResponse.StatusCode = Convert.ToInt32(incipit[0]);
				Header[] headers = HttpParser.parseHeaders(@is, Charset.defaultCharset().name());
				foreach (Header h in headers)
				{
					nextResponse.addHeader(h.Name, h.Value);
				}
				line = HttpParser.readLine(@is, Charset.defaultCharset().name());
				while (line.Trim().Length < 1)
				{
					line = HttpParser.readLine(@is, Charset.defaultCharset().name());
				}
				// check content length and decide how much body you need to parse
				IList<smartrics.rest.client.RestData.Header> cl = nextResponse.getHeader("Content-Length");
				int len = 0;
				if (cl.Count > 0)
				{
					len = Convert.ToInt32(cl[0].Value);
				}
				if (len > 0)
				{
					string content = getContent(req.InputStream);
					nextResponse.Body = line + "\n" + content;
				}
			}
			else
			{
				resp.Status = nextResponse.StatusCode;
				IList<smartrics.rest.client.RestData.Header> headers = nextResponse.Headers;
				foreach (smartrics.rest.client.RestData.Header h in headers)
				{
					resp.addHeader(h.Name, h.Value);
				}
				resp.OutputStream.write(nextResponse.Body.Bytes);
			}
		}

		private string sanitise(string rUri)
		{
			string uri = rUri;
			if (uri.EndsWith("/", StringComparison.Ordinal))
			{
				uri = uri.Substring(0, uri.Length - 1);
			}
			return uri;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String getContent(java.io.InputStream is) throws java.io.IOException
		private string getContent(System.IO.Stream @is)
		{
			StringBuilder sBuff = new StringBuilder();
			int c;
			while ((c = @is.Read()) != -1)
			{
				sBuff.Append((char) c);
			}
			string content = sBuff.ToString();
			return content;
		}

	}

}