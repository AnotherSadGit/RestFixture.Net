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

using System.IO;
using fit;

namespace FitNesseTestServer.Test.FitNesse.Fixture
{
	/// <summary>
	/// Action fixture to support file upload CATs in RestFixture.
	/// 
	/// @author fabrizio
	/// 
	/// </summary>
    public class FileUtilFixture : fit.Fixture
	{

		private string fileContents;

		private string fileName;

		public virtual void content(string fileContents)
		{
			this.fileContents = fileContents;
		}

		public virtual void name(string fileName)
		{
			this.fileName = fileName;
		}

		public virtual bool create()
		{
		    string directoryName = Path.GetDirectoryName(fileName);
            // Won't throw an error if the directory already exists.
		    Directory.CreateDirectory(directoryName);

            // Will overwrite an existing file, without error.
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.Write(fileContents);
            }	
			return true;
		}

		public virtual bool delete()
		{
			if (System.IO.Directory.Exists(fileName))
			{
			    Directory.Delete(fileName, true); 
			}
            else
			{
			    File.Delete(fileName);
			}
			return true;
		}

		public virtual bool exists()
		{
			return System.IO.Directory.Exists(fileName) || System.IO.File.Exists(fileName);
		}
	}

}