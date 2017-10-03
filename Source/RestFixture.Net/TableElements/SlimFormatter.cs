using System;
using RestFixture.Net.Tools;
using RestFixture.Net.TypeAdapters;

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
	/// Formatter of cells handled by Slim.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class SlimFormatter : ICellFormatter<String>
	{

		private int minLenForToggle = -1;
		private bool displayActual;
		private bool displayAbsoluteURLInFull;

		public SlimFormatter()
		{
		}

	    public bool DisplayActual
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

		public bool DisplayAbsoluteURLInFull
		{
			set
			{
				this.displayAbsoluteURLInFull = value;
			}
		}


		public int MinLengthForToggleCollapse
		{
			set
			{
				this.minLenForToggle = value;
			}
		}


        public void exception(ICellWrapper<String> cell, string exceptionMessage)
		{
			cell.body("error:" + HtmlTools.wrapInDiv(exceptionMessage));
		}

        public void exception(ICellWrapper<String> cell, Exception exception)
		{
			//String m = Tools.toHtml(cell.getWrapped() + "\n-----\n") + Tools.toCode(Tools.toHtml(out.toString()));
			string m = HtmlTools.toHtml(cell.Wrapped + "\n-----\n") + HtmlTools.toCode(HtmlTools.toHtml(exception.ToString()));
			cell.body("error:" + HtmlTools.wrapInDiv(m));
			//cell.body("error:" + m);
		}

        public void check(ICellWrapper<String> expected, RestDataTypeAdapter actual)
		{
			if (string.IsNullOrWhiteSpace(expected.body()))
			{
				if (actual.Actual == null)
				{
					return;
				}
				else
				{
					expected.body(gray(actual.Actual.ToString()));
					return;
				}
			}

			if (actual.Actual != null && actual.Equals(expected.body(), actual.Actual.ToString()))
			{
				right(expected, actual);
			}
			else
			{
				wrong(expected, actual);
			}
		}

	    public string label(string @string)
		{
			return HtmlTools.toHtmlLabel(@string);
		}

        public void wrong(ICellWrapper<String> expected, RestDataTypeAdapter ta)
		{
			string expectedContent = expected.body();
			expected.body(Tools.HtmlTools.makeContentForWrongCell(expectedContent, ta, this, minLenForToggle));
			expected.body("fail:" + HtmlTools.wrapInDiv(expected.body()));
		}

        public void right(ICellWrapper<String> expected, RestDataTypeAdapter typeAdapter)
		{
			expected.body("pass:" + HtmlTools.wrapInDiv(Tools.HtmlTools.makeContentForRightCell(expected.body(), typeAdapter, this, minLenForToggle)));
		}

		public string gray(string @string)
		{
			return "report:" + HtmlTools.wrapInDiv(HtmlTools.toHtml(@string));
		}

        public void asLink(ICellWrapper<String> cell, string resolvedUrl, string link, string text)
		{
			string actualText = text;
			string parsed = null;
			if (displayAbsoluteURLInFull)
			{
				parsed = HtmlTools.fromSimpleTag(resolvedUrl);
				if (parsed.Trim().StartsWith("http", StringComparison.Ordinal))
				{
				   actualText = parsed;
				}
			}
			cell.body("report:" + HtmlTools.wrapInDiv(HtmlTools.toHtmlLink(link, actualText)));
		}

		public string fromRaw(string text)
		{
			return HtmlTools.fromHtml(text);
		}

	}

}