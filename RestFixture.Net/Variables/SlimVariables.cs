using System.Collections.Generic;

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
	/// Facade to FitNesse global symbols map for SliM.
	/// 
	/// @author smartrics
	/// </summary>
    /// <remarks>According to this FitSharp issue, https://github.com/jediwhale/fitsharp/issues/123, 
    /// the maintainer of FitSharp says "I don't know of a way to access symbols via code with Slim". 
    /// This applies only to FitSharp, not the original Java implementation of FitNesse.  He also 
    /// suggests that because Slim is mainly encapsulated in the common FitNesse code, written in 
    /// Java, that other languages will always be second class citizens in Slim and that Slim is 
    /// not used much outside of Java for that reason.
    /// 
    /// Hence the Java SlimVariables constructor that takes a StatementExecutorInterface 
    /// parameter, which presumably contains the symbol map, has not been ported to this 
    /// implementation in .NET.</remarks>
	public class SlimVariables : Variables
	{

		private IDictionary<string, string> _symbols = new Dictionary<string, string>(); 

	    /// <summary>
	    /// initialises the variables.
	    /// </summary>
	    /// <param name="c">        the config object </param>
	    public SlimVariables(Config c) : base(c)
	    {
	    }

	    /// <summary>
		/// puts a value.
		/// </summary>
		/// <param name="label"> the symbol </param>
		/// <param name="val">   the value to store </param>
        public override void put(string label, string val)
		{
			if (string.ReferenceEquals(val, null) || val.Equals(base._nullValue))
			{
				_symbols[label] = null;
			}
			else
			{
				_symbols[label] = val;
			}
		}

		/// <summary>
		/// gets a value.
		/// </summary>
		/// <param name="label"> the symbol </param>
		/// <returns> the value. </returns>
		public override string get(string label)
		{
			string value = _symbols.GetValueOrNull(label);
            if (value == null)
			{
				return base._nullValue;
			}
            return value;
		}

	}


}