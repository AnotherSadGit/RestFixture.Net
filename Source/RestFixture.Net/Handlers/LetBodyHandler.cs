using System.Collections.Generic;
using System.Xml.XPath;
using restFixture.Net.Support;
using restFixture.Net.TypeAdapters;
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
namespace restFixture.Net.Handlers
{
	/// <summary>
	/// Handles body of the last response on behalf of LET in RestFixture.
	/// </summary>
	public class LetBodyHandler : ILetHandler
	{

		public LetBodyHandler()
		{
		}

		public virtual string handle(IRunnerVariablesProvider variablesProvider, Config config, 
            RestResponse response, object expressionContext, string expression)
		{
			IDictionary<string, string> namespaceContext = (IDictionary<string, string>) expressionContext;
			string contentTypeString = response.ContentType;
			string charset = response.Charset;
			ContentType contentType = ContentType.parse(contentTypeString);
			BodyTypeAdapter bodyTypeAdapter = (new BodyTypeAdapterFactory(variablesProvider, config)).getBodyTypeAdapter(contentType, charset);
			string body = bodyTypeAdapter.toXmlString(response.Body);
			if (body == null)
			{
				return null;
			}
			string val = null;
			try
			{
                val = XmlTools.GetNodeValue(namespaceContext, expression, body);
			}
			catch (System.ArgumentException)
			{
				// ignore - may be that it's evaluating to a string
				val = (string)XmlTools.extractXPath(namespaceContext, expression, body,
                    XPathEvaluationReturnType.String);
			}
			if (val != null)
			{
				val = val.Trim();
			}
			return val;
		}
	}

}