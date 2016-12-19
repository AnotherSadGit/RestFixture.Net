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


	/// <summary>
	/// Formatter of cells handled by Slim.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class SlimFormatter : CellFormatter<string>
	{

		private int minLenForToggle = -1;
		private bool displayActual;
		private bool displayAbsoluteURLInFull;

		public SlimFormatter()
		{
		}

		public override bool DisplayActual
		{
			set
			{
				this.displayActual = value;
			}
			get
			{
				return displayActual;
			}
		}

		public override bool DisplayAbsoluteURLInFull
		{
			set
			{
				this.displayAbsoluteURLInFull = value;
			}
		}


		public override int MinLengthForToggleCollapse
		{
			set
			{
				this.minLenForToggle = value;
			}
		}


		public override void exception(CellWrapper<string> cell, string exceptionMessage)
		{
			cell.body("error:" + Tools.wrapInDiv(exceptionMessage));
		}

		public override void exception(CellWrapper<string> cell, Exception exception)
		{
			System.IO.MemoryStream @out = new System.IO.MemoryStream();
			PrintStream ps = new PrintStream(@out);
			exception.printStackTrace(ps);
			//String m = Tools.toHtml(cell.getWrapped() + "\n-----\n") + Tools.toCode(Tools.toHtml(out.toString()));
			string m = Tools.toHtml(cell.Wrapped + "\n-----\n") + Tools.toCode(Tools.toHtml(@out.ToString()));
			cell.body("error:" + Tools.wrapInDiv(m));
			//cell.body("error:" + m);
		}

		public override void check(CellWrapper<string> expected, RestDataTypeAdapter actual)
		{
			if (null == expected.body() || "".Equals(expected.body()))
			{
				if (actual.get() == null)
				{
					return;
				}
				else
				{
					expected.body(gray(actual.get().ToString()));
					return;
				}
			}

			if (actual.get() != null && actual.Equals(expected.body(), actual.get().ToString()))
			{
				right(expected, actual);
			}
			else
			{
				wrong(expected, actual);
			}
		}

		public override string label(string @string)
		{
			return Tools.toHtmlLabel(@string);
		}

		public override void wrong(CellWrapper<string> expected, RestDataTypeAdapter ta)
		{
			string expectedContent = expected.body();
			expected.body(Tools.makeContentForWrongCell(expectedContent, ta, this, minLenForToggle));
			expected.body("fail:" + Tools.wrapInDiv(expected.body()));
		}

		public override void right(CellWrapper<string> expected, RestDataTypeAdapter typeAdapter)
		{
			expected.body("pass:" + Tools.wrapInDiv(Tools.makeContentForRightCell(expected.body(), typeAdapter, this, minLenForToggle)));
		}

		public override string gray(string @string)
		{
			return "report:" + Tools.wrapInDiv(Tools.toHtml(@string));
		}

		public override void asLink(CellWrapper<string> cell, string resolvedUrl, string link, string text)
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
			cell.body("report:" + Tools.wrapInDiv(Tools.toHtmlLink(link, actualText)));
		}

		public override string fromRaw(string text)
		{
			return Tools.fromHtml(text);
		}

	}

}