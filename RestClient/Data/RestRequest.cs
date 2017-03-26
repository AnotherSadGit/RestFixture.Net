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
            // WARNING: In the original Java implementation, "Get" was the first enum value so it 
            //  would have the value 0; there was no "NotSet" value.  However, Java and .NET have 
            //  different behaviour for uninitialized enums.  In Java an uninitialized enum has 
            //  value null whereas in .NET it has value 0.  So we need to add a "NotSet" with value 
            //  0 to the .NET implementation, to avoid defaulting to a valid enum value like "Get".
            NotSet = 0,
			Get,
			Post,
			Put,
			Delete,
			Head,
			Options,
            Trace,
            Patch   // WARNING: Original Java implementation does not include Patch.
		}

		private const string FILE = "file";
		private string _fileName;
		[Obsolete]
		private string _multipartFileName;
		[Obsolete]
		private string _multipartFileParameterName = FILE;
        private LinkedHashMap<string, RestMultipart> _multipartFileByParamName = new LinkedHashMap<string, RestMultipart>();
		private string _query;
		private Method _method;
		private bool _followRedirect = true;
		private bool _isResourceUriEscaped = false;

	    /// <value> the method for this request </value>
	    public virtual Method HttpMethod
	    {
	        get { return _method; }
            set { _method = value; }
	    }

		/// <returns> the query for this request </returns>
		public virtual string Query
		{
			get
			{
				return _query;
			}
            set { _query = value; }
		}

		/// <returns> the upload file name for this request </returns>
		public virtual string FileName
		{
			get
			{
				return _fileName;
			}
		    set { _fileName = value; }
		}

		/// <returns> the multipart upload files name for this request </returns>
        public virtual LinkedHashMap<string, RestMultipart> MultipartFileNames
		{
			get
			{
				return _multipartFileByParamName;
			}
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
			_multipartFileByParamName[multiParamName] = restMultipart;
			return this;
		}

		public virtual bool FollowRedirect
		{
			get
			{
				return _followRedirect;
			}
            set { _followRedirect = value; }
		}

		/// <summary>
		/// Checks validity of this request.
		/// 
		/// In this implementation a request is valid if both the method and the
		/// resource Uri are set
		/// </summary>
		/// <returns> true if valid </returns>
		public virtual bool IsValid
		{
			get
			{
                return HttpMethod != Method.NotSet && Resource != null;
			}
		}

		/// <returns> whether resource uri is % escaped (true) or not (false). Defaults to false. </returns>
		public virtual bool IsResourceUriEscaped
		{
			get
			{
				return _isResourceUriEscaped;
			}
            set { _isResourceUriEscaped = value; }
		}

		/// <summary>
		/// String representation of this request.
		/// </summary>
		/// <returns> the string representation </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (HttpMethod != null)
			{
				builder.Append(HttpMethod.ToString()).Append(" ");
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