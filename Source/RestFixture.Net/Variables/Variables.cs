﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using restFixture.Net.Support;

/*  Copyright 2017 Simon Elms
 *
 *  This file is part of RestFixture.Net
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
namespace restFixture.Net.Variables
{
    /// <summary>
	/// Facade to FitNesse global symbols map.
	/// 
	/// @author smartrics
	/// </summary>
	public abstract class Variables
	{
        /// <summary>
        /// pattern matching a variable name: {@code \%([a-zA-Z0-9_]+)\%}
        /// </summary>
        public static readonly Regex SpecialCharactersRegex = new Regex("[{}()\\[\\].+*?^$\\\\|]", RegexOptions.Compiled);

        // Previous regex pattern which simply excluded hex digits (0-9, A-F) from the first 
        //  character of the variable name to avoid confusion with URL encodings: 
        //  "\\%([a-zG-Z_][a-zA-Z0-9_]*)\\%"

        // Current regex excludes pairs of hex digits but allows single hex digits, 3 or more hex 
        //  digits, or a hex digit + a non-hex digit, eg %GA%, %AG%, %A_%.
        public static readonly Regex VariablesRegex =
            new Regex(@"\%([a-zA-Z_]|[g-zG-Z_][a-zA-Z0-9_]|[a-zA-Z_][g-zG-Z_]|[a-zA-Z_][a-zA-Z0-9_]{2,})\%", 
                RegexOptions.Compiled);

		private static readonly string FIT_NULL_VALUE = (string)null;
		protected internal string _nullValue = "null";

		/// <summary>
		/// initialises variables with default config. See @link
		/// <seealso cref="#Variables(Config)"/>
		/// </summary>
        internal Variables()
            : this(Config.getConfig())
		{
		}

		/// <summary>
		/// initialises the variables. reade
		/// {@code restfixture.null.value.representation} to know how to render
		/// {@code null}s.
		/// </summary>
		/// <param name="c"> the config </param>
		internal Variables(Config c)
		{
			if (c != null)
			{
				this._nullValue = c.get("restfixture.null.value.representation", "null");
			}
		}

        /// <summary>
        /// All variables.
        /// </summary>
        /// <remarks>Exposed so that a wrapper class can enumerate the variables to allow them to 
        /// be accessed from JavaScript.</remarks>
        public abstract IDictionary<string, object> Items { get; }

		/// <summary>
		/// puts a value.
		/// </summary>
		/// <param name="label"> the symbol </param>
		/// <param name="val"> the value </param>
		public abstract void put(string label, string val);

		/// <summary>
		/// gets a value.
		/// </summary>
		/// <param name="label"> the symbol </param>
		/// <returns> the value. </returns>
		public abstract string get(string label);

		/// <summary>
		/// replaces a text with variable values. </summary>
		/// <param name="text"> the text to process </param>
		/// <returns> the substituted text. </returns>
		public string substitute(string text)
		{
			if (text == null)
			{
				return null;
			}

			IDictionary<string, string> replacements = new Dictionary<string, string>();
		    MatchCollection matches = VariablesRegex.Matches(text);
		    foreach (Match match in matches)
		    {
		        GroupCollection groups = match.Groups;
                // First group is always the entire match so a match will always have at least one 
                //  group.
		        if (groups.Count == 2)
		        {
		            string textToSubstitute = groups[0].Value;
                    string variableName = groups[1].Value;
		            string variableValue = get(variableName);
                    if (variableValue == FIT_NULL_VALUE)
                    {
                        variableValue = _nullValue;
                    }
                    replacements[textToSubstitute] = variableValue;
		        }
		    }

			string newText = text;
		    foreach (string textToSubstitute in replacements.Keys)
		    {
		        string replacement = replacements[textToSubstitute];

		        if (replacement != null)
		        {
                    // Cope with text that appears to be a regex.  
                    //  See https://github.com/smartrics/RestFixture/issues/118

                    //TODO: Check that the substitution expression works. 
                    //  The original Java substitution expression was "\\\\$0" but Java 
                    //  substitution expressions can include character escapes while .NET ones 
                    //  can't.  So the Java substitution expression "\\\\$0" probably 
                    //  translates to "\\$0" in .NET.  But we need to check this.
                    string sanitisedReplacement = SpecialCharactersRegex.Replace(replacement, "\\$0");
		            newText = newText.Replace(textToSubstitute, sanitisedReplacement);
		        }
            }
            return newText;
		}

		/// <param name="s"> the string to process </param>
		/// <returns> the null representation if the input is null. </returns>
		public string replaceNull(string s)
		{
            return s ?? _nullValue;
		}
	}

}