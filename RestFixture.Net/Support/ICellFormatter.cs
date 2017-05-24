using System;

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

	/// <summary>
	/// Formatter of the content of a cell.
	/// 
	/// @author smartrics
	/// </summary>
    /// <typeparam name="E">the type of the cell</typeparam>
	public interface ICellFormatter<E>
	{

		/// <summary>
		/// formats a cell containing an exception.
		/// </summary>
		/// <param name="cellWrapper">
		///            the cell wrapper </param>
		/// <param name="exception">
		///            the excteption to render. </param>
		void exception(ICellWrapper<E> cellWrapper, Exception exception);

		/// <summary>
		/// formats a cell containing an exception.
		/// </summary>
		/// <param name="cellWrapper">
		///            the cell wrapper </param>
		/// <param name="exceptionMessage">
		///            the exception message to render. </param>
		void exception(ICellWrapper<E> cellWrapper, string exceptionMessage);

		/// <summary>
		/// formats a check cell.
		/// </summary>
		/// <param name="valueCell">
		///            the cell value. </param>
		/// <param name="adapter">
		///            the adapter interpreting the value. </param>
		void check(ICellWrapper<E> valueCell, RestDataTypeAdapter adapter);

		/// <summary>
		/// formats a cell label
		/// </summary>
		/// <param name="string">
		///            the label </param>
		/// <returns> the cell content as a label. </returns>
		string label(string @string);

		/// <summary>
		/// formats a cell representing a wrong expectation.
		/// </summary>
		/// <param name="expected">
		///            the expected value </param>
		/// <param name="typeAdapter">
		///            the adapter with the actual value. </param>
		void wrong(ICellWrapper<E> expected, RestDataTypeAdapter typeAdapter);

		/// <summary>
		/// formats a cell representing a right expectation.
		/// </summary>
		/// <param name="expected">
		///            the expected value </param>
		/// <param name="typeAdapter">
		///            the adapter with the actual value. </param>
		void right(ICellWrapper<E> expected, RestDataTypeAdapter typeAdapter);

		/// <summary>
		/// formats a cell with a gray background. used to ignore the content or for
		/// comments.
		/// </summary>
		/// <param name="string">
		///            the content </param>
		/// <returns> the content grayed out. </returns>
		string gray(string @string);

		/// <summary>
		/// formats the content as a hyperlink.
		/// </summary>
		/// <param name="cell">
		///            the cell. </param>
		/// <param name="resolvedUrl">
		/// 	          the cell content after symbols' substitution. </param>
		/// <param name="link">
		///            the uri in the href. </param>
		/// <param name="text">
		///            the text. </param>
		void asLink(ICellWrapper<E> cell, string resolvedUrl, string link, string text);

		/// <summary>
		/// sets whether the cell should display the actual value after evaluation.
		/// </summary>
		/// <param name="displayActual">
		///            true if actual value has to be rendered. </param>
		bool DisplayActual {set;get;}

		/// <summary>
		/// sets whether absolute urls are displayed in full
		/// </summary>
		/// <param name="displayAbsoluteURLInFull"> the value to set </param>
		bool DisplayAbsoluteURLInFull {set;}

		/// <summary>
		/// renders the cell as a toggle area if the content of the cell is over the
		/// min value set here.
		/// </summary>
		/// <param name="minLen">
		///            the min value of the content of a cell. </param>
		int MinLengthForToggleCollapse {set;}


		/// <summary>
		/// in SLIM cell content is HTML escaped - we abstract this method to
		/// delegate to formatter the cleaning of the content.
		/// </summary>
		/// <param name="text"> the text </param>
		/// <returns> the cleaned text </returns>
		string fromRaw(string text);

	}

}