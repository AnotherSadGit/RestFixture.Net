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


	using Header = smartrics.rest.client.RestData.Header;
	using RestResponse = smartrics.rest.client.RestResponse;

	/// <summary>
	/// Handles header (a list of Header objects) LET manipulations.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class LetHeaderHandler : LetHandler
	{

		public override string handle(RunnerVariablesProvider variablesProvider, Config config, RestResponse response, object expressionContext, string expression)
		{
			IList<string> content = new List<string>();
			if (response != null)
			{
				foreach (Header e in response.Headers)
				{
					string @string = Tools.convertEntryToString(e.Name, e.Value, ":");
					content.Add(@string);
				}
			}

			string value = null;
			if (content.Count > 0)
			{
				Pattern p = Pattern.compile(expression);
				foreach (string c in content)
				{
					Matcher m = p.matcher(c);
					if (m.find())
					{
						int cc = m.groupCount();
						value = m.group(cc);
						break;
					}
				}
			}
			return value;
		}

	}

}