using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using restFixture.Net.Support;
using restFixture.Net.TableElements;
using restFixture.Net.TypeAdapters;

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
namespace restFixture.Net.Tools
{
    /// <summary>
    /// Misc tool methods for string manipulation.
    /// </summary>
    public sealed class StringTools
    {

        private StringTools()
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
                        ret[k] = HtmlTools.fromSimpleTag(v);
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