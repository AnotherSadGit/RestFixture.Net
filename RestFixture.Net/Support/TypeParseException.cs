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
namespace restFixture.Net.Support
{
    /// <summary>
    /// Represents an error that occurred while using a specified type to parse a string.
    /// </summary>
    /// <remarks>Based on the Java FitNesse class CouldNotParseFitFailureException.</remarks>
    public class TypeParseException : ApplicationException
    {
        public TypeParseException(string text, Type type)
            : base(string.Format("Could not parse: '{0}', expected type: {1}.",
                text, type.FullName))
        {}

        public TypeParseException(string errorMessage) : base(errorMessage)
        {}
    }
}