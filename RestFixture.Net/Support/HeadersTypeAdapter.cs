using System;
using System.Collections.Generic;
using System.Text;

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


	using Header = smartrics.rest.client.RestData.Header;

	/// <summary>
	/// Type adapter for HTTP header cell in a RestFixture table.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class HeadersTypeAdapter : RestDataTypeAdapter
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public boolean equals(Object expectedObj, Object actualObj)
		public override bool Equals(object expectedObj, object actualObj)
		{
			if (expectedObj == null || actualObj == null)
			{
				return false;
			}
			// r1 and r2 are Map<String, String> containing either the header
			// from the HTTP response or the data value in the expected cell
			// equals checks for r1 being a subset of r2
			ICollection<Header> expected = (ICollection<Header>) expectedObj;
			ICollection<Header> actual = (ICollection<Header>) actualObj;
			foreach (Header k in expected)
			{
				Header aHdr = find(actual, k);
				if (aHdr == null)
				{
					addError("not found: [" + k.Name + " : " + k.Value + "]");
				}
			}
			return Errors.size() == 0;
		}

		private Header find(ICollection<Header> actual, Header k)
		{
			foreach (Header h in actual)
			{
				bool nameMatches = h.Name.Equals(k.Name);
				bool valueMatches = Tools.regex(h.Value, k.Value);
				if (nameMatches && valueMatches)
				{
					return h;
				}
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Object parse(String s) throws Exception
		public override object parse(string s)
		{
			// parses a cell content as a map of headers.
			// syntax is name:value\n*
			IList<Header> expected = new List<Header>();
			if (!"".Equals(s.Trim()))
			{
				string expStr = Tools.fromHtml(s.Trim());
				string[] nvpArray = expStr.Split("\n", true);
				foreach (string nvp in nvpArray)
				{
					try
					{
						string[] nvpEl = nvp.Split(":", 2);
						expected.Add(new Header(nvpEl[0].Trim(), nvpEl[1].Trim()));
					}
					catch (Exception)
					{
						throw new System.ArgumentException("Each entry in the must be separated by \\n and each entry must be expressed as a name:value");
					}
				}
			}
			return expected;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public String toString(Object obj)
		public override string ToString(object obj)
		{
			StringBuilder b = new StringBuilder();
			IList<Header> list = (IList<Header>) obj;
			foreach (Header h in list)
			{
				b.Append(h.Name).Append(" : ").Append(h.Value).Append("\n");
			}
			return b.ToString().Trim();
		}

	}

}