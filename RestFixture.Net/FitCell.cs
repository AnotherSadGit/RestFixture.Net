using System;
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
	using Parse = fit.Parse;

	/// <summary>
	/// Wrapper class to a table cell for the Fit runner.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class FitCell : CellWrapper<Parse>
	{

		private readonly Parse cell;

		/// <summary>
		/// a fit cell </summary>
		/// <param name="c"> the parse object representing the cell. </param>
		public FitCell(Parse c)
		{
			this.cell = c;
		}

		public string text()
		{
			try
			{
				return cell.Text;
			}
			catch (Exception)
			{
				return "";
			}
		}

		public void body(string @string)
		{
            cell.SetBody(@string);
		}

		public string body()
		{
			return cell.Body;
		}

		public void addToBody(string @string)
		{
			cell.AddToBody(@string);
		}

		public Parse Wrapped
		{
			get
			{
				return cell;
			}
		}
	}

}