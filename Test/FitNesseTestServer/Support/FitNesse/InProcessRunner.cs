using System;
using System.Text;
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
namespace FitNesseTestServer.Support.FitNesse
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using ComponentFactory = fitnesse.ComponentFactory;
	using FitNesseContext = fitnesse.FitNesseContext;
	using Responder = fitnesse.Responder;
	using WikiPageFactory = fitnesse.WikiPageFactory;
	using Request = fitnesse.http.Request;
	using Response = fitnesse.http.Response;
	using ResponseSender = fitnesse.http.ResponseSender;
	using ResponderFactory = fitnesse.responders.ResponderFactory;
	using WikiPage = fitnesse.wiki.WikiPage;

	public class InProcessRunner
	{

		private static string SRC = "build/fitnesse";
		private static string SUITE_ROOT = "RestFixtureTests";
		private static string FITNESSE_ROOT_PAGE = "FitNesseRoot";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void main(String... args) throws Exception
		public static void Main(params string[] args)
		{
			ResponderFactory rFac = new ResponderFactory(SRC);
			ComponentFactory componentFactory = new ComponentFactory();
			WikiPageFactory pFac = new WikiPageFactory();
			WikiPage root = pFac.makeRootPage(SRC, FITNESSE_ROOT_PAGE, componentFactory);
			Request request = mock(typeof(Request));
			when(request.Resource).thenReturn(SUITE_ROOT);
			when(request.QueryString).thenReturn("suite&format=xml");
			verifyNoMoreInteractions(request);
			Responder responder = rFac.makeResponder(request, root);
			FitNesseContext context = new FitNesseContext(root);
			context.rootDirectoryName = FITNESSE_ROOT_PAGE;
			context.rootPath = SRC;
			context.doNotChunk = true;
			context.setRootPagePath();
			VelocityFactory.makeVelocityFactory(context);
			Response response = responder.makeResponse(context, request);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer sb = new StringBuffer();
			StringBuilder sb = new StringBuilder();
			ResponseSender sender = new ResponseSenderAnonymousInnerClass(sb);
			response.readyToSend(sender);
			Console.WriteLine();
			for (int i = 0; i < 20; i++)
			{
				// System.out.print(".");
				Console.Write(sb.ToString());
				Thread.Sleep(1000);
				if (sb.ToString().IndexOf("Exit-Code", StringComparison.Ordinal) > 0)
				{
					Console.WriteLine();
					break;
				}
			}

			Console.WriteLine(sb.ToString());
		}

		private class ResponseSenderAnonymousInnerClass : ResponseSender
		{
			private StringBuilder sb;

			public ResponseSenderAnonymousInnerClass(StringBuilder sb)
			{
				this.sb = sb;
			}


			public override void send(sbyte[] bytes)
			{
				sb.Append(StringHelperClass.NewString(bytes));
			}

			public override void close()
			{
			}

			public override Socket Socket
			{
				get
				{
					return null;
				}
			}

		}

	}

}