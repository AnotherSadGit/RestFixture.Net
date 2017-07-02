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

using fitSharp.Machine.Engine;

namespace RestFixture.Net.Support
{

	using Fixture = fit.Fixture;

	/// <summary>
	/// Facade to FitNesse global symbols map for FIT.
	/// 
	/// @author smartrics
	/// </summary>
	public class FitVariables : Variables
	{
	    private static Symbols _symbols;

		/// <summary>
		/// initialises variables with default config. See @link
		/// <seealso cref="#FitVariables(Config)"/>
		/// </summary>
		public FitVariables() : base()
		{
		}

		/// <summary>
		/// initialises the variables. reade
		/// {@code restfixture.null.value.representation} to know how to render
		/// {@code null}s.
		/// </summary>
		/// <param name="c"> the config </param>
		public FitVariables(Config c) : base(c)
		{
		}

	    static FitVariables()
	    {
	        _symbols = new Fixture().Symbols;
	    }

		/// <summary>
		/// puts a value.
		/// </summary>
		/// <param name="label"> the symbol </param>
		/// <param name="val"> the value </param>
		public override void put(string label, string val)
		{
			_symbols.Save(label, val);
		}

		/// <summary>
		/// gets a value.
		/// </summary>
		/// <param name="label"> the symbol </param>
		/// <returns> the value. </returns>
		public override string get(string label)
		{
			if (_symbols.HasValue(label))
		    {
                return (string)_symbols.GetValue(label);
		    }
			return null;
		}

		/// <summary>
		/// crears all variables
		/// (used for tests only, given the fact that the Fit variables are in fact static)
		/// </summary>
		public virtual void clearAll()
		{
			_symbols.Clear();
		}

	}

}