using System;
using System.Collections.Generic;
using System.ComponentModel;
using NLog.LayoutRenderers.Wrappers;

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
	/// Simple implementation of a named configuration store.
	/// 
	/// The interface implements a named map, with the ability of using a default
	/// name if not passed. The named maps are singletons (implemented as a private
	/// static field), so beware!
	/// </summary>
	public sealed class Config
	{
		/// <summary>
		/// the default name of the named config: the actual value is
		/// {@code "default"}.
		/// </summary>
		public const string DEFAULT_CONFIG_NAME = "default";

		/// <summary>
		/// the static bucket where the config data is stored.
		/// </summary>
		private static readonly IDictionary<string, Config> CONFIGURATIONS = new Dictionary<string, Config>();

		/// <summary>
		/// the configuration with default name. See
		/// <seealso cref="Config#DEFAULT_CONFIG_NAME"/>;
		/// </summary>
		/// <returns> the config with default name. </returns>
		public static Config getConfig()
		{
			return getConfig(DEFAULT_CONFIG_NAME);
		}

		/// <param name="name">
		///            the name of the config. </param>
		/// <returns> the named config object. </returns>
		public static Config getConfig(string name)
		{
			if (name == null)
			{
				name = DEFAULT_CONFIG_NAME;
			}
			Config namedConfig = null;
            if (!CONFIGURATIONS.TryGetValue(name, out namedConfig))
			{
				namedConfig = new Config(name);
				CONFIGURATIONS[name] = namedConfig;
			}
			return namedConfig;
		}

		/// <summary>
		/// this instance name.
		/// </summary>
		private readonly string name;

		private IDictionary<string, string> data;

		/// <summary>
		/// creates a config with a given name.
		/// </summary>
		/// <param name="name">
		///            the name of this config </param>
		private Config(string name)
		{
			this.name = name;
			this.data = new Dictionary<string, string>();
		}

		/// <summary>
		/// This config name.
		/// </summary>
		/// <returns> the name </returns>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Adds a key/value pair to a named configuration.
		/// </summary>
		/// <param name="key">
		///            the key </param>
		/// <param name="value">
		///            the value </param>
		public void add(string key, string value)
		{
			data[key] = value;
		}

		/// <summary>
		/// Returns a key/value from a named config.
		/// </summary>
		/// <param name="key">
		///            the key </param>
		/// <returns> the value </returns>
		public string get(string key)
		{
            if (key != null && data.ContainsKey(key))
		    {
		        return data[key];
		    }

		    return null;
		}

		/// <summary>
		/// Returns a key/value from a named config.
		/// </summary>
		/// <param name="key">
		///            the key </param>
		/// <param name="def">
		///            the default value to return when a value for the key is not
		///            present. </param>
		/// <returns> the value, Returns the default if the key is not found in the map </returns>
		public string get(string key, string def)
		{
			string v = get(key);
			if (v == null)
			{
				v = def;
			}
			return v;
		}

		/// <summary>
		/// returns a key/value from a named config, parsed as Long.
		/// </summary>
		/// <param name="key">
		///            the key </param>
		/// <param name="def">
		///            the default value for value not existent or not parseable </param>
		/// <returns> a Long representing the value, def if the value cannot be parsed
		///         as Long </returns>
		public long getAsLong(string key, long def)
		{
            return GetAsValueType<long>(key, def);
		}

		/// <summary>
		/// returns a key/value from a named config, parsed as Boolean.
		/// </summary>
		/// <param name="key">
		///            the key </param>
		/// <param name="def">
		///            the default value for value not existent or not parseable </param>
		/// <returns> a Boolean representing the value, def if the value cannot be
		///         parsed as Boolean </returns>
		public bool getAsBoolean(string key, bool def)
		{
            return GetAsValueType<bool>(key, def);
		}

		/// <summary>
		/// returns a key/value from a named config, parsed as Integer.
		/// </summary>
		/// <param name="key">
		///            the key </param>
		/// <param name="def">
		///            the default value for value not existent or not parseable </param>
		/// <returns> a Integer representing the value, def if the value cannot be
		///         parsed as Integer </returns>
		public int getAsInteger(string key, int def)
		{
		    return GetAsValueType<int>(key, def);
		}

	    private T GetAsValueType<T>(string key, T def) where T : struct
	    {
	        T? parsedValue = GetAsNullable<T>(key, def);
	        if (parsedValue == null)
	        {
	            return default(T);
	        }

	        return parsedValue.Value;
	    }

        private T? GetAsNullable<T>(string key, T? def) where T : struct 
	    {
            string val = get(key);
            if (string.IsNullOrWhiteSpace(val))
            {
                return def;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter == null || !converter.IsValid(val))
            {
                return def;
            }

            return (T)converter.ConvertFromString(val);
	    }

		/// <summary>
		/// returns a key/value from a named config, parsed as a <code>Map&lt;String, String&gt;</code>.
		/// Each line (separated by \n) in the value is parsed as {@code name=value}
		/// and stored in the returned map.
		/// </summary>
		/// <param name="key">
		///            the key </param>
		/// <param name="def">
		///            the default map to return if key is not present in the config. </param>
		/// <returns> a map representing the key value. </returns>
		public IDictionary<string, string> getAsMap(string key, IDictionary<string, string> def)
		{
			IDictionary<string, string> returnMap = new Dictionary<string, string>(def);
			string val = get(key);
			try
			{
			    if (val != null)
			    {
			        IDictionary<string, string> result = Tools.convertStringToMap(val, "=", "\n", true);
			        if (result != null && result.Keys.Count > 0)
			        {
			            foreach (string key2 in result.Keys)
			            {
			                returnMap[key2] = result[key2];
			            }
			        }
			    }
			    return returnMap;
			}
			catch (Exception)
			{
				return def;
			}
		}

		/// <summary>
		/// Clears a config store.
		/// </summary>
		public void clear()
		{
			data.Clear();
		}

		public override string ToString()
		{
			return "[name=" + Name + "] " + data.ToString();
		}
	}

}