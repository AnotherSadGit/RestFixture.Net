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
namespace RestFixture.Net
{

	using CellWrapper = smartrics.rest.fitnesse.fixture.support.CellWrapper;

	/// <summary>
	/// Wrapper for a cell in the table when running on Slim.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class SlimCell : CellWrapper<string>
	{

		private string cell;

		/// <summary>
		/// a slim cell. </summary>
		/// <param name="c"> the content. </param>
		public SlimCell(string c)
		{
			this.cell = c;
		}

		public override string text()
		{
			return cell;
		}

		public override void body(string @string)
		{
			cell = @string;
		}

		public override string body()
		{
			return cell;
		}

		public override void addToBody(string @string)
		{
			cell = cell + @string;
		}

		public override string Wrapped
		{
			get
			{
				return cell;
			}
		}

	}

}