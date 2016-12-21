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
	/// Wraps a REST request multipart object used in RestRequest}.
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

		private RestMultipartType type;
		private string value;
		private string contentType;
		private string charset;

		/// 
		/// <param name="type"> Type of the stringValue </param>
		/// <param name="stringValue"> The String content or the file Path </param>
		public RestMultipart(RestMultipartType type, string stringValue) : this(type, stringValue, (string)null, (string)null)
		{
		}

		public RestMultipart(RestMultipartType type, string stringValue, string contentType) : this(type, stringValue, contentType, (string)null)
		{
		}

		public RestMultipart(RestMultipartType type, string stringValue, string contentType, string charset)
		{
			this.type = type;
			this.value = stringValue;
			this.contentType = contentType;
			this.charset = charset;
		}

		/// <returns> the upload file name for this request </returns>
		public virtual string Value
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// Sets the multipart upload file name for this request.
		/// </summary>
		/// <param name="value">
		///            the multipart file name </param>
		/// <returns> this restMultipart </returns>
		public virtual RestMultipart setValue(string value)
		{
			this.value = value;
			return this;
		}

		/// <returns> the Content Type for upload file name for this request </returns>
		public virtual string ContentType
		{
			get
			{
				return contentType;
			}
		}

		/// <summary>
		/// Sets the content type of multipart upload file name for this request.
		/// </summary>
		/// <param name="contentType">
		///            the content type </param>
		/// <returns> this restMultipart </returns>
		public virtual RestMultipart setContentType(string contentType)
		{
			this.contentType = contentType;
			return this;

		}

		/// <returns> the Charset for upload file name for this request </returns>
		public virtual string Charset
		{
			get
			{
				return charset;
			}
		}

		/// <summary>
		/// Sets the charset of multipart upload file name for this request.
		/// </summary>
		/// <param name="charset">
		///            the charset </param>
		/// <returns> this restMultipart </returns>

		public virtual RestMultipart setCharset(string charset)
		{
			this.charset = charset;
			return this;
		}

		public virtual RestMultipartType Type
		{
			get
			{
				return type;
			}
			set
			{
				this.type = value;
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

			if (!value.Equals(that.value))
			{
				return false;
			}
			if (!string.ReferenceEquals(contentType, null) ?!contentType.Equals(that.contentType) :!string.ReferenceEquals(that.contentType, null))
			{
				return false;
			}
			return !string.ReferenceEquals(charset, null) ? charset.Equals(that.charset) : string.ReferenceEquals(that.charset, null);

		}



		public override int GetHashCode()
		{
			int result = value.GetHashCode();
			result = 31 * result + (!string.ReferenceEquals(contentType, null) ? contentType.GetHashCode() : 0);
			result = 31 * result + (!string.ReferenceEquals(charset, null) ? charset.GetHashCode() : 0);
			return result;
		}


		public override string ToString()
		{
			return "RestMultipart{" + "value='" + value + '\'' + ", contentType='" + contentType + '\'' + ", charset='" + charset + '\'' + '}';
		}

	}

}