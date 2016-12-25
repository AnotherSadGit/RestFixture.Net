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
	/// Wraps a REST request multipart object used in RestRequest.
	/// </summary>
	public class RestMultipart
	{

		/// <summary>
		/// An http verb (those supported).
		/// </summary>
		public enum RestMultipartType
		{
			FILE,
			STRING
		}

		private RestMultipartType _type;
		private string _value;
		private string _contentType;
		private string _charset;

		/// 
		/// <param name="type"> Type of the stringValue </param>
		/// <param name="stringValue"> The String content or the file Path </param>
		public RestMultipart(RestMultipartType type, string stringValue) : 
            this(type, stringValue, (string)null, (string)null)
		{
		}

		public RestMultipart(RestMultipartType type, string stringValue, string contentType) : 
            this(type, stringValue, contentType, (string)null)
		{
		}

		public RestMultipart(RestMultipartType type, string stringValue, string contentType, string charset)
		{
			this._type = type;
			this._value = stringValue;
			this._contentType = contentType;
			this._charset = charset;
		}

		/// <returns> the upload file name for this request </returns>
		public virtual string Value
		{
			get
			{
				return _value;
			}
		    set { this._value = value; }
		}

		/// <returns> the Content Type for upload file name for this request </returns>
		public virtual string ContentType
		{
			get
			{
				return _contentType;
			}
		    set { _contentType = value; }
		}

		/// <returns> the Charset for upload file name for this request </returns>
		public virtual string Charset
		{
			get
			{
				return _charset;
            }
            set { _charset = value; }
		}

		public virtual RestMultipartType Type
		{
			get
			{
				return _type;
			}
			set
			{
				this._type = value;
			}
		}


		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (!(o is RestMultipart))
			{
				return false;
			}

			RestMultipart that = (RestMultipart) o;

			if (!_value.Equals(that._value))
			{
				return false;
			}
			if (!string.ReferenceEquals(_contentType, null) ?!_contentType.Equals(that._contentType) :!string.ReferenceEquals(that._contentType, null))
			{
				return false;
			}
			return !string.ReferenceEquals(_charset, null) ? _charset.Equals(that._charset) : string.ReferenceEquals(that._charset, null);

		}



		public override int GetHashCode()
		{
			int result = _value.GetHashCode();
			result = 31 * result + (!string.ReferenceEquals(_contentType, null) ? _contentType.GetHashCode() : 0);
			result = 31 * result + (!string.ReferenceEquals(_charset, null) ? _charset.GetHashCode() : 0);
			return result;
		}


		public override string ToString()
		{
			return "RestMultipart{" + "value='" + _value + '\'' + ", contentType='" + _contentType + '\'' + ", charset='" + _charset + '\'' + '}';
		}

	}

}