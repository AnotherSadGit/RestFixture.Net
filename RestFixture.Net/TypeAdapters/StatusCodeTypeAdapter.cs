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

	using Parse = fit.Parse;

	/// <summary>
	/// Type adapter for handling http status code cell.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class StatusCodeTypeAdapter : RestDataTypeAdapter
	{

		public override bool Equals(object r1, object r2)
		{
			if (r1 == null || r2 == null)
			{
				return false;
			}
			string expected = r1.ToString();
			if (r1 is Parse)
			{
				expected = ((Parse) r1).Text;
			}
			string actual = (string) r2;
			if (!Tools.regex(actual, expected))
			{
				addError("not match: " + expected);
			}
			return Errors.Count == 0;
		}

		public override object parse(string s)
		{
			if (string.ReferenceEquals(s, null))
			{
				return "null";
			}
			return s.Trim();
		}

		public override string ToString(object obj)
		{
			if (obj == null)
			{
				return "null";
			}
			if (obj.ToString().Trim().Equals(""))
			{
				return "blank";
			}
			return obj.ToString();
		}
	}

}