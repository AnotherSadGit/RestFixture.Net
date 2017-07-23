//using System;
//using System.Collections.Generic;
//using System.Text;

///*  Copyright 2017 Simon Elms
// *
// *  This file is part of RestFixture.Net
// *
// *  RestFixture.Net is free software:
// *  You can redistribute it and/or modify it under the terms of the
// *  GNU Lesser General Public License as published by the Free Software Foundation,
// *  either version 3 of the License, or (at your option) any later version.
// *
// *  RestFixture.Net is distributed in the hope that it will be useful,
// *  but WITHOUT ANY WARRANTY; without even the implied warranty of
// *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// *  GNU Lesser General Public License for more details.
// *
// *  You should have received a copy of the GNU Lesser General Public License
// *  along with RestFixture.Net.  If not, see <http://www.gnu.org/licenses/>.
// */
//namespace RestFixture.Net
//{


//    using Log = org.apache.commons.logging.Log;
//    using LogFactory = org.apache.commons.logging.LogFactory;

//    using HtmlTag = fitnesse.html.HtmlTag;
//    using Matcher = fitnesse.wikitext.parser.Matcher;
//    using Maybe = fitnesse.wikitext.parser.Maybe;
//    using Parser = fitnesse.wikitext.parser.Parser;
//    using Rule = fitnesse.wikitext.parser.Rule;
//    using Symbol = fitnesse.wikitext.parser.Symbol;
//    using SymbolProvider = fitnesse.wikitext.parser.SymbolProvider;
//    using SymbolType = fitnesse.wikitext.parser.SymbolType;
//    using Translation = fitnesse.wikitext.parser.Translation;
//    using Translator = fitnesse.wikitext.parser.Translator;

//    /// <summary>
//    /// Allows inclusion of an SVG file in a FitNesse page. For the file to be
//    /// reachable over HTTP in FitNesse, it needs to be store under (a directory in)
//    /// <code><i>FitNesseRoot</i>/files</code>.
//    /// 
//    /// For the symbol to be deployed, put the RestFixture.jar in the FitNesse start
//    /// command (eg:
//    /// <code>java -classpath <i>path_to</i>/RestFixture.jar -jar fitnesse.jar</code>
//    /// ) and add to the plugins.properties file the following property:
//    /// <code>SymbolTypes=smartrics.rest.fitnesse.fixture.SvgImage</code>. See <a
//    /// href="http://fitnesse.org/FitNesse.UserGuide.FixtureGallery.ImportantConcepts.FixtureSymbols">this</a> for more
//    /// details.
//    /// 
//    /// To use the symbol, is sufficient to specify in a line in the test page the
//    /// following:
//    /// 
//    /// <code>!svg <i>/files/path/to/image.svg rendering_mode</i></code>
//    /// 
//    /// The <code><i>rendering_mode</i></code> is one of the following:
//    /// 
//    /// <table border="1">
//    /// <caption>modes</caption>
//    /// <tr>
//    /// <td>inline</td>
//    /// <td>the image is included as is, hence using the &lt;svg&gt; tag</td>
//    /// </tr>
//    /// <tr>
//    /// <td>embed</td>
//    /// <td>the image is rendered using the &lt;embed&gt; tag</td>
//    /// </tr>
//    /// <tr>
//    /// <td>object</td>
//    /// <td>the image is rendered using the &lt;object&gt; tag</td>
//    /// </tr>
//    /// <tr>
//    /// <td>img</td>
//    /// <td>the image is rendered using the &lt;img&gt; tag</td>
//    /// </tr>
//    /// <tr>
//    /// <td>iframe</td>
//    /// <td>the image is rendered using the &lt;iframe&gt; tag</td>
//    /// </tr>
//    /// <tr>
//    /// <td>anchor</td>
//    /// <td>the image is rendered using the &lt;a&gt; tag</td>
//    /// </tr>
//    /// </table>
//    /// Each mode (except <code>inline</code>) will point to the file in the fitnesse
//    /// server.
//    /// 
//    /// Basically this means that when embed is used, the conent of the SVG file is
//    /// read at that point in time and substituted as the page is rendering. With the
//    /// others, an HTML tag is rendered that has a src that points to the remote
//    /// file, hence the image is rendered when the browser decides to.
//    /// 
//    /// Also note that <b>rendering SVG files embedded in HTML pages is one of the
//    /// least supported features across all browsers. So the end result of embedding
//    /// an SVG is highly dependant on the browser and unluckely to be portable.</b>
//    /// 
//    /// @author fabrizio
//    /// 
//    /// </summary>
//    public class SvgImage : SymbolType, Rule, Translation
//    {

