using System;
using System.Collections.Generic;
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
	/// Wraps a REST request object.
	/// </summary>
	public class RestRequest : RestData
	{
		/// <summary>
		/// An http verb (those supported).
		/// </summary>
		public enum Method
		{
			Get,
			Post,
			Put,
			Delete,
			Head,
			Options,
			Trace
		}

		private const string FILE = "file";
		private string fileName;
		[Obsolete]
		private string multipartFileName;
		[Obsolete]
		private string multipartFileParameterName = FILE;
		private IDictionary<string, RestMultipart> multipartFileByParamName = new LinkedHashMap<string, RestMultipart>();
		private string query;
		private Method method;
		private bool followRedirect = true;
		private bool resourceUriEscaped = false;

		/// <returns> the method for this request </returns>
		public virtual Method getMethod()
		{
			return method;
		}

		/// <summary>
		/// Sets the method for this request.
		/// </summary>
		/// <param name="method">
		///            the method </param>
		/// <returns> this request </returns>
		/// <seealso cref= smartrics.rest.client.RestRequest.Method </seealso>
		public virtual RestRequest setMethod(Method method)
		{
			this.method = method;
			return this;
		}

		/// <returns> the query for this request </returns>
		public virtual string Query
		{
			get
			{
				return query;
			}
		}

		/// <summary>
		/// Sets the query for this request.
		/// </summary>
		/// <param name="query">
		///            the query </param>
		/// <returns> this request </returns>
		public virtual RestRequest setQuery(string query)
		{
			this.query = query;
			return this;
		}

		/// <returns> the upload file name for this request </returns>
		public virtual string FileName
		{
			get
			{
				return fileName;
			}
		}

	//    /**
	//     * @return the multipart upload file name for this request
	//     */
	//    public String getMultipartFileName() {
	//        return multipartFileName;
	//    }

		/// <returns> the multipart upload files name for this request </returns>
		public virtual IDictionary<string, RestMultipart> MultipartFileNames
		{
			get
			{
				// Add History Api
				if ((!string.ReferenceEquals(this.multipartFileName, null)) && (this.multipartFileName.Trim().Length > 0))
				{
					RestMultipart restMultipart = new RestMultipart(RestMultipart.RestMultipartType.FILE, this.multipartFileName);
					this.addMultipart(this.multipartFileParameterName, restMultipart);
				}
				// Return the Map
				return multipartFileByParamName;
			}
		}

		/// <returns> the multipart file form parameter name for this request </returns>
		[Obsolete]
		public virtual string MultipartFileParameterName
		{
			get
			{
				return multipartFileParameterName;
			}
		}

		/// <summary>
		/// Sets the multipart file form parameter name for this request.
		/// </summary>
		/// <param name="multipartFileParameterName">
		///            the multipart file form parameter name </param>
		/// <returns> this request </returns>
		[Obsolete]
		public virtual RestRequest setMultipartFileParameterName(string multipartFileParameterName)
		{
			this.multipartFileParameterName = multipartFileParameterName;
			return this;
		}

		/// <summary>
		/// Sets the multipart upload file name for this request.
		/// </summary>
		/// <param name="multipartFileName">
		///            the multipart file name </param>
		/// <returns> this request </returns>
		[Obsolete]
		public virtual RestRequest setMultipartFileName(string multipartFileName)
		{
			this.multipartFileName = multipartFileName;
			return this;
		}


		/// <summary>
		/// Add the multipart upload file name for this request.
		/// </summary>
		/// <param name="multipartFileName">
		///            the multipart file name </param>
		/// <returns> this request </returns>
		public virtual RestRequest addMultipartFileName(string multipartFileName)
		{
			RestMultipart restMultipart = new RestMultipart(RestMultipart.RestMultipartType.FILE, multipartFileName);
			return this.addMultipart(FILE, restMultipart);
		}

		/// <summary>
		/// Add the multipart upload file name for this request.
		/// </summary>
		/// <param name="multipartFileName">
		///            the multipart file name </param>
		/// <param name="contentType">
		///            the multipart contentType </param>
		/// <returns> this request </returns>
		public virtual RestRequest addMultipartFileName(string multipartFileName, string contentType)
		{
			RestMultipart restMultipart = new RestMultipart(RestMultipart.RestMultipartType.FILE,multipartFileName, contentType);
			return this.addMultipart(FILE, restMultipart);
		}


		/// <summary>
		/// Add the multipart upload file name for this request.
		/// </summary>
		/// <param name="multipartFileName">
		///            the multipart file name </param>
		/// <param name="contentType">
		///            the multipart contentType </param>
		/// <param name="charSet">
		///            the multipart charSet </param>
		/// <returns> this request </returns>
		public virtual RestRequest addMultipartFileName(string multipartFileName, string contentType, string charSet)
		{
			RestMultipart restMultipart = new RestMultipart(RestMultipart.RestMultipartType.FILE, multipartFileName, contentType, charSet);
			return this.addMultipart(FILE, restMultipart);
		}

		/// <summary>
		/// Add the multipart upload file name for this request.
		/// </summary>
		/// <param name="multiParamName">
		///            the multipart file form parameter name </param>
		/// <param name="restMultipart">
		///            the multipart restMultipart data </param>
		/// <returns> this request </returns>
		public virtual RestRequest addMultipart(string multiParamName, RestMultipart restMultipart)
		{
			multipartFileByParamName[multiParamName] = restMultipart;
			return this;
		}

		public virtual RestRequest setFollowRedirect(bool v)
		{
			this.followRedirect = v;
			return this;
		}

		public virtual bool FollowRedirect
		{
			get
			{
				return followRedirect;
			}
		}

		/// <summary>
		/// Sets the upload file name for this request.
		/// </summary>
		/// <param name="fileName">
		///            the upload file name </param>
		/// <returns> this request </returns>
		public virtual RestRequest setFileName(string fileName)
		{
			this.fileName = fileName;
			return this;
		}

		/// <summary>
		/// Checks validity of this request.
		/// 
		/// In this implementation a request is valid if both the method and the
		/// resource Uri not null
		/// </summary>
		/// <returns> true if valid </returns>
		public virtual bool Valid
		{
			get
			{
				return getMethod() != null && Resource != null;
			}
		}

		/// <returns> whether resource uri is % escaped (true) or not (false). Defaults to false. </returns>
		public virtual bool ResourceUriEscaped
		{
			get
			{
				return resourceUriEscaped;
			}
		}

		/// <param name="escaped"> whether resource uri is % escaped or not </param>
		/// <returns> this request </returns>
		public virtual RestRequest setResourceUriEscaped(bool escaped)
		{
			this.resourceUriEscaped = escaped;
			return this;
		}


		/// <summary>
		/// String representation of this request.
		/// </summary>
		/// <returns> the string representation </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (getMethod() != null)
			{
				builder.Append(getMethod().ToString()).Append(" ");
			}
			if (Resource != null)
			{
				builder.Append(this.Resource);
			}
			if (!string.ReferenceEquals(Query, null))
			{
				builder.Append("?").Append(this.Query);
			}
			builder.Append(LINE_SEPARATOR);
			builder.Append(base.ToString());
			return builder.ToString();
		}
	}

}