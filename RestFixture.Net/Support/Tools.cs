using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

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
    /// Misc tool methods for string manipulation.
    /// </summary>
    public sealed class Tools
    {

        private Tools()
        {

        }

        /// <param name="name">  the name </param>
        /// <param name="value"> the value </param>
        /// <param name="nvSep"> the separator </param>
        /// <returns> the kvp as a string <code>&lt;name&gt;&lt;sep&gt;&lt;value&gt;</code>. </returns>
        public static string convertEntryToString(string name, string value, string nvSep)
        {
            return string.Format("{0}{1}{2}", name, nvSep, value);
        }

        /// <param name="text"> the text </param>
        /// <param name="expr"> the regex </param>
        /// <returns> true if regex matches text. </returns>
        public static bool regex(string text, string expr)
        {
            try
            {
                return Regex.IsMatch(text, expr);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException("Either regex or string being searched is null");
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Invalid regex " + expr, ex);
            }
        }

        /// <summary>
        /// parses a map from a string.
        /// </summary>
        /// <param name="expStr">    the string with the serialised map. </param>
        /// <param name="nvSep">     the separator for keys and values. </param>
        /// <param name="entrySep">  the separator for entries in the map. </param>
        /// <param name="cleanTags"> if true the value is cleaned from any present html tag. </param>
        /// <returns> the parsed map. </returns>
        public static IDictionary<string, string> convertStringToMap(string expStr, string nvSep, 
            string entrySep, bool cleanTags)
        {
            string sanitisedExpStr = expStr.Trim();
            sanitisedExpStr = removeOpenEscape(sanitisedExpStr);
            sanitisedExpStr = removeCloseEscape(sanitisedExpStr);
            sanitisedExpStr = sanitisedExpStr.Trim();
            string[] nvpArray = sanitisedExpStr.Split(new string[] {entrySep}, 
                StringSplitOptions.None);

            IDictionary<string, string> ret = new Dictionary<string, string>();

            foreach (string nvp in nvpArray)
            {
                try
                {
                    string keyValueText = nvp.Trim();
                    if (string.IsNullOrEmpty(keyValueText))
                    {
                        continue;
                    }
                    keyValueText = removeOpenEscape(keyValueText).Trim();
                    keyValueText = removeCloseEscape(keyValueText).Trim();
                    string[] nvpArr = keyValueText.Split(new string[] { nvSep }, 
                        StringSplitOptions.None);
                    string k, v;
                    k = nvpArr[0].Trim();
                    v = "";
                    if (nvpArr.Length == 2)
                    {
                        v = nvpArr[1].Trim();
                    }
                    else if (nvpArr.Length > 2)
                    {
                        int pos = keyValueText.IndexOf(nvSep, StringComparison.Ordinal) + nvSep.Length;
                        v = keyValueText.Substring(pos).Trim();
                    }
                    if (cleanTags)
                    {
                        ret[k] = fromSimpleTag(v);
                    }
                    else
                    {
                        ret[k] = v;
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage =
                        string.Format("Each entry in the must be separated by '{0}' and "
                                      + "each entry must be expressed as a name{1}value",
                                    entrySep, nvSep);
                    throw new System.ArgumentException(errorMessage, ex);
                }
            }
            return ret;
        }

        /// <param name="message"> the message to be included in the collapsable section header. </param>
        /// <param name="content"> the content collapsed. </param>
        /// <returns> a string with the html/js code to implement a collapsable section
        /// in fitnesse. </returns>
        public static string makeToggleCollapseable(string message, string content)
        {
            Random random = new Random();
            // The Java implementation used random.nextLong but the .NET Random class only has 
            //  Next, which returns an int.
            string id = Convert.ToString(content.GetHashCode()) + Convert.ToString(random.Next());
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='collapsible closed'>");
            sb.Append(
                "<ul><li><a href='#' class='expandall'>Expand</a></li><li><a href='#' class='collapseall'>Collapse</a></li></ul>");
            sb.Append("<p class='title'>").Append(message).Append("</p>");
            sb.Append("<div>").Append(content).Append("</div>");
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// <table border="1">
        /// <caption>Substitutions</caption>
        /// <tr>
        /// <td><code>&lt;pre&gt;</code> and <code>&lt;/pre&gt;</code></td>
        /// <td><code>""</code></td>
        /// </tr>
        /// <tr>
        /// <td><code>&lt;</code></td>
        /// <td><code>&amp;lt;</code></td>
        /// </tr>
        /// <tr>
        /// <td><code>\n</code></td>
        /// <td><code>&lt;br /&gt;</code></td>
        /// </tr>
        /// <tr>
        /// <td><code>&nbsp;</code> <i>(space)</i></td>
        /// <td><code>&amp;nbsp;</code></td>
        /// </tr>
        /// <tr>
        /// <td><code>-----</code> <i>(5 hyphens)</i></td>
        /// <td><code>&lt;hr /&gt;</code></td>
        /// </tr>
        /// </table>
        /// </summary>
        /// <param name="text"> some text. </param>
        /// <returns> the html. </returns>
        public static string toHtml(string text)
        {
            return
                text.Replace("<pre>", "")
                    .Replace("</pre>", "")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .RegexReplace(@"\r\n?|\n", "<br/>")
                    .Replace("\t", "    ")
                    .Replace(" ", "&nbsp;")
                    .Replace("-----", "<hr/>");
        }

        /// <param name="c"> some text </param>
        /// <returns> the text within <code>&lt;code&gt;</code> tags. </returns>
        public static string toCode(string c)
        {
            return "<code>" + c + "</code>";
        }

        /// <param name="somethingWithinATag"> some text enclosed in some html tag. </param>
        /// <returns> the text within the tag. </returns>
        public static string fromSimpleTag(string somethingWithinATag)
        {
            return somethingWithinATag.RegexReplace("<[^>]+>", "").RegexReplace("</[^>]+>", "");
        }

        /// <param name="text"> some html </param>
        /// <returns> the text stripped out of all tags. </returns>
        public static string fromHtml(string text)
        {
            string ls = "\n";
            return
                text.RegexReplace("<br[\\s]*/>", ls)
                    .RegexReplace("<BR[\\s]*/>", ls)
                    .RegexReplace("<span[^>]*>", "")
                    .Replace("</span>", "")
                    .Replace("<pre>", "")
                    .Replace("</pre>", "")
                    .Replace("&nbsp;", " ")
                    .Replace("&gt;", ">")
                    .Replace("&amp;", "&")
                    .Replace("&lt;", "<")
                    .Replace("&nbsp;", " ");
        }

        /// <param name="string"> a string </param>
        /// <returns> the string htmlified as a fitnesse label. </returns>
        public static string toHtmlLabel(string @string)
        {
            return "<i><span class='fit_label'>" + @string + "</span></i>";
        }

        /// <param name="href"> a string ending up in the anchor href. </param>
        /// <param name="text"> a string within anchors. </param>
        /// <returns> the string htmlified as a html link. </returns>
        public static string toHtmlLink(string href, string text)
        {
            return "<a href='" + href + "'>" + text + "</a>";
        }

        /// <param name="expected">        the expected value </param>
        /// <param name="typeAdapter">     the body adapter for the cell </param>
        /// <param name="formatter">       the formatter </param>
        /// <param name="minLenForToggle"> the value determining whether the content should be rendered
        ///                        as a collapseable section. </param>
        /// <returns> the formatted content for a cell with a wrong expectation </returns>
        public static string makeContentForWrongCell<T1>(string expected, RestDataTypeAdapter typeAdapter,
            ICellFormatter<T1> formatter, int minLenForToggle)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Tools.toHtml(expected));
            if (formatter.DisplayActual)
            {
                sb.Append(toHtml("\n"));
                sb.Append(formatter.label("expected"));
                string actual = typeAdapter.ToString();
                sb.Append(toHtml("-----"));
                sb.Append(toHtml("\n"));
                if (minLenForToggle >= 0 && actual.Length > minLenForToggle)
                {
                    sb.Append(makeToggleCollapseable("toggle actual", toHtml(actual)));
                }
                else
                {
                    sb.Append(toHtml(actual));
                }
                sb.Append(toHtml("\n"));
                sb.Append(formatter.label("actual"));
            }
            IReadOnlyList<string> errors = typeAdapter.Errors;
            if (errors.Count > 0)
            {
                sb.Append(toHtml("-----"));
                sb.Append(toHtml("\n"));
                foreach (string e in errors)
                {
                    sb.Append(toHtml(e + "\n"));
                }
                sb.Append(toHtml("\n"));
                sb.Append(formatter.label("errors"));
            }
            return sb.ToString();
        }

        /// <param name="expected">        the expected value </param>
        /// <param name="typeAdapter">     the body type adaptor </param>
        /// <param name="formatter">       the formatter
        ///                        the value determining whether the content should be rendered
        ///                        as a collapseable section. </param>
        /// <param name="minLenForToggle"> the value determining whether the content should be rendered
        ///                        as a collapseable section. </param>
        /// <returns> the formatted content for a cell with a right expectation </returns>
        public static string makeContentForRightCell<T1>(string expected, RestDataTypeAdapter typeAdapter,
            ICellFormatter<T1> formatter, int minLenForToggle)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(toHtml(expected));
            string actual = typeAdapter.ToString();
            if (formatter.DisplayActual && !expected.Equals(actual))
            {
                sb.Append(toHtml("\n"));
                sb.Append(formatter.label("expected"));
                sb.Append(toHtml("-----"));
                sb.Append(toHtml("\n"));
                if (minLenForToggle >= 0 && actual.Length > minLenForToggle)
                {
                    sb.Append(makeToggleCollapseable("toggle actual", toHtml(actual)));
                }
                else
                {
                    sb.Append(toHtml(actual));
                }
                sb.Append(toHtml("\n"));
                sb.Append(formatter.label("actual"));
            }
            return sb.ToString();
        }

        private static string removeCloseEscape(string str)
        {
            return trimStartEnd("-!", str);
        }

        private static string removeOpenEscape(string str)
        {
            return trimStartEnd("!-", str);
        }

        private static string trimStartEnd(string pattern, string str)
        {
            if (str.StartsWith(pattern, StringComparison.Ordinal))
            {
                str = str.Substring(2);
            }
            if (str.EndsWith(pattern, StringComparison.Ordinal))
            {
                str = str.Substring(0, str.Length - 2);
            }
            return str;
        }

        public static string wrapInDiv(string body)
        {
            return string.Format("<div>{0}</div>", body);
        }

        /// <summary>
        /// Parses the specified text using the Parse method of the specified type.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="type">The type that will parse the text.</param>
        /// <returns></returns>
        /// <remarks>Based on the Java FitNesse fit.Fixture.parse(String s, Class type) method.</remarks>
        /// <exception cref="TypeParseException">Thrown if the specified type does not have a 
        /// Parse method or if the type is a DateTime but the text is not a valid date or time.</exception>
        public static object parse(string text, Type type)
        {
            if (type == typeof (string))
            {
                if (text.ToLower() == "null")
                {
                    return null;
                }

                if (text.ToLower() == "blank")
                {
                    return string.Empty;
                }

                return text;
            }

            string errorMessage = null;
            if (type == typeof (DateTime))
            {
                DateTime dateTimeResult = DateTime.MinValue;
                if (DateTime.TryParse(text, out dateTimeResult))
                {
                    return dateTimeResult;
                }

                errorMessage = string.Format("Text '{0}' is not a valid date.", text);
                throw new TypeParseException(errorMessage);
            }

            MethodInfo methodInfo = type.GetMethod("Parse");
            if (methodInfo == null)
            {
                errorMessage = string.Format("Text '{0}' cannot be parsed: "
                                             + "Type '{1}' does not have a Parse method.", text, type.FullName);
                throw new TypeParseException(errorMessage);
            }

            object parser = Activator.CreateInstance(type);
            object result = methodInfo.Invoke(parser, new object[] {text});
            return result;
        }
    }

}