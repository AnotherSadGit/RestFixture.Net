using System.Collections.Generic;
using RestFixture.Net.Support;

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
	/// <summary>
	/// Wrapper class for a row when running with Slim.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
    public class SlimRow : IRowWrapper
	{

		private readonly IList<ICellWrapper> row;

		/// <param name="rawRow"> a list of string representing the row cells as passed by Slim. </param>
		public SlimRow(IList<string> rawRow)
		{
			this.row = new List<ICellWrapper>();
			foreach (string r in rawRow)
			{
				this.row.Add(new SlimCell(r));
			}
		}

		public virtual ICellWrapper getCell(int c)
		{
			if (c < this.row.Count)
			{
				return this.row[c];
			}
			return null;
		}

		public virtual int size()
		{
			if (row != null)
			{
				return row.Count;
			}
			return 0;
		}

		/// <returns> the row as list of strings. </returns>
		public virtual IList<string> asList()
		{
			IList<string> ret = new List<string>();
			foreach (ICellWrapper w in row)
			{
				ret.Add(w.body());
			}
			return ret;
		}

		public virtual ICellWrapper removeCell(int c)
		{
			if (c < this.row.Count)
			{
			    ICellWrapper removedCell = this.row[c];
			    this.row.Remove(removedCell);
			    return removedCell;
			}
			return null;
		}
	}

}