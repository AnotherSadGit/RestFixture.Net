using System;
using System.Collections.Generic;
using System.Xml.XPath;
using restFixture.Net.Support;

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
namespace restFixture.Net.TypeAdapters
{
	/// <summary>
	/// Type adapter for body cells with XML content.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class XPathBodyTypeAdapter : BodyTypeAdapter
	{

		/// <summary>
		/// Equality check for bodies.
		/// 
		/// Expected body is a {@code List<String>} of XPaths - as parsed by
		/// <seealso cref="XPathBodyTypeAdapter#parse(String )"/> - to be executed in the
		/// actual body. The check is true if all XPaths executed in the actual body
		/// return a node list not null or empty.
		/// 
		/// A special case is dedicated to {@code no-body}. If the expected body is
		/// {@code no-body}, the equality check is true if the actual body returned
		/// by the REST response is empty or null.
		/// </summary>
		/// <param name="expected">
		///            the expected body, it's a string with XPaths separated by
		///            {@code System.getProperty("line.separator")} </param>
		/// <param name="actual">
		///            the body of the REST response returned by the call in the
		///            current test row </param>
		/// <seealso cref= fit.TypeAdapter </seealso>
		public override bool Equals(object expected, object actual)
		{
			if (checkNoBody(expected))
			{
				return checkNoBody(actual);
			}
			if (checkNoBody(actual))
			{
				return checkNoBody(expected);
			}
			// r2 is the actual. it needs to be parsed as XML and the XPaths in r1
			// must be verified
		    string actualText = actual.ToString();
			IList<string> expressions = (IList<string>) expected;
			foreach (string expr in expressions)
			{
				try
				{
                    bool b = eval(expr, actualText);
					if (!b)
					{
						addError("not found: '" + expr + "'");
					}
				}
				catch (Exception e)
				{
                    throw new System.ArgumentException("Cannot evaluate '" + expr + "' in " + actualText, e);
				}
			}

			return Errors.Count == 0;
		}

		protected internal virtual bool eval(string expr, string content)
		{
			bool? b;
			try
			{
				XPathNodeIterator ret = XmlTools.extractXPath(Context, expr, content);
                return !(ret == null || ret.Count == 0);
			}
			catch (System.ArgumentException)
			{
				// may be evaluated as BOOLEAN
                b = (bool?)XmlTools.extractXPath(Context, expr, content,
                    XPathEvaluationReturnType.Boolean);
			}
			return b.Value;
		}

		/// <summary>
		/// Parses the expected body in the current test.
		/// <para>
		/// A body is a String containing XPaths one for each new line. Empty body
		/// would result in an empty {@code List<String>}. A body containing the
		/// value {@code no-body} is especially treated separately.
		/// 
		/// </para>
		/// </summary>
		/// <param name="expectedListOfXpathsAsString"> expected list of xpaths as string </param>
		public override object parse(string expectedListOfXpathsAsString)
		{
			// expected values are parsed as a list of XPath expressions
			IList<string> expectedXPathAsList = new List<string>();
			if (expectedListOfXpathsAsString == null)
			{
				return expectedXPathAsList;
			}
			string expStr = expectedListOfXpathsAsString.Trim();
			if ("no-body".Equals(expStr))
			{
				return expectedXPathAsList;
			}
			if ("".Equals(expStr))
			{
				return expectedXPathAsList;
			}
			expStr = Tools.fromHtml(expStr);
            string[] nvpArray = expStr.Split(new string[] {"\r", "\n", "\r\n"}, 
                StringSplitOptions.None);
			foreach (string nvp in nvpArray)
			{
                if (!string.IsNullOrWhiteSpace(nvp))
				{
					expectedXPathAsList.Add(nvp.Trim());
				}
			}
			return expectedXPathAsList;
		}

		public override string toXmlString(string content)
		{
			return content;
		}
	}

}