using System;
using System.Collections.Generic;
using restFixture.Net.Support;

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
namespace restFixture.Net
{

	using Fixture = fit.Fixture;
	using Parse = fit.Parse;

	/// <summary>
	/// A simple fixture to store configuration data for the rest fixture.
	/// <para>
	/// A configuration is a named map that stores key/value pairs. The name of the
	/// map is passed as an optional parameter to the fixture. If not passed it's
	/// assumed that a default name is used. The default value of the map name is
	/// <seealso cref="Config#DEFAULT_CONFIG_NAME"/>.
	/// </para>
	/// <para>
	/// The structure of the table of this fixture simply a table that reports
	/// key/values. The name of the config is optionally passed to the fixture.
	/// </para>
	/// <para>
	/// Example:
	/// </para>
	/// <para>
	/// Uses the default config name:
	/// <table border="1">
	/// <caption>example</caption>
	/// <tr>
	/// <td colspan="2">smartrics.rest.fitnesse.fixture.RestFixtureConfig</td>
	/// </tr>
	/// <tr>
	/// <td>key1</td>
	/// <td>value1</td>
	/// </tr>
	/// <tr>
	/// <td>key2</td>
	/// <td>value2</td>
	/// </tr>
	/// <tr>
	/// <td>...</td>
	/// <td>...</td>
	/// </tr>
	/// </table>
	/// </para>
	/// <para>
	/// Uses the config name <i>confname</i>:
	/// <table border="1">
	/// <caption>example</caption>
	/// <tr>
	/// <td>smartrics.rest.fitnesse.fixture.RestFixtureConfig</td>
	/// <td>confname</td>
	/// </tr>
	/// <tr>
	/// <td>key1</td>
	/// <td>value1</td>
	/// </tr>
	/// <tr>
	/// <td>key2</td>
	/// <td>value2</td>
	/// </tr>
	/// <tr>
	/// <td>...</td>
	/// <td>...</td>
	/// </tr>
	/// </table>
	/// </para>
	/// <para>
	/// <seealso cref="RestFixture"/> accesses the config passed by name as second parameter to
	/// the fixture or the default if no name is passed:
	/// <table border="1">
	/// <caption>example</caption>
	/// <tr>
	/// <td>smartrics.rest.fitnesse.fixture.RestFixture</td>
	/// <td>http://localhost:7070</td>
	/// </tr>
	/// <tr>
	/// <td colspan="2">...</td>
	/// </tr>
	/// </table>
	/// 
	/// or
	/// 
	/// <table border="1">
	/// <caption>example</caption>
	/// <tr>
	/// <td >smartrics.rest.fitnesse.fixture.RestFixture</td>
	/// <td>http://localhost:7070</td>
	/// <td>confname</td>
	/// </tr>
	/// <tr>
	/// <td colspan="3">...</td>
	/// </tr>
	/// </table>
	/// </para>
	/// </summary>
	public class RestFixtureConfig : Fixture
	{

		private Config config;

		/// <summary>
		/// Default constructor.
		/// 
		/// For fixtures with no args.
		/// 
		/// </summary>
		public RestFixtureConfig()
		{
		}

		/// <summary>
		/// Constructor with args. Arguments are extracted from the first row of the
		/// fixture.
		/// </summary>
		/// <param name="args"> the fixture args </param>
		
        // Shouldn't need this.  FitSharp.Fixture reads args from first row of table.
        //public RestFixtureConfig(params string[] args)
        //{
        //    base.args = args;
        //}

		/// <summary>
		/// Support for Slim runner.
		/// </summary>
		/// <param name="rows"> the rows </param>
		/// <returns> the content as a list (of rows) of lists of strings (the cells). </returns>
		public virtual IList<IList<string>> doTable(IList<IList<string>> rows)
		{
			Config c = Config;
			foreach (IList<string> row in rows)
			{
				string k = row[0];
				if (row.Count == 2)
				{
					k = row[0];
					string v = row[1];
					c.add(k, v);
					row[0] = "";
					row[1] = "pass:" + Tools.toHtml(v);
				}
				else
				{
					row[0] = "error:" + k + Tools.toHtml("\n\nthis line doesn't conform to NVP format " + "(col 0 for name, col 1 for value) - content skipped");
				}
			}
			return rows;
		}

		/// <summary>
		/// Processes each row in the config fixture table and loads the key/value
		/// pairs. The fixture optional first argument is the config name. If not
		/// supplied the value is defaulted. See <seealso cref="Config#DEFAULT_CONFIG_NAME"/>.
		/// </summary>
        public override void DoRow(Parse p)
		{
			Parse cells = p.Parts;
			try
			{
				string key = cells.Text;
                string value = cells.More.Text;
				Config c = Config;
				c.add(key, value);
				string fValue = Tools.toHtml(value);
				Parse valueParse = cells.More;
				valueParse.SetBody(fValue);
				Right(valueParse);
			}
			catch (Exception e)
			{
				Exception(p, e);
			}
		}

		private Config Config
		{
			get
			{
				if (config != null)
				{
					return config;
				}
				if (base.Args != null && base.Args.Length > 0)
				{
                    config = Config.getConfig(base.Args[0]);
				}
				else
				{
                    config = Config.getConfig();
				}
				return config;
			}
		}
	}

}