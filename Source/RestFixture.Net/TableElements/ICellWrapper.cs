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

namespace RestFixture.Net.TableElements
{
    /// <summary>
	/// Wrapper of a Slim/Fit cell.
	/// 
	/// @author smartrics
	/// </summary>
	public interface ICellWrapper<T>
	{
        /// <returns> the underlying cell object. </returns>
        T Wrapped { get; }

		/// <returns> the text in the cell. </returns>
		string text();

		/// <param name="text">
		///            the body of the cell to set. </param>
		void body(string text);

		/// <returns> the current body of the cell. </returns>
		string body();

		/// <summary>
		/// appends to the current cell body.
		/// </summary>
		/// <param name="text">
		///            the string to append. </param>
		void addToBody(string text);
	}
}