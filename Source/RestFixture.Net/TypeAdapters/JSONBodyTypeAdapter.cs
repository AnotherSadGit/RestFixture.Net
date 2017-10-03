using System.Collections.Generic;
using System.Text.RegularExpressions;
using RestFixture.Net.Javascript;
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
namespace RestFixture.Net.TypeAdapters
{


	/// <summary>
	/// Type adapted for cells containing JSON content.
	/// 
	/// @author smartrics
	/// </summary>
	public class JSONBodyTypeAdapter : XPathBodyTypeAdapter
	{
		private readonly IDictionary<string, string> imports;
		private readonly JavascriptWrapper wrapper;
		private bool forceJsEvaluation = false;
        private Regex _jsHeaderRegex = new Regex(@"\/\*\s*javascript\s*\*\/", 
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		/// <summary>
		/// def ctor
		/// </summary>
		/// <param name="variablesProvider"> used for substitutions </param>
		/// <param name="config">            the config </param>
		public JSONBodyTypeAdapter(IRunnerVariablesProvider variablesProvider, Config config)
		{
			wrapper = new JavascriptWrapper(variablesProvider);
			imports = config.getAsMap("restfixture.javascript.imports.map", new Dictionary<string, string>());
		}

		protected internal override bool eval(string expr, string json)
		{
			// for backward compatibility we should keep for now xpath expectations
			if (!forceJsEvaluation && XmlTools.isValidXPath(Context, expr) && !wrapper.looksLikeAJsExpression(expr))
			{
				throw new System.ArgumentException("XPath expectations in JSON content are not supported anymore. Please use JavaScript expressions.");
			}
			object exprResult = wrapper.evaluateExpression(json, expr, imports);
			if (exprResult == null)
			{
				return false;
			}
			return bool.Parse(exprResult.ToString());
		}

		public override object parse(string possibleJsContent)
		{
            if (possibleJsContent == null || !HasJavaScriptHeader(possibleJsContent.Trim()))
			{
				forceJsEvaluation = false;
				return base.parse(possibleJsContent);
			}
			forceJsEvaluation = true;
			return HtmlTools.fromHtml(possibleJsContent.Trim());
		}

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
			if (expected is IList<string>)
			{
				return base.Equals(expected, actual);
			}
			bool result = false;
			if (expected is string)
			{
				result = eval(expected.ToString(), actual.ToString());
				if (!result)
				{
					addError("not found: '" + expected.ToString() + "'");
				}
			}
			return result;
		}

        public override string ToString(object obj)
		{
			if (obj == null || obj.ToString().Trim().Equals(""))
			{
				return "no-body";
			}
			// the actual value is passed as an xml string
			// TODO: pretty print?
			return obj.ToString();
		}

		public override string toXmlString(string content)
		{
			return XmlTools.fromJSONtoXML(content);
		}

        /// <summary>
        /// Determines whether the specified content string contains a JavaScript block 
        /// identifier: "/* javascript */"
        /// </summary>
        /// <param name="jsContent">The content to check.</param>
        /// <returns>This is more robust than the Java RestFixture implementation, which 
        /// just does a string.Contains, so would fail if the identifier isn't exactly as 
        /// specified (eg contains zero or more than one leading or trailing space, or 
        /// uses tabs instead of spaces).</returns>
	    private bool HasJavaScriptHeader(string jsContent)
	    {
	        if (string.IsNullOrWhiteSpace(jsContent))
	        {
	            return false;
	        }

            bool isMatch = _jsHeaderRegex.IsMatch(jsContent);
            return isMatch;
	    }
	}

}