using System;
using System.Collections.Generic;
using System.Text;
using RestClient.Data;

/*  Copyright 2017 SImon Elms
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
namespace RestFixture.Net.Support
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
			ContentType r = contentTypeToEnum[t];
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
                Encoding.Default.EncodingName);
			string htmlConfig = config.get("restfixture.content.handlers.map", "");
			string configStr = Tools.fromHtml(htmlConfig);
			IDictionary<string, string> map = Tools.convertStringToMap(configStr, "=", "\n", true);
			foreach (KeyValuePair<string, string> e in map.SetOfKeyValuePairs())
			{
				string value = e.Value;
				string enumName = value.ToUpper();
				ContentType ct = ContentType.valueOf(enumName);
				if (null == ct)
				{
					IList<ContentType> values = ContentType.values();
					StringBuilder sb = new StringBuilder();
					sb.Append("[");
					foreach (ContentType cType in values)
					{
						sb.Append("'").Append(cType.ToString()).Append("' ");
					}
					sb.Append("]");
                    throw new System.ArgumentException(
                        "I don't know how to handle " + value + ". Use one of " + sb.ToString());
				}
				contentTypeToEnum[e.Key] = ct;
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
			contentTypeToEnum["default"] = ContentType.XML;
			contentTypeToEnum["application/xml"] = ContentType.XML;
			contentTypeToEnum["application/json"] = ContentType.JSON;
			contentTypeToEnum["text/plain"] = ContentType.TEXT;
			contentTypeToEnum["application/x-javascript"] = ContentType.JS;
		}

		/// <summary>
		/// parses a string to a content type. </summary>
		/// <param name="contentTypeString"> the content type </param>
		/// <returns> the <seealso cref="ContentType"/>. </returns>
		public static ContentType parse(string contentTypeString)
		{
			string c = contentTypeString;
			if (string.ReferenceEquals(c, null))
			{
				return contentTypeToEnum["default"];
			}
			int pos = contentTypeString.IndexOf(";", StringComparison.Ordinal);
			if (pos > 0)
			{
				c = contentTypeString.Substring(0, pos).Trim();
			}
			ContentType ret = contentTypeToEnum[c];
			if (ret == null)
			{
				return contentTypeToEnum["default"];
			}
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