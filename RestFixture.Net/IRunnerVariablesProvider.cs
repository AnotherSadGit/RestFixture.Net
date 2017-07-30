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
namespace restFixture.Net
{
	/// <summary>
	/// The fixture provides the variables of the runner.
	/// This interface abstracts the fixture so that it can
	/// be tested easily.
	/// </summary>
	public interface IRunnerVariablesProvider
	{
		/// <summary>
		/// Get a variable store linked to the current runner environment.
		/// </summary>
		/// <returns> the variables </returns>
		Variables.Variables CreateRunnerVariables();
	}

}