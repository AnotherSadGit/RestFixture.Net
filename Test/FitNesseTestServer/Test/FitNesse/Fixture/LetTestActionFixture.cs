using fit;

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
    /// <summary>
	/// Supports Let CATs by providing a simple interface to FitNesse symbols map.
	/// </summary>
	public class LetTestActionFixture : ActionFixture
	{
		public string SymbolName { get; set; }

        public string SymbolValue
        {
            get
            {
                return (string)Symbols.GetValue(this.SymbolName);
            }

            set
            {
                this.Symbols.SaveValue(this.SymbolName, value);
            }
        }
	}

}