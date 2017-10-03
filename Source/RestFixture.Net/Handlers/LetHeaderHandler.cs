using System.Collections.Generic;
using System.Text.RegularExpressions;
using RestClient.Data;
using RestFixture.Net.Support;
using RestFixture.Net.Tools;

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
namespace RestFixture.Net.Handlers
{
	/// <summary>
	/// Handles header (a list of Header objects) LET manipulations.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class LetHeaderHandler : ILetHandler
	{

		public string handle(IRunnerVariablesProvider variablesProvider, Config config, 
            RestResponse response, object expressionContext, string expression)
		{
			IList<string> content = new List<string>();
			if (response != null)
			{
				foreach (RestData.Header e in response.Headers)
				{
					string @string = StringTools.convertEntryToString(e.Name, e.Value, ":");
					content.Add(@string);
				}
			}

			string value = null;
			if (content.Count > 0)
			{
				Regex regex = new Regex(expression);
				foreach (string c in content)
				{
					Match match = regex.Match(c);
				    if (match.Success)
				    {
				        int cc = match.Groups.Count;
				        value = match.Groups[cc - 1].Value;
				        break;
				    }
				}
			}
			return value;
		}

	}

}