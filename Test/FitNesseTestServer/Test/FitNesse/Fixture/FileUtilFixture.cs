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
namespace FitNesseTestServer.Test.FitNesse.Fixture
{


	using ActionFixture = fit.ActionFixture;

	/// <summary>
	/// Action fixture to support file upload CATs in RestFixture.
	/// 
	/// @author fabrizio
	/// 
	/// </summary>
	public class FileUtilFixture : ActionFixture
	{

		private string fileContents;

		private string fileName;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileUtilFixture() throws Exception
		public FileUtilFixture() : base()
		{
		}

		public virtual void content(string fileContents)
		{
			this.fileContents = fileContents;
		}

		public virtual void name(string fileName)
		{
			this.fileName = fileName;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean create() throws Exception
		public virtual bool create()
		{
			File f = new File(fileName);
			int pos = f.Path.IndexOf(f.Name);
			System.IO.Directory.CreateDirectory(f.Path.substring(0, pos));
			System.IO.StreamWriter fw = new System.IO.StreamWriter(f);
			fw.Write(fileContents);
			fw.Close();
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean delete() throws Exception
		public virtual bool delete()
		{
			if (System.IO.Directory.Exists(fileName)) System.IO.Directory.Delete(fileName, true); else System.IO.File.Delete(fileName);
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean exists() throws Exception
		public virtual bool exists()
		{
			return System.IO.Directory.Exists(fileName) || System.IO.File.Exists(fileName);
		}
	}

}