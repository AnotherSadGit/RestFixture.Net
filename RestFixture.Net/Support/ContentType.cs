using System;
using System.Collections.Generic;
using System.Text;
using RestClient.Data;

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
	/// Supported content types.
	/// 
	/// @author smartrics
	/// </summary>
	public sealed class ContentType
	{

		/// <summary>
		/// represents xml content.
		/// </summary>
		public static readonly ContentType XML = new ContentType("XML", InnerEnum.XML);
		/// <summary>
		/// represents json content.
		/// </summary>
		public static readonly ContentType JSON = new ContentType("JSON", InnerEnum.JSON);
		/// <summary>
		/// represents plain text content.
		/// </summary>
		public static readonly ContentType TEXT = new ContentType("TEXT", InnerEnum.TEXT);
		/// <summary>
		/// represents javascript content.
		/// </summary>
		public static readonly ContentType JS = new ContentType("JS", InnerEnum.JS);

		private static readonly IList<ContentType> valueList = new List<ContentType>();

		public enum InnerEnum
		{
			XML,
			JSON,
			TEXT,
			JS
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		private ContentType(string name, InnerEnum innerEnum)
		{
			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		private static IDictionary<string, ContentType> contentTypeToEnum = new Dictionary<string, ContentType>();

		static ContentType()
		{
			resetDefaultMapping();

			valueList.Add(XML);
			valueList.Add(JSON);
			valueList.Add(TEXT);
			valueList.Add(JS);
		}

		/// <returns> the content type as mime type </returns>
		public IList<string> toMime()
		{
			IList<string> types = new List<string>();
			foreach (KeyValuePair<string, ContentType> e in contentTypeToEnum.SetOfKeyValuePairs())
			{
				if (e.Value.Equals(this))
				{
					types.Add(e.Key);
				}
			}
			return types;
		}

		/// <param name="t">
		///            the content type string </param>
		/// <returns> the registered content type matching the input string. </returns>
		public static ContentType typeFor(string t)
		{
			ContentType r = contentTypeToEnum.GetValueOrNull(t);
			if (r == null)
			{
				r = contentTypeToEnum["default"];
			}
			return r;
		}

		/// <summary>
		/// configures the internal map of handled content types (See
		/// <seealso cref="RestFixtureConfig"/>). It reads two properties:
		/// <ul>
		/// <li>{@code restfixture.content.default.charset} to determine the default
		/// charset for that content type, in cases when the content type header of
		/// the request/response isn't specifying the charset. If this config
		/// parameter is not set, then the default value is
		/// <seealso cref="Charset#defaultCharset()"/>.
		/// <li>{@code restfixture.content.handlers.map} to override the default map.
		/// </ul>
		/// </summary>
		/// <param name="config">
		///            the config </param>
		public static void config(Config config)
		{
			RestData.DEFAULT_ENCODING = config.get("restfixture.content.default.charset", 
                Encoding.UTF8.HeaderName);
			string htmlConfig = config.get("restfixture.content.handlers.map", "");
			string configStr = Tools.fromHtml(htmlConfig);
			IDictionary<string, string> map = Tools.convertStringToMap(configStr, "=", "\n", true);
			foreach (string key in map.Keys)
			{
				string value = map[key];
				string enumName = value.ToUpper();
				ContentType ct = ContentType.valueOf(enumName);
				if (null == ct)
				{
					IList<ContentType> values = ContentType.values();
					StringBuilder sb = new StringBuilder();
					sb.Append("[");
					foreach (ContentType cType in values)
					{
						sb.Append("'").Append(cType).Append("' ");
					}
					sb.Append("]");
                    throw new System.ArgumentException(
                        "I don't know how to handle " + value + ". Use one of " + sb);
				}
				contentTypeToEnum[key] = ct;
			}
		}

		/// <summary>
		/// resets the internal cache to default values.
		/// <table border="1">
		/// <caption>default mappings</caption>
		/// <tr>
		/// <td>{@code default}</td>
		/// <td><seealso cref="ContentType#XML"/></td>
		/// </tr>
		/// <tr>
		/// <td>{@code application/xml}</td>
		/// <td><seealso cref="ContentType#XML"/></td>
		/// </tr>
		/// <tr>
		/// <td>{@code application/json}</td>
		/// <td><seealso cref="ContentType#JSON"/></td>
		/// </tr>
		/// <tr>
		/// <td>{@code text/plain}</td>
		/// <td><seealso cref="ContentType#TEXT"/></td>
		/// </tr>
		/// <tr>
		/// <td>{@code application/x-javascript}</td>
		/// <td><seealso cref="ContentType#JS"/></td>
		/// </tr>
		/// </table>
		/// </summary>
		public static void resetDefaultMapping()
		{
			contentTypeToEnum.Clear();
		    IDictionary<string, ContentType> defaultMappings = DefaultMappings;
		    foreach (string contentType in defaultMappings.Keys)
		    {
		        contentTypeToEnum[contentType] = defaultMappings[contentType];
		    }
		}

	    /// <summary>
        /// The default content type mappings, from HTTP content-types to ContentType enums.
        /// </summary>
	    public static IDictionary<string, ContentType> DefaultMappings
	    {
            get
            {
                IDictionary<string, ContentType> defaultMappings = 
                    new Dictionary<string, ContentType>();
                defaultMappings.Add("default", ContentType.XML);
                defaultMappings.Add("application/xml", ContentType.XML);
                defaultMappings.Add("application/json", ContentType.JSON);
                defaultMappings.Add("text/plain", ContentType.TEXT);
                defaultMappings.Add("application/x-javascript", ContentType.JS);
                return defaultMappings;
            }
	    }

		/// <summary>
		/// parses a string to a content type. </summary>
		/// <param name="contentTypeString"> the content type </param>
		/// <returns> the <seealso cref="ContentType"/>. </returns>
		public static ContentType parse(string contentTypeString)
		{
			string c = contentTypeString;
			if (c == null)
			{
				return contentTypeToEnum["default"];
			}

			int pos = contentTypeString.IndexOf(";", StringComparison.Ordinal);
			if (pos > 0)
			{
				c = contentTypeString.Substring(0, pos);
			}

            c = c.Trim();
			
            if (!contentTypeToEnum.ContainsKey(c))
			{
				return contentTypeToEnum["default"];
			}

            ContentType ret = contentTypeToEnum[c];
			return ret;
		}

		public static IList<ContentType> values()
		{
			return valueList;
		}

		public InnerEnum InnerEnumValue()
		{
			return innerEnumValue;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static ContentType valueOf(string name)
		{
			foreach (ContentType enumInstance in ContentType.values())
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}