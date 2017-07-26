using System;

/*  Copyright 2017 Simon Elms
 *
 *  This file is part of RestFixture.Net
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
	/// Facade to <seealso cref="java.net.URL"/>. Just to offer a REST oriented interface.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class Url
	{

		private Uri _baseUrl;

		/// <param name="urlString">
		///            the string representation of url. </param>
		public Url(string urlString)
		{
			try
			{
                if (string.IsNullOrWhiteSpace(urlString))
				{
					throw new System.ArgumentException("Null or empty input: " + urlString);
				}
				string u = urlString;
				if (urlString.EndsWith("/", StringComparison.Ordinal))
				{
					u = urlString.Substring(0, u.Length - 1);
				}
				_baseUrl = new Uri(u);
				if ("".Equals(_baseUrl.Host))
				{
					throw new System.ArgumentException("No host specified in base URL: " + urlString);
				}
			}
            catch (UriFormatException e)
			{
				throw new System.ArgumentException("Malformed base URL: " + urlString, e);
			}
		}

		/// <returns> the base url </returns>
		public virtual Uri GetUrl()
		{
			return _baseUrl;
		}

		public override string ToString()
		{
			return GetUrl().ToString();
		}

		/// <returns> the resource </returns>
		public virtual string Resource
		{
			get
			{
                string res = GetUrl().AbsolutePath.Trim();
				if (res.Length == 0)
				{
					return "/";
				}
				return res;
			}
		}

		/// 
		/// <returns> the base url. </returns>
		public virtual string BaseUrlString
		{
			get
			{
				string path = Resource.Trim();
				if (path.Length == 0 || path.Equals("/"))
				{
					return ToString();
				}
				int index = ToString().IndexOf(Resource, StringComparison.OrdinalIgnoreCase);
				if (index >= 0)
				{
					return ToString().Substring(0, index);
				}
				else
				{
					throw new System.InvalidOperationException("Invalid URL");
				}
			}
		}

		/// <summary>
		/// builds a url
		/// </summary>
		/// <param name="file">
		///            the file </param>
		/// <returns> the full url. </returns>
		public virtual Uri BuildUrl(string file)
		{
			try
			{
				return new Uri(_baseUrl, file);
			}
            catch (UriFormatException)
			{
				throw new System.ArgumentException("Invalid URL part: " + file);
			}
		}

	}

}