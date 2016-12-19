using System.Collections.Generic;

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
namespace RestFixture.Net.Support
{


	using Fixture = fit.Fixture;

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
		public static Pattern SPECIAL_REGEX_CHARS = Pattern.compile("[{}()\\[\\].+*?^$\\\\|]");
		// exclude 0-9, A-F from the first variable name as those are confused with URL encodings.
		public static readonly Pattern VARIABLES_PATTERN = Pattern.compile("\\%([a-zG-Z_][a-zA-Z0-9_]*)\\%");
		// original regex pattern, allowing all initial characters.
		//	public static final Pattern VARIABLES_PATTERN = Pattern.compile("\\%([a-zA-Z0-9_]+)\\%");
		private static readonly string FIT_NULL_VALUE = fitSymbolForNull();
		protected internal string nullValue = "null";

		/// <summary>
		/// initialises variables with default config. See @link
		/// <seealso cref="#Variables(Config)"/>
		/// </summary>
		internal Variables() : this(Config.Config)
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
				this.nullValue = c.get("restfixture.null.value.representation", "null");
			}
		}

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
			if (string.ReferenceEquals(text, null))
			{
				return null;
			}
			Matcher m = VARIABLES_PATTERN.matcher(text);
			IDictionary<string, string> replacements = new Dictionary<string, string>();
			while (m.find())
			{
				int gc = m.groupCount();
				if (gc == 1)
				{
					string g0 = m.group(0);
					string g1 = m.group(1);
					string value = get(g1);
					if (FIT_NULL_VALUE.Equals(value))
					{
						value = nullValue;
					}
					replacements[g0] = value;
				}
			}
			string newText = text;
			foreach (KeyValuePair<string, string> en in replacements.SetOfKeyValuePairs())
			{
				string k = en.Key;
				string replacement = replacements[k];
				if (!string.ReferenceEquals(replacement, null))
				{
					// this fixes issue #118
					string sanitisedReplacement = SPECIAL_REGEX_CHARS.matcher(replacement).replaceAll("\\\\$0");
					newText = newText.replaceAll(k, sanitisedReplacement);
				}
			}
			return newText;
		}

		private static string fitSymbolForNull()
		{
			const string k = "somerandomvaluetogettherepresentationofnull-1234567890";
			Fixture.setSymbol(k, null);
			return Fixture.getSymbol(k).ToString();
		}

		/// <param name="s"> the string to process </param>
		/// <returns> the null representation if the input is null. </returns>
		public string replaceNull(string s)
		{
			if (string.ReferenceEquals(s, null))
			{
				return nullValue;
			}
			return s;
		}
	}

}