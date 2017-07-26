using System.Collections.Generic;
using NLog;
using RestClient.Data;


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
namespace restFixture.Net.Support
{
	/// <summary>
	/// Handles let expressions on XML content, returning XML string rather than the
	/// string with the content within the tags.
	/// </summary>
	public class LetBodyJsHandler : ILetHandler
	{
        private static NLog.Logger LOG = LogManager.GetCurrentClassLogger();

		public virtual string handle(IRunnerVariablesProvider variablesProvider, Config config, 
            RestResponse response, object expressionContext, string expression)
		{
			JavascriptWrapper js = new JavascriptWrapper(variablesProvider);
			IDictionary<string, string> urlMap = 
                config.getAsMap("restfixture.javascript.imports.map", 
                    new Dictionary<string, string>());
			object result = js.evaluateExpression(response, expression, urlMap);
			if (result == null)
			{
				return null;
			}
			return result.ToString();
		}


	}

}