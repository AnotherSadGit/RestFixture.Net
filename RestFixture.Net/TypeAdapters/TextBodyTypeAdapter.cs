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

using System;
using System.Text.RegularExpressions;

namespace RestFixture.Net.Support
{


	using Parse = fit.Parse;

	/// <summary>
	/// Type adapter for body cell containing plain text.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class TextBodyTypeAdapter : BodyTypeAdapter
	{

		public override bool Equals(object exp, object act)
		{
			if (exp == null || act == null)
			{
				return false;
			}
			string expected = exp.ToString();
			if (exp is Parse)
			{
				expected = ((Parse) exp).Text;
			}
			string actual = (string) act;
			try
			{
				if (!Regex.IsMatch(actual, expected))
			    {
                    addError("no regex match: " + expected);
			    }
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException("Either regex or string being searched is null");
            }
            catch (ArgumentException)
			{
				// lets try to string match just to be kind
                if (actual != expected)
				{
					addError("no string match found: " + expected);
				}
			}
			return Errors.Count == 0;
		}

		public override object parse(string s)
		{
            if (s == null)
			{
				return "null";
			}
			return s.Trim();
		}

		public override string toXmlString(string content)
		{
			return "<text>" + content + "</text>";
		}

	}

}