using System;
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

	using NodeList = org.w3c.dom.NodeList;

	using RestResponse = smartrics.rest.client.RestResponse;

	/// <summary>
	/// Handles let expressions on XML content, returning XML string rather than the
	/// string with the content within the tags.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class LetBodyXmlHandler : ILetHandler
	{

		public virtual string handle(IRunnerVariablesProvider variablesProvider, Config config, RestResponse response, object expressionContext, string expression)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Map<String, String> namespaceContext = (java.util.Map<String, String>) expressionContext;
			IDictionary<string, string> namespaceContext = (IDictionary<string, string>) expressionContext;
			NodeList list = Tools.extractXPath(namespaceContext, expression, response.Body);
			string val = Tools.xPathResultToXmlString(list);
			int pos = val.IndexOf("?>", StringComparison.Ordinal);
			if (pos >= 0)
			{
				val = val.Substring(pos + 2);
			}
			return val;
		}

	}

}