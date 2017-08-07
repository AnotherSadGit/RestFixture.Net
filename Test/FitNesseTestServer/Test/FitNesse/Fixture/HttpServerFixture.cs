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

	using Log = org.apache.commons.logging.Log;
	using ActionFixture = fit.ActionFixture;

	/// <summary>
	/// Fixture to manage the HTTP server required to support RestFixture CATs.
	/// 
	/// @author fabrizio
	/// 
	/// </summary>
	public class HttpServerFixture : ActionFixture
	{

		private static readonly Log LOG = LogFactory.getLog(typeof(HttpServerFixture));
		private int port;
		private static HttpServer server;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HttpServerFixture() throws Exception
		public HttpServerFixture() : base()
		{
		}

		public virtual void startServer(string port)
		{
			startServer(int.Parse(port));
		}

		private void startServer(int port)
		{
			if (server == null)
			{
				this.port = port;
				LOG.info("Starting server on port " + port);
				server = new HttpServer(port);
				server.addServlet(new ResourcesServlet(), "/");
				server.start();
			}
			else
			{
				LOG.info("Server already started on port " + port);
			}
		}

		public virtual bool Started
		{
			get
			{
				return server != null && server.Started;
			}
		}

		public virtual bool Stopped
		{
			get
			{
				return server != null && server.Stopped;
			}
		}

		public virtual void stopServer()
		{
			if (server != null)
			{
				LOG.info("Stopping server on port " + port);
				server.stop();
			}
			else
			{
				if (port == 0)
				{
					LOG.info("Server never started");
				}
				else
				{
					LOG.info("Server already stopped on port " + port);
				}
			}
		}

		public virtual void join()
		{
			server.join();
		}

		public virtual bool resetResourcesDatabase()
		{
			Resources.Instance.reset();
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void main(String[] args) throws Exception
		public static void Main(string[] args)
		{
			HttpServerFixture f = new HttpServerFixture();
			f.startServer(8765);
			f.join();
		}

	}

}