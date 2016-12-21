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
namespace RestClient.Data
{
	/// <summary>
	/// Wraps a REST response object
	/// </summary>
	public class RestResponse : RestData
	{
		private string statusText;
		private int? statusCode;

		/// <returns> the status code of this response </returns>
		public virtual int? StatusCode
		{
			get
			{
				return statusCode;
			}
		}

		/// <param name="sCode"> the status code for this response </param>
		/// <returns> this response </returns>
		public virtual RestResponse setStatusCode(int? sCode)
		{
			this.statusCode = sCode;
			return this;
		}

		/// <returns> the status text for this response </returns>
		public virtual string StatusText
		{
			get
			{
				return statusText;
			}
		}

		/// <param name="st"> the status text for this response </param>
		/// <returns> this response </returns>
		public virtual RestResponse setStatusText(string st)
		{
			this.statusText = st;
			return this;
		}

		/// <returns> string representation of this response </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (StatusCode != null)
			{
				builder.Append(string.Format("[{0}] {1}", this.StatusCode, this.StatusText));
			}
			builder.Append(LINE_SEPARATOR);
			builder.Append(base.ToString());
			return builder.ToString();
		}

	}

}