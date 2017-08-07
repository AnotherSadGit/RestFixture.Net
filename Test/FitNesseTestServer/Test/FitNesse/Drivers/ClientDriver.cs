using System;

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
namespace FitNesseTestServer.Test.FitNesse.Drivers
{

	using HttpClient = org.apache.commons.httpclient.HttpClient;

	using RestClient = smartrics.rest.client.RestClient;
	using RestClientImpl = smartrics.rest.client.RestClientImpl;
	using RestRequest = smartrics.rest.client.RestRequest;
	using RestResponse = smartrics.rest.client.RestResponse;

	public class ClientDriver
	{

		public static void Main(string[] args)
		{
			postForm(args);
		}

		public static void postForm(string[] args)
		{
			RestClient c = new RestClientImpl(new HttpClient());
			RestRequest req = new RestRequest();
			req.Body = "name=n&data=d1";
			req.Resource = "/resources/";
			req.Method = RestRequest.Method.Post;
			req.addHeader("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
			RestResponse res = c.execute("http://localhost:8765", req);
			Console.WriteLine("=======>\n" + res + "\n<=======");
		}

		public static void postXml(string[] args)
		{
			RestClient c = new RestClientImpl(new HttpClient());
			RestRequest req = new RestRequest();

			req.Body = "<resource><name>n</name><data>d1</data></resource>";
			req.Resource = "/resources/";
			req.Method = RestRequest.Method.Post;
			RestResponse res = c.execute("http://localhost:8765", req);
			Console.WriteLine("=======>\n" + res + "\n<=======");

			string loc = res.getHeader("Location").get(0).Value;
			req.Resource = loc + ".json";
			req.Method = RestRequest.Method.Get;
			res = c.execute("http://localhost:8765", req);
			Console.WriteLine("=======>\n" + res + "\n<=======");

			req.Method = RestRequest.Method.Put;
			req.Body = "<resource><name>another name</name><data>another data</data></resource>";
			res = c.execute("http://localhost:8765", req);
			Console.WriteLine("=======>\n" + res + "\n<=======");

			req.Resource = "/resources/";
			req.Method = RestRequest.Method.Get;
			res = c.execute("http://localhost:8765", req);
			Console.WriteLine("=======>\n" + res + "\n<=======");

			req.Method = RestRequest.Method.Delete;
			req.Resource = loc;
			res = c.execute("http://localhost:8765", req);
			Console.WriteLine("=======>\n" + res + "\n<=======");
		}

	}

}