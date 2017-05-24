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

	using ActionFixture = fit.ActionFixture;
	using Parse = fit.Parse;
    using FitFailureException = fitSharp.Fit.Exception.FitFailureException;

	/// <summary>
	/// Cell formatter for the Fit runner.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
    public class FitFormatter : ICellFormatter
	{

		private ActionFixture fixture;
	    private int minLenForToggle = -1;
		private bool displayAbsoluteURLInFull;

		public FitFormatter()
		{
		}

		/// <summary>
		/// sets the action fixture delegate to forward formatting messages.
		/// </summary>
		/// <param name="f"> the fixture </param>
		public virtual ActionFixture ActionFixtureDelegate
		{
			set
			{
				this.fixture = value;
			}
		}

	    public bool DisplayActual { get; set; }

	    public int MinLengthForToggleCollapse
		{
			set
			{
				this.minLenForToggle = value;
			}
		}


		public bool DisplayAbsoluteURLInFull
		{
			set
			{
				this.displayAbsoluteURLInFull = value;
			}
		}

		public void exception(ICellWrapper cell, string exceptionMessage)
		{
		    IFitCellWrapper fitCell = cell as IFitCellWrapper;
            Parse wrapped = fitCell.Wrapped;
			fixture.exception(wrapped, new FitFailureException(exceptionMessage));
		}

	    public void exception(ICellWrapper cell, Exception exception)
		{
            IFitCellWrapper fitCell = cell as IFitCellWrapper;
            Parse wrapped = fitCell.Wrapped;
			fixture.exception(wrapped, exception);
		}

        public void check(ICellWrapper valueCell, RestDataTypeAdapter adapter)
        {
            IFitCellWrapper fitCell = valueCell as IFitCellWrapper;
            fitCell.body(Tools.toHtml(fitCell.body()));
            fixture.check(fitCell.Wrapped, adapter);
        }

		public string label(string text)
		{
			return ActionFixture.label(text);
		}

	    public void wrong(ICellWrapper expected, RestDataTypeAdapter typeAdapter)
		{
            IFitCellWrapper fitCell = expected as IFitCellWrapper;
            string expectedContent = fitCell.body();
			string body = Tools.makeContentForWrongCell(expectedContent, typeAdapter, this, minLenForToggle);
            fitCell.body(body);
            fixture.wrong(fitCell.Wrapped);
		}

		public void right(ICellWrapper expected, RestDataTypeAdapter typeAdapter)
		{
            IFitCellWrapper fitCell = expected as IFitCellWrapper;
            string expectedContent = fitCell.body();
            fitCell.body(Tools.makeContentForRightCell(expectedContent, typeAdapter, this, minLenForToggle));
            fixture.right(fitCell.Wrapped);
		}

		public string gray(string text)
		{
			return ActionFixture.gray(Tools.toHtml(text));
		}

        public void asLink(ICellWrapper cell, string resolvedUrl, string link, string text)
		{
			string actualText = text;
			string parsed = null;
			if (displayAbsoluteURLInFull)
			{
				parsed = Tools.fromSimpleTag(resolvedUrl);
				if (parsed.Trim().StartsWith("http", StringComparison.Ordinal))
				{
					actualText = parsed;
				}
			}
			cell.body(Tools.toHtmlLink(link, actualText));
		}

		public string fromRaw(string text)
		{
			return text;
		}
	}

}