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


	public class FitnesseResultVerifier
	{

		private static string FITNESSE_RESULTS_REGEX = "<strong>Test Pages:</strong> (\\d+) right, (\\d+) wrong, (\\d+) ignored, (\\d+) exceptions.+<strong>Assertions:</strong> (\\d+) right, (\\d+) wrong, (\\d+) ignored, (\\d+) exceptions";

		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				throw new Exception("You need to pass the path to th eHTML file produced by FitNesse containing the results of test execution");
			}
			try
			{
				FitnesseResultVerifier verifier = new FitnesseResultVerifier();
				Console.WriteLine("Processing " + args[0]);
				string content = verifier.readFile(args[0]);
				Pattern p = Pattern.compile(FITNESSE_RESULTS_REGEX);
				Matcher m = p.matcher(content);
				int count = m.groupCount();
				if (count != 8)
				{
					Console.WriteLine("The file doesn't look like a result produced by FitNesse");
					Console.WriteLine("It should contain something matching:\n\t" + FITNESSE_RESULTS_REGEX);
				}
				int tRight = -1;
				int tWrong = -1;
				int tIgnored = -1;
				int tExc = -1;
				int aRight = -1;
				int aWrong = -1;
				int aIgnored = -1;
				int aExc = -1;

				bool found = m.find();

				if (!found)
				{
					Console.WriteLine("Unable to find tests result string matching " + FITNESSE_RESULTS_REGEX);
				}
				else
				{
					tRight = verifier.toInt("Tests right", m.group(1));
					tWrong = verifier.toInt("Tests wrong", m.group(2));
					tIgnored = verifier.toInt("Tests ignored", m.group(3));
					tExc = verifier.toInt("Tests exceptions", m.group(4));
					aRight = verifier.toInt("Assertions right", m.group(5));
					aWrong = verifier.toInt("Assertions wrong", m.group(6));
					aIgnored = verifier.toInt("Assertions ignored", m.group(7));
					aExc = verifier.toInt("Assertions exceptions", m.group(8));

					Console.WriteLine("Results:");
					Console.WriteLine("\tTests right:" + tRight);
					Console.WriteLine("\tTests wrong:" + tWrong);
					Console.WriteLine("\tTests ignored:" + tIgnored);
					Console.WriteLine("\tTests exceptions:" + tExc);
					Console.WriteLine("\tAssertions right:" + aRight);
					Console.WriteLine("\tAssertions wrong:" + aWrong);
					Console.WriteLine("\tAssertions ignored:" + aIgnored);
					Console.WriteLine("\tAssertions exceptions:" + aExc);
				}
				Environment.Exit(tWrong + tExc);
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception when processing file " + args[0]);
				e.printStackTrace(System.out);
				Environment.Exit(-1);
			}
		}

		private int toInt(string text, string num)
		{
			try
			{
				return int.Parse(num);
			}
			catch (System.FormatException)
			{
				Console.WriteLine(text + " is not a number: " + num);
				return -1;
			}
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

	}

}