using System;
using System.Threading;

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
	using Connector = org.mortbay.jetty.Connector;
	using Server = org.mortbay.jetty.Server;
	using SocketConnector = org.mortbay.jetty.bio.SocketConnector;
	using Context = org.mortbay.jetty.servlet.Context;
	using ServletHolder = org.mortbay.jetty.servlet.ServletHolder;

	/// <summary>
	/// Wrapper to jetty providing the sample application used by RestFixture CATs.
	/// 
	/// @author fabrizio
	/// 
	/// </summary>
	public class HttpServer
	{

		private static readonly Log LOG = LogFactory.getLog(typeof(HttpServer));
		private Server server = null;
		private int port;

		public HttpServer(int port)
		{
			BasicConfigurator.configure();
			server = new Server();
			server.StopAtShutdown = true;
			Port = port;
		}

		protected internal virtual Server Server
		{
			get
			{
				return server;
			}
		}

		public virtual string stop()
		{
			string ret = null;
			LOG.debug("Stopping jetty in port " + Port);
			try
			{
				Server.stop();
				// Wait for 3 seconds to stop
				int watchdog = 30;
				while (!Server.Stopped && watchdog > 0)
				{
					Thread.Sleep(100);
					watchdog--;
				}
				ret = "OK";
			}
			catch (Exception e)
			{
				ret = "Failed stopping http server: " + e.Message;
			}
			if (!Server.Stopped)
			{
				ret = "Failed stopping http server after 30 seconds wait";
			}
			return ret;
		}

		private int Port
		{
			set
			{
				LOG.debug("Adding socket connector to Jetty on port " + value);
				Connector connector = createHttpConnector(value);
				addConnector(value, connector);
				this.port = value;
			}
			get
			{
				return port;
			}
		}

		private void addConnector(int port, Connector connector)
		{
			bool found = false;
			Connector[] cArray = Server.Connectors;
			if (cArray != null)
			{
				foreach (Connector c in cArray)
				{
					if (c.Port == port)
					{
						found = true;
						break;
					}
				}
			}
			if (!found)
			{
				// Configure port.
				Server.addConnector(connector);
			}
		}

		private Connector createHttpConnector(int port)
		{
			Connector connector = new SocketConnector();
			connector.Port = port;
			return connector;
		}


		public virtual string start()
		{
			string ret = null;
			LOG.debug("Starting jetty in port " + Port);
			try
			{
				Server.start();
				// Wait for 3 seconds to start
				int watchdog = 30;
				while (!Server.Running && !Server.Started && watchdog > 0)
				{
					Thread.Sleep(100);
					watchdog--;
				}
				ret = "OK";
			}
			catch (Exception e)
			{
				ret = "Failed starting http server: " + e.Message;
				LOG.error(ret, e);
			}
			if (!Server.Running)
			{
				ret = "Failed to start http server after waiting 30 seconds";
			}
			return ret;
		}

		public virtual bool Started
		{
			get
			{
				return server.Started;
			}
		}

		public virtual bool Stopped
		{
			get
			{
				return server.Stopped;
			}
		}

		public virtual void join()
		{
			try
			{
				Server.join();
			}
			catch (InterruptedException e)
			{
				throw new System.InvalidOperationException("Interruption occurred!", e);
			}
		}

		public virtual void addServlet(HttpServlet servlet, string ctxRoot)
		{
			Context ctx = new Context(server, ctxRoot, Context.SESSIONS);
			ctx.addServlet(new ServletHolder(servlet), "/*");
		}

	}

}