//        internal static readonly Log LOG = LogFactory.getLog(typeof(SvgImage));

//        /// <summary>
//        /// The selected rendering mode.
//        /// </summary>
//        private sealed class Mode
//        {
//            public static readonly Mode inline = new Mode("inline", InnerEnum.inline);
//            public static readonly Mode embed = new Mode("embed", InnerEnum.embed, "embed", "src", "type=\"image/svg+xml\"");
//            public static readonly Mode @object = new Mode("@object", InnerEnum.@object, "object", "data", "type=\"image/svg+xml\"");
//            public static readonly Mode iframe = new Mode("iframe", InnerEnum.iframe, "iframe", "src");
//            public static readonly Mode img = new Mode("img", InnerEnum.img, "img", "src");
//            public static readonly Mode anchor = new Mode("anchor", InnerEnum.anchor, "a", "href");

//            private static readonly IList<Mode> valueList = new List<Mode>();

//            static Mode()
//            {
//                valueList.Add(inline);
//                valueList.Add(embed);
//                valueList.Add(@object);
//                valueList.Add(iframe);
//                valueList.Add(img);
//                valueList.Add(anchor);
//            }

//            public enum InnerEnum
//            {
//                inline,
//                embed,
//                @object,
//                iframe,
//                img,
//                anchor
//            }

//            private readonly string nameValue;
//            private readonly int ordinalValue;
//            private readonly InnerEnum innerEnumValue;
//            private static int nextOrdinal = 0;

//            public string ToString(string stuff)
//            {
//                if (this.Equals(inline))
//                {
//                    return stuff;
//                }
//                string s = "";
//                if (otherAttr != null)
//                {
//                    s = otherAttr;
//                }
//                return "<" + tag + " " + srcAttr + "=\"" + stuff + "\" " + s + " />";
//            }

//            internal Mode(string name, InnerEnum innerEnum,) : this(null, null, null)
//            {

//                nameValue = name;
//                ordinalValue = nextOrdinal++;
//                innerEnumValue = innerEnum;
//            }

//            internal Mode(string name, InnerEnum innerEnum, string tag, string srcAttr) : this(tag, srcAttr, null)
//            {

//                nameValue = name;
//                ordinalValue = nextOrdinal++;
//                innerEnumValue = innerEnum;
//            }

//            internal Mode(string name, InnerEnum innerEnum, string tag, string srcAttr, string otherAttr)
//            {
//                this.tag = tag;
//                this.srcAttr = srcAttr;
//                this.otherAttr = otherAttr;

//                nameValue = name;
//                ordinalValue = nextOrdinal++;
//                innerEnumValue = innerEnum;
//            }

//            internal string tag;
//            internal string srcAttr;
//            internal string otherAttr;

//            public static IList<Mode> values()
//            {
//                return valueList;
//            }

//            public InnerEnum InnerEnumValue()
//            {
//                return innerEnumValue;
//            }

//            public int ordinal()
//            {
//                return ordinalValue;
//            }

//            public static Mode valueOf(string name)
//            {
//                foreach (Mode enumInstance in Mode.values())
//                {
//                    if (enumInstance.nameValue == name)
//                    {
//                        return enumInstance;
//                    }
//                }
//                throw new System.ArgumentException(name);
//            }
//        }

//        private Mode defaultMode = Mode.inline;

//        /// <summary>
//        /// ctor.
//        /// </summary>
//        public SvgImage() : base("SVG-Image")
//        {
//            wikiMatcher((new Matcher()).@string("!svg"));
//            htmlTranslation(this);
//            wikiRule(this);
//        }

//        public virtual Maybe<Symbol> parse(Symbol current, Parser parser)
//        {
//            Symbol targetList = parser.parseToEnds(-1, SymbolProvider.pathRuleProvider, new SymbolType[] {SymbolType.Newline});
//            return new Maybe<Symbol>(current.add(targetList));
//        }

