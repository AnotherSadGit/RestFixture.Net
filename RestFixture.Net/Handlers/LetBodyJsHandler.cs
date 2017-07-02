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

	using Logger = org.slf4j.Logger;
	using RestResponse = smartrics.rest.client.RestResponse;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.slf4j.LoggerFactory.getLogger;

	/// <summary>
	/// Handles let expressions on XML content, returning XML string rather than the
	/// string with the content within the tags.
	/// 
	/// @author smartrics
	/// </summary>
	public class LetBodyJsHandler : ILetHandler
	{

		private static readonly Logger LOG = getLogger(typeof(LetBodyJsHandler));

		public virtual string handle(IRunnerVariablesProvider variablesProvider, Config config, RestResponse response, object expressionContext, string expression)
		{
			JavascriptWrapper js = new JavascriptWrapper(variablesProvider);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, String> urlMap = config.getAsMap("restfixture.javascript.imports.map", new java.util.HashMap<String, String>());
			IDictionary<string, string> urlMap = config.getAsMap("restfixture.javascript.imports.map", new Dictionary<string, string>());
			object result = js.evaluateExpression(response, expression, urlMap);
			if (result == null)
			{
				return null;
			}
			return result.ToString();
		}


	}

}