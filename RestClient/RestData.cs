using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using RestClient.Helpers;

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
namespace RestClient
{


    /// <summary>
    /// Base class for holding shared data between {@code RestRequest} and {@code RestResponse}.
    /// </summary>
    public abstract class RestData
    {
        public static readonly string LINE_SEPARATOR = Environment.NewLine;
        public static string DEFAULT_ENCODING = "UTF-8";

        /// <summary>
        /// Holds an Http Header.
        /// </summary>
        public class Header
        {
            internal readonly string name;
            internal readonly string value;

            public Header(string name, string value)
            {
                if (string.ReferenceEquals(name, null) || string.ReferenceEquals(value, null))
                {
                    throw new System.ArgumentException("Name or Value is null");
                }
                this.name = name;
                this.value = value;
            }

            public virtual string Name
            {
                get
                {
                    return name;
                }
            }

            public virtual string Value
            {
                get
                {
                    return value;
                }
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() + 37 * Value.GetHashCode();
            }

            public override bool Equals(object o)
            {
                if (!(o is Header))
                {
                    return false;
                }
                Header h = (Header)o;
                return Name.Equals(h.Name) && Value.Equals(h.Value);
            }

            public override string ToString()
            {
                return string.Format("{0}:{1}", Name, Value);
            }
        }

        private readonly IList<Header> headers = new List<Header>();
        private sbyte[] raw;
        private string resource;
        private long? transactionId;

        /// <returns> the body of this http request/response </returns>
        public virtual string Body
        {
            get
            {
                if (raw == null)
                {
                    return null;
                }
                try
                {
                    return StringHelper.NewString(raw, Charset);
                }
                catch (ArgumentException)  
                    // Thrown if Charset is not a valid code page or is unsupported by the 
                    //  underlying platform. 
                {
                    throw new System.InvalidOperationException("Unsupported encoding: " + Charset);
                }
            }

            set
            {
                if (value == null)
                {
                    RawBody = null;
                }
                else
                {
                    RawBody = value.GetBytes();
                }
            }
        }

        public virtual sbyte[] RawBody
        {
            get
            {
                return raw;
            }

            set { raw = value; }
        }

        /// <summary> the resource type (for example {@code /resource-type}) for this
        ///         request/response </summary>
        public virtual string Resource
        {
            get { return resource; }

            set { resource = value; }
        }

        /// <summary>
        /// A transaction Id is a unique long for this transaction.
        /// 
        /// It can be used to tie request and response, especially when debugging or parsing logs.
        /// </summary>
        public virtual long? TransactionId
        {
            get
            {
                return transactionId;
            }

            set { transactionId = value; }
        }

        /// <summary>The list of headers for this request/response</summary>
        /// <remarks>Use a list rather than a dictionary to allow for multiple headers with the 
        /// same field name.  RFC 2616 states that multiple headers with the same field name are 
        /// possible, when headers which are comma-separated lists are split.  They could be 
        /// combined back into one for display but that could result in a very wide header - better 
        /// to keep them on separate lines.</remarks>
        public virtual IList<Header> Headers
        {
            get
            {
                return new ReadOnlyCollection<Header>(headers);
            }
        }

        /// <summary>Gets the headers with the specified name.</summary>
        /// <param name="name">The header name.</param>
        /// <remarks>RFC 2616 states that multiple headers with the same field name are 
        /// possible, when headers which are comma-separated lists are split.  Hence return a list 
        /// of matches rather than a single header.
        /// 
        /// This method performs a case-insensitive lookup of the header name because RFC 7230 
        /// states header field names should be case-insensitive.</remarks>
        public virtual IList<Header> getHeader(string name)
        {
            IList<Header> headersWithTheSameName = new List<Header>();
            foreach (Header h in headers)
            {
                if (h.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    headersWithTheSameName.Add(h);
                }
            }
            return headersWithTheSameName;
        }

        /// <summary>
        /// Adds an HTTP header to the current list.
        /// </summary>
        /// <param name="name">
        ///            the header name </param>
        /// <param name="value">
        ///            the header value </param>
        /// <returns> this RestData </returns>
        public virtual RestData addHeader(string name, string value)
        {
            this.headers.Add(new Header(name, value));
            return this;
        }

        /// <summary>
        /// Adds a collection of HTTP headers to the current list of headers.
        /// </summary>
        /// <param name="headers">
        ///            the collection of headers </param>
        /// <returns> this RestData </returns>
        public virtual RestData addHeaders(IDictionary<string, string> headers)
        {
            foreach (KeyValuePair<string, string> e in headers.SetOfKeyValuePairs())
            {
                addHeader(e.Key, e.Value);
            }
            return this;
        }

        /// <summary>
        /// Adds a collection of HTTP headers to the current list of headers.
        /// </summary>
        /// <param name="headers">
        ///            the list of headers </param>
        /// <returns> this RestData </returns>
        public virtual RestData addHeaders(IList<Header> headers)
        {
            foreach (Header e in headers)
            {
                addHeader(e.Name, e.Value);
            }
            return this;
        }

        /// <summary>
        /// A visually easy to read representation of this {@code RestData}.
        /// 
        /// It tries to match the typical Http Request/Response
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Header h in Headers)
            {
                builder.Append(h).Append(LINE_SEPARATOR);
            }
            if (raw != null)
            {
                builder.Append(LINE_SEPARATOR);
                builder.Append(this.Body);
            }
            else
            {
                builder.Append("[empty/null body]");
            }
            return builder.ToString();
        }

        public virtual string ContentType
        {
            get
            {
                return getHeaderValue("Content-Type");
            }
        }

        public virtual string Charset
        {
            get
            {
                string contentType = this.ContentType;

                if (contentType == null)
                {
                    return DEFAULT_ENCODING;
                }

                // RFC 2045: Content-Type parameter names, such as "charset", are case-insensitive.
                int pos = contentType.IndexOf("charset", StringComparison.InvariantCultureIgnoreCase);

                if (pos == -1)
                {
                    return DEFAULT_ENCODING;
                }

                pos = contentType.IndexOf("=", pos, StringComparison.InvariantCultureIgnoreCase);

                try
                {
                    string substring = contentType.Substring(pos + 1);
                    // The HTML 4.01 spec, section 5.2.1, states that the names for character 
                    //  encodings are case-insensitive.
                    return substring.Trim();
                }
                catch (Exception)
                {
                    return DEFAULT_ENCODING;
                }
            }
        }

        public virtual string ContentLength
        {
            get
            {
                return getHeaderValue("Content-Length");
            }
        }

        /// <summary>
        /// Gets the value of the specified header.
        /// </summary>
        /// <param name="name">The name of the header to read.</param>
        /// <returns>The string value of the header or null if the header is not found.</returns>
        /// <remarks>This method performs a case-insensitive lookup of the header name because 
        /// RFC 7230 states header field names should be case-insensitive.</remarks>
        public virtual string getHeaderValue(string name)
        {
            IList<Header> matchingHeaders = getHeader(name);

            if (matchingHeaders.Count > 0)
            {
                // Multiple headers may share the same name but only if they represent a single 
                //  comma-separated list that has been split (see RFC 2616).  Join the different 
                //  parts of the list back into one.
                string headerValue = string.Empty;
                foreach (Header matchingHeader in matchingHeaders)
                {
                    if (headerValue.Trim().Length > 0)
                    {
                        headerValue += ", ";
                    }
                    headerValue += matchingHeader.Value;
                }
                return headerValue;
            }
            return null;
        }

    }

}