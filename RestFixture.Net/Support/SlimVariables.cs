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

	using StatementExecutorInterface = fitnesse.slim.StatementExecutorInterface;

	/// <summary>
	/// Facade to FitNesse global symbols map for SliM.
	/// 
	/// @author smartrics
	/// </summary>
	public class SlimVariables : Variables
	{

		private readonly StatementExecutorInterface executor;

		/// <summary>
		/// initialises the variables. reade
		/// {@code restfixture.null.value.representation} to know how to render
		/// {@code null}s.
		/// </summary>
		/// <param name="c">        the config object </param>
		/// <param name="executor"> the executor </param>
		public SlimVariables(Config c, StatementExecutorInterface executor) : base(c)
		{
			this.executor = executor;
		}

		/// <summary>
		/// puts a value.
		/// </summary>
		/// <param name="label"> the symbol </param>
		/// <param name="val">   the value to store </param>
		public virtual void put(string label, string val)
		{
			if (string.ReferenceEquals(val, null) || val.Equals(base.nullValue))
			{
				executor.assign(label, null);
			}
			else
			{
				executor.assign(label, val);
			}
		}

		/// <summary>
		/// gets a value.
		/// </summary>
		/// <param name="label"> the symbol </param>
		/// <returns> the value. </returns>
		public virtual string get(string label)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object symbol = executor.getSymbol(label);
			object symbol = executor.getSymbol(label);
			if (symbol == null)
			{
				return base.nullValue;
			}
			return symbol.ToString();
		}

	}


}