using System;
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
	public sealed class ServletUtils
	{

		private ServletUtils()
		{

		}

		public static string sanitiseUri(string rUri)
		{
			string uri = rUri;
			if (uri.EndsWith("/", StringComparison.Ordinal))
			{
				uri = uri.Substring(0, uri.Length - 1);
			}
			return uri;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String getContent(java.io.InputStream is) throws java.io.IOException
		public static string getContent(System.IO.Stream @is)
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