//        public virtual string toTarget(Translator translator, Symbol symbol)
//        {
//            string symContent = symbol.Content;
//            string target = symContent + translator.translate(symbol.childAt(0));
//            return toTarget(translator, target, symbol);
//        }

//        /// <param name="translator"> the translator </param>
//        /// <param name="body"> the body </param>
//        /// <param name="args"> the args </param>
//        /// <returns> the inlined svg. </returns>
//        public virtual string toTarget(Translator translator, string body, Symbol args)
//        {
//            Symbol symbol = getPathSymbol(args);
//            if (symbol == null)
//            {
//                return error("Missing image path");
//            }
//            string line = translator.translate(symbol);
//            line = line.replaceAll("\\s+", " ").Trim();
//            string[] parts = line.Split(" ", true);
//            string location = null;
//            Mode mode = defaultMode;
//            if (parts.Length > 0)
//            {
//                location = parts[0];
//            }
//            if (parts.Length > 1)
//            {
//                foreach (string part in parts)
//                {
//                    if (part.Contains("mode="))
//                    {
//                        mode = parseMode(part);
//                    }
//                }
//            }
//            return inlineSvg(mode, location);
//        }

//        private Mode parseMode(string s)
//        {
//            string mString = null;
//            Mode mode = defaultMode;
//            try
//            {
//                mString = s.Trim();
//                string[] subParts = mString.Split("=", true);
//                mode = Enum.valueOf(typeof(Mode), subParts[1].Trim());
//            }
//            catch (System.ArgumentException)
//            {
//                throw new System.ArgumentException("Mode not supported: " + mString);
//            }
//            return mode;
//        }

//        private Symbol getPathSymbol(Symbol args)
//        {
//            if (args.Children.size() > 0)
//            {
//                return args.childAt(0);
//            }
//            return null;
//        }

//        private string inlineSvg(Mode tag, string location)
//        {
//            if (location == null)
//            {
//                return error("Invalid file path: null");
//            }
//            if (tag.Equals(Mode.inline))
//            {
//                string content = read(location);
//                return tag.ToString(content);
//            }
//            else
//            {
//                return tag.ToString(location);
//            }
//        }


//        private string read(string path)
//        {
//            string loc = path.Trim();
//            if (loc.StartsWith("/files", StringComparison.Ordinal))
//            {
//                loc = "FitNesseRoot" + path.Trim();
//            }
//            string content = null;
//            File f = new File(loc);
//            try
//            {
//                if (f.exists())
//                {
//                    content = readFile(f);
//                }
//                else
//                {
//                    content = error("File not found: " + f.AbsolutePath + ", path=" + path);
//                }
//            }
//            catch (FileNotFoundException e)
//            {
//                content = error("File not found: " + loc + ", path=" + path + ", err=" + e.Message);
//            }
//            catch (Exception)
//            {
//                content = error("Unable to read: " + loc + ", path=" + path);
//            }
//            return content;
//        }

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: private String readFile(java.io.File f) throws java.io.FileNotFoundException
//        private string readFile(File f)
//        {
//            System.IO.StreamReader reader = new System.IO.StreamReader(f);
//            System.IO.StreamReader r = new System.IO.StreamReader(reader);
//            StringBuilder sb = new StringBuilder();
//            string line;
//            LOG.debug("Reading file " + f.AbsolutePath);
//            try
//            {
//                line = r.ReadLine();
//                while (line != null)
//                {
//                    sb.Append(line);
//                    line = r.ReadLine();
//                }
//            }
//            catch (IOException e)
//            {
//                throw new System.ArgumentException("Unable to read from stream", e);
//            }
//            finally
//            {
//                try
//                {
//                    r.Close();
//                }
//                catch (IOException)
//                {
//                    LOG.debug("Exception closing file reader for file " + f.AbsolutePath);
//                }
//            }
//            return sb.ToString();
//        }

//        private string error(string @string)
//        {
//            HtmlTag tag = new HtmlTag("p");
//            tag.addAttribute("style", "color:red");
//            tag.add(@string);
//            return tag.htmlInline();
//        }

//    }
//}