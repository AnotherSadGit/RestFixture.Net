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
namespace FitNesseTestServer.Test.FitNesse.Fixture
{

	using ActionFixture = fit.ActionFixture;

    /// <summary>
	/// Supports Let CATs by providing a simple interface to FitNesse symbols map.
	/// 
	/// @author fabrizio
	/// 
	/// </summary>
	public class LetTestActionFixture : ActionFixture
	{
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private string symbolName_Renamed;

		public virtual void symbolName(string name)
		{
			this.symbolName_Renamed = name;
		}

		public virtual string symbolValue()
		{
			return (string) Fixture.getSymbol(symbolName_Renamed);
		}

		public virtual void symbolValue(string val)
		{
			Fixture.setSymbol(symbolName_Renamed, val);
		}
	}

}