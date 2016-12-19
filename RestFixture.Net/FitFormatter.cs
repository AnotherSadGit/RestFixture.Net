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
namespace RestFixture.Net
{

	using CellFormatter = smartrics.rest.fitnesse.fixture.support.CellFormatter;
	using CellWrapper = smartrics.rest.fitnesse.fixture.support.CellWrapper;
	using RestDataTypeAdapter = smartrics.rest.fitnesse.fixture.support.RestDataTypeAdapter;
	using Tools = smartrics.rest.fitnesse.fixture.support.Tools;
	using ActionFixture = fit.ActionFixture;
	using Parse = fit.Parse;
	using FitFailureException = fit.exception.FitFailureException;

	/// <summary>
	/// Cell formatter for the Fit runner.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class FitFormatter : CellFormatter<Parse>
	{

		private ActionFixture fixture;
		private bool displayActual;
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

		public override bool DisplayActual
		{
			get
			{
				return displayActual;
			}
			set
			{
				this.displayActual = value;
			}
		}

		public override int MinLengthForToggleCollapse
		{
			set
			{
				this.minLenForToggle = value;
			}
		}


		public override bool DisplayAbsoluteURLInFull
		{
			set
			{
				this.displayAbsoluteURLInFull = value;
			}
		}

		public override void exception(CellWrapper<Parse> cell, string exceptionMessage)
		{
			Parse wrapped = cell.Wrapped;
			fixture.exception(wrapped, new FitFailureException(exceptionMessage));
		}

		public override void exception(CellWrapper<Parse> cell, Exception exception)
		{
			Parse wrapped = cell.Wrapped;
			fixture.exception(wrapped, exception);
		}

		public override void check(CellWrapper<Parse> valueCell, RestDataTypeAdapter adapter)
		{
			valueCell.body(Tools.toHtml(valueCell.body()));
			fixture.check(valueCell.Wrapped, adapter);
		}

		public override string label(string @string)
		{
			return ActionFixture.label(@string);
		}

		public override void wrong(CellWrapper<Parse> expected, RestDataTypeAdapter typeAdapter)
		{
			string expectedContent = expected.body();
			string body = Tools.makeContentForWrongCell(expectedContent, typeAdapter, this, minLenForToggle);
			expected.body(body);
			fixture.wrong(expected.Wrapped);
		}

		public override void right(CellWrapper<Parse> expected, RestDataTypeAdapter typeAdapter)
		{
			string expectedContent = expected.body();
			expected.body(Tools.makeContentForRightCell(expectedContent, typeAdapter, this, minLenForToggle));
			fixture.right(expected.Wrapped);
		}

		public override string gray(string @string)
		{
			return ActionFixture.gray(Tools.toHtml(@string));
		}

		public override void asLink(CellWrapper<Parse> cell, string resolvedUrl, string link, string text)
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

		public override string fromRaw(string text)
		{
			return text;
		}
	}

}