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
namespace restFixture.Net.Support
{

	/// <summary>
	/// Wrapper to a table row.
	/// 
	/// @author smartrics
	/// </summary>
	public interface IRowWrapper<T>
	{

		/// <param name="c"> the cell index </param>
		/// <returns> the <seealso cref="ICellWrapper{T}"/> at a given position </returns>
		ICellWrapper<T> getCell(int c);

		/// <returns> the row size. </returns>
		int size();

		/// <summary>
		/// removes a cell at a given position.
		/// </summary>
		/// <param name="c"> the cell index </param>
		/// <returns> the removed cell. </returns>
		ICellWrapper<T> removeCell(int c);
	}

}