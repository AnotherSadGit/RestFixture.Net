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

using NLog;

namespace RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture
{
	/// <summary>
	/// Fixture to manage the HTTP server required to support RestFixture CATs.
	/// 
	/// @author fabrizio
	/// 
	/// </summary>
	public class HttpServerFixture : fit.Fixture
	{
        private static Logger LOG = LogManager.GetCurrentClassLogger();

		private int _port;
		private static HttpServer _server = null;

		public virtual void startServer(string port)
		{
			startServer(int.Parse(port));
		}

		private void startServer(int port)
		{
			if (_server == null)
			{
				this._port = port;
				LOG.Info("Starting server on port {0}", port);
			    _server = new HttpServer(port);
				_server.Start();
			}
			else
			{
				LOG.Info("Server already started on port ", port);
			}
		}

		public virtual bool Started
		{
			get
			{
				return _server != null && _server.Started;
			}
		}

		public virtual bool Stopped
		{
			get
			{
				return _server != null && _server.Stopped;
			}
		}

		public virtual void stopServer()
		{
			if (_server != null)
			{
				LOG.Info("Stopping server on port " + _port);
				_server.Stop();
			}
			else
			{
				if (_port == 0)
				{
					LOG.Info("Server never started");
				}
				else
				{
					LOG.Info("Server already stopped on port " + _port);
				}
			}
		}

		public virtual bool resetResourcesDatabase()
		{
			Resources.Instance.reset();
			return true;
		}

		public static void Main(string[] args)
		{
			HttpServerFixture f = new HttpServerFixture();
			f.startServer(8765);
		}

	}

}