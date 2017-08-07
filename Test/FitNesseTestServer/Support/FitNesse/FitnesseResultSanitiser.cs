using System;
using System.Text;

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
namespace FitNesseTestServer.Support.FitNesse
{
    public class FitnesseResultSanitiser
	{

		private static string FITNESSE_CSS_TAG = "<link rel=\"stylesheet\" type=\"text/css\" href=\"/files/css/fitnesse.css\" media=\"screen\"/>";
		private static string FITNESSE_JS_TAG = "<script src=\"/files/javascript/fitnesse.js\" type=\"text/javascript\"></script>";
		private static string FITNESSE_PRINT_CSS_TAG = "<link rel=\"stylesheet\" type=\"text/css\" href=\"/files/css/fitnesse_print.css\" media=\"print\"/>";

		private string alreadyImported;

		public static void Main(string[] args)
		{
			try
			{
				if (args.Length != 2)
				{
					throw new Exception("You need to pass the file to sanitise and the location of the FitNesseRoot wiki root");
				}
				FitnesseResultSanitiser sanitiser = new FitnesseResultSanitiser();
				string content = sanitiser.readFile(args[0]);
				content = sanitiser.removeNonHtmlGarbage(content);
				content = sanitiser.removeLinksToExternalPages(content);
				if (string.ReferenceEquals(content, null))
				{
					Console.WriteLine("Invalid content in file (not an html file?): " + args[0]);
					Environment.Exit(1);
				}
				string fitnesseRootLocation = args[1];
				content = sanitiser.injectCssAndJs(fitnesseRootLocation, content);
				content = sanitiser.embedPictures(fitnesseRootLocation, content);
				string newName = sanitiser.generateNewName(args[0]);
				sanitiser.writeFile(newName, content);
				Console.WriteLine("Input file has been sanitised. Result is: " + newName);
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception when sanitising file " + args[0]);
				e.printStackTrace(System.out);
			}
		}

		private string removeLinksToExternalPages(string content)
		{
			if (string.ReferenceEquals(content, null))
			{
				return null;
			}
			content = content.Replace("<a style=\"font-size:small;\" href=\"RestFixtureTests?pageHistory\"> [history]</a>", "");
			int pos = content.IndexOf("<div id=\"execution-status\">");
			if (pos >= 0)
			{
				int endPos = content.IndexOf("</div>", pos, StringComparison.Ordinal) + 6;
				content = content.Substring(0, pos - 1) + content.Substring(endPos + 1);
			}
			return content;
		}

