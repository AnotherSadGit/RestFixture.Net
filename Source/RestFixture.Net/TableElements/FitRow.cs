using System.Collections.Generic;
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
namespace RestFixture.Net.TableElements
{
	/// <summary>
	/// Wrapper class for table row for Fit Runner.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class FitRow : IRowWrapper<Parse>
	{
		private readonly Parse cells;

		private readonly IList<ICellWrapper<Parse>> row;

		/// <summary>
		/// a fit row </summary>
		/// <param name="parse"> the parse object representing the row. </param>
		public FitRow(Parse parse)
		{
			this.cells = parse;
			Parse next = cells;
			row = new List<ICellWrapper<Parse>>();
			while (next != null)
			{
				row.Add(new FitCell(next));
				next = next.More;
			}
		}

		public virtual int size()
		{
			if (row != null)
			{
				return row.Count;
			}
			return 0;
		}

        public virtual ICellWrapper<Parse> getCell(int c)
		{
			if (c < row.Count)
			{
				return row[c];
			}
			return null;
		}

        public virtual ICellWrapper<Parse> removeCell(int c)
		{
			if (c < this.row.Count)
            {
                ICellWrapper<Parse> removedCell = this.row[c];
                this.row.Remove(removedCell);
                return removedCell;
            }
            return null;
		}
	}

}