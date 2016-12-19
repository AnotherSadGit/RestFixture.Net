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

	/// <summary>
	/// Type adapter for cells containing strings.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class StringTypeAdapter : RestDataTypeAdapter
	{

		public override bool Equals(object expected, object actual)
		{
			string se = "null";
			if (expected != null)
			{
				se = expected.ToString();
			}
			string sa = "null";
			if (actual != null)
			{
				sa = actual.ToString();
			}
			return se.Equals(sa);
		}

		public override object parse(string s)
		{
			if ("null".Equals(s))
			{
				return null;
			}
			if ("blank".Equals(s))
			{
				return "";
			}
			return s;
		}

		public override string ToString(object obj)
		{
			if (obj == null)
			{
				return "null";
			}
			if ("".Equals(obj.ToString().Trim()))
			{
				return "blank";
			}
			return obj.ToString();

		}
	}

}