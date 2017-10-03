using System;
using System.Collections.Generic;
using System.Text;
using RestClient.Data;
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
	/// Type adapter for HTTP header cell in a RestFixture table.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class HeadersTypeAdapter : RestDataTypeAdapter
	{
		public override bool Equals(object expectedObj, object actualObj)
		{
			if (expectedObj == null || actualObj == null)
			{
				return false;
			}
			// r1 and r2 are Map<String, String> containing either the header
			// from the HTTP response or the data value in the expected cell
			// equals checks for r1 being a subset of r2
			ICollection<RestData.Header> expected = (ICollection<RestData.Header>) expectedObj;
			ICollection<RestData.Header> actual = (ICollection<RestData.Header>) actualObj;
			foreach (RestData.Header k in expected)
			{
				RestData.Header aHdr = find(actual, k);
				if (aHdr == null)
				{
					addError("not found: [" + k.Name + " : " + k.Value + "]");
				}
			}
			return Errors.Count == 0;
		}

		private RestData.Header find(ICollection<RestData.Header> actualHeaders, 
            RestData.Header expectedHeader)
		{
			foreach (RestData.Header h in actualHeaders)
			{
				bool nameMatches = h.Name.Equals(expectedHeader.Name);
			    string expectedRegexPattern = expectedHeader.Value;
			    if (!string.IsNullOrWhiteSpace(expectedRegexPattern))
			    {
                    expectedRegexPattern = expectedRegexPattern.Trim();
                    // Was failing to match a URL with a query string because the "?" was parsed 
                    //  as a regex metacharacter.  So if the regex pattern appears to be a 
                    //  URL assume the "?" represents the start of the query string and escape it.
                    if (expectedRegexPattern.ToLower().StartsWith("http://")
                        || expectedRegexPattern.ToLower().StartsWith("https://"))
                    {
                        expectedRegexPattern = expectedRegexPattern.Replace("?", @"\?");
                    }
			    }
                bool valueMatches = StringTools.regex(h.Value, expectedRegexPattern);
				if (nameMatches && valueMatches)
				{
					return h;
				}
			}
			return null;
		}

		public override object parse(string s)
		{
			// parses a cell content as a map of headers.
			// syntax is name:value\n*
			IList<RestData.Header> expected = new List<RestData.Header>();
			if (!"".Equals(s.Trim()))
			{
				string expStr = HtmlTools.fromHtml(s.Trim());
				string[] nvpArray = expStr.Split(new char[] {'\n'});
				foreach (string nvp in nvpArray)
				{
					try
					{
						string[] nvpEl = nvp.Split(new char[] {':'}, 2);
						expected.Add(new RestData.Header(nvpEl[0].Trim(), nvpEl[1].Trim()));
					}
					catch (Exception)
					{
						throw new System.ArgumentException("Each entry in the must be separated by \\n and each entry must be expressed as a name:value");
					}
				}
			}
			return expected;
		}

		public override string ToString(object obj)
		{
			StringBuilder b = new StringBuilder();
			IList<RestData.Header> list = (IList<RestData.Header>) obj;
			foreach (RestData.Header h in list)
			{
				b.Append(h.Name).Append(" : ").Append(h.Value).Append("\n");
			}
			return b.ToString().Trim();
		}

	}

}