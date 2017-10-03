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
namespace RestFixture.Net.Javascript
{

	/// <summary>
	/// Signals an error in the evaluation of the JavaScript in LetBodyJsHandler.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class JavascriptException : Exception
	{

		private const long serialVersionUID = 1L;

		/// <param name="message"> the exception message. </param>
		public JavascriptException(string message) : base(message)
		{
		}

		public JavascriptException(string message, Exception t) : base(message, t)
		{
		}
	}

}