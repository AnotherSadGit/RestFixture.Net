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


	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	using RestResponse = smartrics.rest.client.RestResponse;

	/// <summary>
	/// Handles body of the last response on behalf of LET in RestFixture.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class LetBodyHandler : ILetHandler
	{

		public LetBodyHandler()
		{
		}

		public virtual string handle(IRunnerVariablesProvider variablesProvider, Config config, RestResponse response, object expressionContext, string expression)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Map<String, String> namespaceContext = (java.util.Map<String, String>) expressionContext;
			IDictionary<string, string> namespaceContext = (IDictionary<string, string>) expressionContext;
			string contentTypeString = response.ContentType;
			string charset = response.Charset;
			ContentType contentType = ContentType.parse(contentTypeString);
			BodyTypeAdapter bodyTypeAdapter = (new BodyTypeAdapterFactory(variablesProvider, config)).getBodyTypeAdapter(contentType, charset);
			string body = bodyTypeAdapter.toXmlString(response.Body);
			if (string.ReferenceEquals(body, null))
			{
				return null;
			}
			string val = null;
			try
			{
				NodeList list = Tools.extractXPath(namespaceContext, expression, body);
				Node item = list.item(0);
				if (item != null)
				{
					val = item.TextContent;
				}
			}
			catch (System.ArgumentException)
			{
				// ignore - may be that it's evaluating to a string
				val = (string) Tools.extractXPath(namespaceContext, expression, body, XPathConstants.STRING, charset);
			}
			if (!string.ReferenceEquals(val, null))
			{
				val = val.Trim();
			}
			return val;
		}
	}

}