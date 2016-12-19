using System.Collections.Generic;

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
namespace RestFixture.Net.Support
{


	/// <summary>
	/// Builds strategies to handle LET body.
	/// 
	/// <table>
	/// <caption>Supported strategies</caption>
	/// <tr>
	/// <td>{@code header}</td><td>applies the expression to the response headers</td>
	/// </tr>
	/// <tr>
	/// <td>{@code body}</td><td>applies the expression to the body</td>
	/// </tr>
	/// <tr>
	/// <td>{@code body:xml}</td><td>applies the expression to the body as XML. expressions are XPaths.</td>
	/// </tr>
	/// <tr>
	/// <td>{@code js}</td><td>applies expression to body as JSON</td>
	/// </tr>
	/// <tr>
	/// <td>{@code const}</td><td>it's actually  a shortcut to allow setting of const labels</td>
	/// </tr>
	/// </table>
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class LetHandlerFactory
	{
		private static IDictionary<string, LetHandler> strategies = new Dictionary<string, LetHandler>();

		static LetHandlerFactory()
		{
			strategies["header"] = new LetHeaderHandler();
			strategies["body"] = new LetBodyHandler();
			strategies["body:xml"] = new LetBodyXmlHandler();
			strategies["js"] = new LetBodyJsHandler();
			strategies["const"] = new LetBodyConstHandler();
		}

		private LetHandlerFactory()
		{

		}

		/// <param name="part"> the part to consider in the let expression </param>
		/// <returns> the handler for the given strategy. null if not found. </returns>
		public static LetHandler getHandlerFor(string part)
		{
			return strategies[part];
		}
	}

}