		private string generateNewName(string name)
		{
			int pos = name.LastIndexOf('.');
			const string suffix = "_sanitised";
			if (pos == -1)
			{
				return name + suffix;
			}
			string p0 = name.Substring(0, pos - 1);
			string p1 = name.Substring(pos);
			return p0 + suffix + p1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String writeFile(String name, String content) throws Exception
		private string writeFile(string name, string content)
		{
			System.IO.FileStream fos = null;
			try
			{
				fos = new System.IO.FileStream(name, System.IO.FileMode.Create, System.IO.FileAccess.Write);
				fos.WriteByte(content.GetBytes());
				fos.Flush();
			}
			finally
			{
				if (fos != null)
				{
					fos.Close();
				}
			}
			return name;

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String injectCssAndJs(String fitnesseRootLocation, String content) throws Exception
		private string injectCssAndJs(string fitnesseRootLocation, string content)
		{
			string replContent = doReplacement(fitnesseRootLocation, FITNESSE_CSS_TAG, "<style>\n", "\n</style>\n", content);
			replContent = doReplacement(fitnesseRootLocation, FITNESSE_PRINT_CSS_TAG, "<style>\n", "\n</style>\n", replContent);
			replContent = doReplacement(fitnesseRootLocation, FITNESSE_JS_TAG, "\n<script language='JavaScript' type='text/javascript'>\n", "\n</script>\n", replContent);
			return replContent;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String doReplacement(String filesRootLoc, String link, String pre, String post, String content) throws Exception
		private string doReplacement(string filesRootLoc, string link, string pre, string post, string content)
		{
			int len = link.Length;
			int pos = content.IndexOf(link, StringComparison.Ordinal);
			if (pos < 0)
			{
				return content;
			}
			string part1 = content.Substring(0, pos - 1);
			string part2 = content.Substring(pos + len);

			string pattern = "href=\"";
			int sPos = link.IndexOf(pattern, StringComparison.Ordinal);
			if (sPos == -1)
			{
				pattern = "src=\"";
				sPos = link.IndexOf(pattern, StringComparison.Ordinal);
			}
			if (sPos == -1)
			{
				return content;
			}
			int ePos = link.IndexOf("\"", sPos + 1 + pattern.Length);
			string resName = StringHelperClass.SubstringSpecial(link, sPos + pattern.Length, ePos);
			string fileName = filesRootLoc + resName;

			string resContent = readFile(fileName);

			if (resContent.IndexOf("@import url", StringComparison.Ordinal) >= 0)
			{
				// looks like there's a css with some import to be resolved
				resContent = resolveImport(filesRootLoc, resContent);
			}

			return part1 + pre + resContent + post + part2;

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String resolveImport(String rootLoc, String content) throws Exception
		private string resolveImport(string rootLoc, string content)
		{
			string newContent = "";
			const string pattern = "@import url";
			int sPos = content.IndexOf(pattern, StringComparison.Ordinal);
			int ePos = content.IndexOf("\n", sPos, StringComparison.Ordinal);
			if (ePos == -1)
			{
				ePos = content.Length;
			}
			string importString = content.Substring(sPos, ePos - sPos);
			int iPos = importString.IndexOf("\"");
			int lPos = importString.IndexOf("\"", iPos + 1);
			string imported = importString.Substring(iPos + 1, lPos - (iPos + 1));
			if (string.ReferenceEquals(alreadyImported, null) || !imported.Equals(alreadyImported))
			{
				string importedContent = readFile(rootLoc + imported);
				string part1 = content.Substring(0, sPos);
				string part2 = content.Substring(ePos, content.Length - ePos);
				newContent = part1 + importedContent + part2 + "\n";
				return newContent;
			}
			else
			{
				return content;
			}
		}

		private string removeNonHtmlGarbage(string content)
		{
			int pos = content.IndexOf("<html>", StringComparison.Ordinal);
			int endPos = content.IndexOf("</html>", StringComparison.Ordinal);
			if (pos > -1 && endPos > -1)
			{
				endPos = endPos + 7;
			}
			else
			{
				return null;
			}
			content = content.Substring(pos, endPos - pos);
			return content;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String readFile(String fileLocation) throws Exception
		private string readFile(string fileLocation)
		{
			System.IO.FileStream fis = null;
			try
			{
				File f = new File(fileLocation);
				if (!f.exists())
				{
					throw new Exception("File '" + f.AbsolutePath + "' doesn't exist");
				}
				fis = new System.IO.FileStream(f, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				string content = ToString(fis);
				return content;
			}
			finally
			{
				if (fis != null)
				{
					try
					{
						fis.Close();
					}
					catch (Exception)
					{
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String toString(java.io.InputStream is) throws java.io.IOException
		private string ToString(System.IO.Stream @is)
		{
			StringBuilder sb = new StringBuilder();
			sbyte[] buff = new sbyte[1024];
			while (true)
			{
				int r = @is.Read(buff, 0, buff.Length);
				if (r == -1)
				{
					break;
				}
				sb.Append(StringHelperClass.NewString(buff, 0, r));
			}
			return sb.ToString();
		}

		private string embedPictures(string filesRootLoc, string content)
		{
			bool foundAll = false;
			int posEnd = 0;
			while (!foundAll)
			{
				int pos = content.IndexOf("<img", posEnd, StringComparison.Ordinal);
				foundAll = pos < 0;
				if (!foundAll)
				{
					posEnd = content.IndexOf("/>", pos, StringComparison.Ordinal) + 1;
					string pre = content.Substring(0, pos - 1);
					string tag = content.Substring(pos, posEnd - pos).Trim();
					string post = content.Substring(posEnd + 1);
					if (post.StartsWith(" ", StringComparison.Ordinal))
					{
						post = post.Substring(1);
					}
					content = pre + generateTagWithEmbeddedImage(filesRootLoc, tag) + post;
				}
			}
			// content =
			// generateEncodedImgFromFileName("var collapsableOpenImg = \"/files/images/collapsableOpen.gif\"",
			// filesRootLoc, content);
			// content =
			// generateEncodedImgFromFileName("var collapsableClosedImg = \"/files/images/collapsableClosed.gif\"",
			// filesRootLoc, content);

			return content;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private String generateEncodedImgFromFileName(String matchingString, String filesRootLoc, String content)
		private string generateEncodedImgFromFileName(string matchingString, string filesRootLoc, string content)
		{
			int pos = content.IndexOf(matchingString, StringComparison.Ordinal);
			pos = content.IndexOf("\"", pos) + 1;
			int posEnd = content.IndexOf('"', pos);
			string pre = content.Substring(0, pos - 1);
			string fileName = content.Substring(pos, posEnd - pos);
			string post = content.Substring(posEnd + 1);
			string enc = encodeBase64(filesRootLoc + fileName);
			content = pre + enc + post;
			return content;
		}

		private string generateTagWithEmbeddedImage(string filesRootLoc, string imgTag)
		{
			int pos = imgTag.IndexOf("src=\"");
			if (pos == -1)
			{
				pos = imgTag.IndexOf("src='", StringComparison.Ordinal);
			}
			pos = pos + 5;
			int posEnd = imgTag.IndexOf("\"", pos + 1);
			if (posEnd == -1)
			{
				posEnd = imgTag.IndexOf("'", pos + 1, StringComparison.Ordinal);
			}
			string file = imgTag.Substring(pos, posEnd - pos);
			string fileName = filesRootLoc + file;
			string encoded = encodeBase64(fileName);
			if (null != encoded)
			{
				return encoded;
			}
			return imgTag;
		}

		private string encodeBase64(string fileName)
		{
			int extPos = fileName.LastIndexOf('.') + 1;
			string ext = fileName.Substring(extPos);
			try
			{
				string content = readFile(fileName);
				string encoded = new string(Base64.encodeBase64(content.GetBytes()));
				return "<img src=\"data:image/" + ext + ";base64," + encoded + "\"/>";
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.Message);
			}
			return null;
		}

	}

}