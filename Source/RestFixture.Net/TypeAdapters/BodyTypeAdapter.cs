using System.Collections;

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
namespace RestFixture.Net.TypeAdapters
{

	using Parse = fit.Parse;

	/// <summary>
	/// Base class for body type adaptors.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public abstract class BodyTypeAdapter : RestDataTypeAdapter
	{

		private string charset;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public BodyTypeAdapter() : base()
		{
		}

		protected internal virtual string Charset
		{
			set
			{
				this.charset = value;
			}
			get
			{
				return charset;
			}
		}


		/// <summary>
		/// Checks if body of a cell is "no-body" meaning empty in the context of a
		/// REST call.
		/// </summary>
		/// <param name="value">
		///            the cell </param>
		/// <returns> true if no-body </returns>
		protected internal virtual bool checkNoBody(object value)
		{
			if (value == null)
			{
				return true;
			}
			if (value is ICollection)
			{
				return ((ICollection)value).Count == 0;
			}
			string s = value.ToString();
			if (value is Parse)
			{
				s = ((Parse) value).Body.Trim();
			}
			return checkNoBodyForString(s);
		}

		private bool checkNoBodyForString(string value)
		{
			return "".Equals(value.Trim()) || "no-body".Equals(value.Trim());
		}

		/// <param name="content">
		///            the content of the body response to be XMLified. </param>
		/// <returns> the content as xml. </returns>
		public abstract string toXmlString(string content);

		/// <summary>
		/// This renders the actual body - expected as a String containing XML - as
		/// HTML to be displayed in the test page.
		/// </summary>
		/// <param name="obj">
		///            the {@code List<String>} actual body, or an empty/null body
		///            rendered as HTML </param>
		/// <returns> the string representation </returns>
        public override string ToString(object obj)
		{
			if (obj == null || obj.ToString().Trim().Equals(""))
			{
				return "no-body";
			}
			// the actual value is passed as an xml string
			// TODO: pretty print toString on BodyTypeAdapter
			return obj.ToString();
		}

	}

}