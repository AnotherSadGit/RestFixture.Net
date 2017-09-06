using System;
using System.Collections.Generic;
using System.Text;
using restFixture.Net.Support;
using restFixture.Net.TableElements;
using restFixture.Net.TypeAdapters;

namespace restFixture.Net.Tools
{
    public sealed class HtmlTools
    {
        /// <summary>
        /// <table border="1">
        /// <caption>Substitutions</caption>
        /// <tr>
        /// <td><code>&lt;pre&gt;</code> and <code>&lt;/pre&gt;</code></td>
        /// <td><code>""</code></td>
        /// </tr>
        /// <tr>
        /// <td><code>&lt;</code></td>
        /// <td><code>&amp;lt;</code></td>
        /// </tr>
        /// <tr>
        /// <td><code>\n</code></td>
        /// <td><code>&lt;br /&gt;</code></td>
        /// </tr>
        /// <tr>
        /// <td><code>&nbsp;</code> <i>(space)</i></td>
        /// <td><code>&amp;nbsp;</code></td>
        /// </tr>
        /// <tr>
        /// <td><code>-----</code> <i>(5 hyphens)</i></td>
        /// <td><code>&lt;hr /&gt;</code></td>
        /// </tr>
        /// </table>
        /// </summary>
        /// <param name="text"> some text. </param>
        /// <returns> the html. </returns>
        public static string toHtml(string text)
        {
            return
                text.Replace("<pre>", "")
                    .Replace("</pre>", "")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .RegexReplace(@"\r\n?|\n", "<br/>")
                    .Replace("\t", "    ")
                    .Replace(" ", "&nbsp;")
                    .Replace("-----", "<hr/>");
        }

        /// <param name="c"> some text </param>
        /// <returns> the text within <code>&lt;code&gt;</code> tags. </returns>
        public static string toCode(string c)
        {
            return "<code>" + c + "</code>";
        }

        /// <param name="somethingWithinATag"> some text enclosed in some html tag. </param>
        /// <returns> the text within the tag. </returns>
        public static string fromSimpleTag(string somethingWithinATag)
        {
            return somethingWithinATag.RegexReplace("<[^>]+>", "").RegexReplace("</[^>]+>", "");
        }

        /// <param name="text"> some html </param>
        /// <returns> the text stripped out of all tags. </returns>
        public static string fromHtml(string text)
        {
            string ls = "\n";
            return
                text.RegexReplace("<br[\\s]*/>", ls)
                    .RegexReplace("<BR[\\s]*/>", ls)
                    .RegexReplace("<span[^>]*>", "")
                    .Replace("</span>", "")
                    .Replace("<pre>", "")
                    .Replace("</pre>", "")
                    .Replace("&nbsp;", " ")
                    .Replace("&gt;", ">")
                    .Replace("&amp;", "&")
                    .Replace("&lt;", "<")
                    .Replace("&nbsp;", " ");
        }

        /// <param name="string"> a string </param>
        /// <returns> the string htmlified as a fitnesse label. </returns>
        public static string toHtmlLabel(string @string)
        {
            return "<i><span class='fit_label'>" + @string + "</span></i>";
        }

        /// <param name="href"> a string ending up in the anchor href. </param>
        /// <param name="text"> a string within anchors. </param>
        /// <returns> the string htmlified as a html link. </returns>
        public static string toHtmlLink(string href, string text)
        {
            return "<a href='" + href + "'>" + text + "</a>";
        }

        public static string wrapInDiv(string body)
        {
            return String.Format("<div>{0}</div>", body);
        }

        /// <param name="message"> the message to be included in the collapsable section header. </param>
        /// <param name="content"> the content collapsed. </param>
        /// <returns> a string with the html/js code to implement a collapsable section
        /// in fitnesse. </returns>
        public static string makeToggleCollapseable(string message, string content)
        {
            Random random = new Random();
            // The Java implementation used random.nextLong but the .NET Random class only has 
            //  Next, which returns an int.
            string id = Convert.ToString(content.GetHashCode()) + Convert.ToString(random.Next());
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='collapsible closed'>");
            sb.Append(
                "<ul><li><a href='#' class='expandall'>Expand</a></li><li><a href='#' class='collapseall'>Collapse</a></li></ul>");
            sb.Append("<p class='title'>").Append(message).Append("</p>");
            sb.Append("<div>").Append(content).Append("</div>");
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <param name="expected">        the expected value </param>
        /// <param name="typeAdapter">     the body adapter for the cell </param>
        /// <param name="formatter">       the formatter </param>
        /// <param name="minLenForToggle"> the value determining whether the content should be rendered
        ///                        as a collapseable section. </param>
        /// <returns> the formatted content for a cell with a wrong expectation </returns>
        public static string makeContentForWrongCell<T1>(string expected, RestDataTypeAdapter typeAdapter,
            ICellFormatter<T1> formatter, int minLenForToggle)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HtmlTools.toHtml(expected));
            if (formatter.DisplayActual)
            {
                sb.Append(HtmlTools.toHtml("\n"));
                sb.Append(formatter.label("expected"));
                string actual = typeAdapter.ToString();
                sb.Append(HtmlTools.toHtml("-----"));
                sb.Append(HtmlTools.toHtml("\n"));
                if (minLenForToggle >= 0 && actual.Length > minLenForToggle)
                {
                    sb.Append(makeToggleCollapseable("toggle actual", HtmlTools.toHtml(actual)));
                }
                else
                {
                    sb.Append(HtmlTools.toHtml(actual));
                }
                sb.Append(HtmlTools.toHtml("\n"));
                sb.Append(formatter.label("actual"));
            }
            IReadOnlyList<string> errors = typeAdapter.Errors;
            if (errors.Count > 0)
            {
                sb.Append(HtmlTools.toHtml("-----"));
                sb.Append(HtmlTools.toHtml("\n"));
                foreach (string e in errors)
                {
                    sb.Append(HtmlTools.toHtml(e + "\n"));
                }
                sb.Append(HtmlTools.toHtml("\n"));
                sb.Append(formatter.label("errors"));
            }
            return sb.ToString();
        }

        /// <param name="expected">        the expected value </param>
        /// <param name="typeAdapter">     the body type adaptor </param>
        /// <param name="formatter">       the formatter
        ///                        the value determining whether the content should be rendered
        ///                        as a collapseable section. </param>
        /// <param name="minLenForToggle"> the value determining whether the content should be rendered
        ///                        as a collapseable section. </param>
        /// <returns> the formatted content for a cell with a right expectation </returns>
        public static string makeContentForRightCell<T1>(string expected, RestDataTypeAdapter typeAdapter,
            ICellFormatter<T1> formatter, int minLenForToggle)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HtmlTools.toHtml(expected));
            string actual = typeAdapter.ToString();
            if (formatter.DisplayActual && !expected.Equals(actual))
            {
                sb.Append(HtmlTools.toHtml("\n"));
                sb.Append(formatter.label("expected"));
                sb.Append(HtmlTools.toHtml("-----"));
                sb.Append(HtmlTools.toHtml("\n"));
                if (minLenForToggle >= 0 && actual.Length > minLenForToggle)
                {
                    sb.Append(makeToggleCollapseable("toggle actual", HtmlTools.toHtml(actual)));
                }
                else
                {
                    sb.Append(HtmlTools.toHtml(actual));
                }
                sb.Append(HtmlTools.toHtml("\n"));
                sb.Append(formatter.label("actual"));
            }
            return sb.ToString();
        }
    }
}