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
namespace FitNesseTestServer.Test.FitNesse.Fixture
{


	/// <summary>
    /// The database of resources to support RestFixture FitNesse acceptance tests.
	/// </summary>
	public class Resources
	{
		private readonly IDictionary<string, IDictionary<string, Resource>> resourceDb = 
            new Dictionary<string, IDictionary<string, Resource>>();
		private static Resources instance = new Resources();
		private int counter = 0;

		public static Resources Instance
		{
			get
			{
				return instance;
			}
		}

		public virtual void clear()
		{
			foreach (string c in resourceDb.Keys)
			{
				resourceDb[c].Clear();
			}
			resourceDb.Clear();
		}

		public virtual ICollection<Resource> asCollection(string context)
		{
			ICollection<Resource> c = new List<Resource>();
			IDictionary<string, Resource> m = resourceDb[context];
			if (m != null)
			{
				foreach (KeyValuePair<string, Resource> e in m.SetOfKeyValuePairs())
				{
					string s = e.Key;
					c.Add(m[s]);
				}
			}
			return c;
		}

		public virtual IList<string> contexts()
		{
			IList<string> ctxKeys = new List<string>();
			((List<string>)ctxKeys).AddRange(resourceDb.Keys);
			return ctxKeys;
		}

		public virtual void add(string context, Resource r)
		{
			if (string.ReferenceEquals(r.Id, null))
			{
				r.Id = Convert.ToString(newCounter());
			}
			IDictionary<string, Resource> m = getMapForContext(context);
			m[r.Id] = r;
		}

		private IDictionary<string, Resource> getMapForContext(string context)
		{
			IDictionary<string, Resource> m = resourceDb[context];
			if (m == null)
			{
				m = new Dictionary<string, Resource>();
				resourceDb[context] = m;
			}
			return m;
		}

		public virtual Resource get(string context, string i)
		{
			return getMapForContext(context)[i];
		}

		public virtual int size(string context)
		{
			return getMapForContext(context).Count;
		}

		public virtual void remove(string context, string index)
		{
			getMapForContext(context).Remove(index);
		}

		public virtual void remove(string context, Resource o)
		{
			remove(context, o.Id);
		}

		public virtual void reset()
		{
			clear();
			counter = 0;
			add("/resources", new Resource("<resource>\n    <name>a funky name</name>\n    <data>an important message</data>" + "\n    <nstag xmlns:ns1='http://smartrics/ns1'>\n        <ns1:number>3</ns1:number>\n    </nstag>" + "\n</resource>"));
			add("/resources", new Resource("{ \"resource\" : { \"name\" : \"a funky name\", " + "\"data\" : \"an important message\" } }"));
			StringBuilder sb = new StringBuilder();
			sb.Append("<resource>\n");
			sb.Append("   <name>giant bob</name>\n");
			sb.Append("   <type>large content</type>\n");
			sb.Append("   <address>\n");
			sb.Append("       <street>\n");
			sb.Append("            Regent Street\n");
			sb.Append("       </street>\n");
			sb.Append("       <number>\n");
			sb.Append("            12345\n");
			sb.Append("       </number>\n");
			sb.Append("   </address>\n");
			sb.Append("   <data>\n");
			sb.Append("       <part id='0'>\n");
			sb.Append("           <source href='http://en.wikipedia.org/wiki/Inferno_(Dante)' />\n");
			sb.Append("           <content>\n");
			sb.Append("Inferno (Italian for 'Hell') is the first part of Dante Alighieri's 14th-century epic poem Divine Comedy. \n");
			sb.Append("It is followed by Purgatorio and Paradiso. It is an allegory telling of the journey of Dante through what is \n");
			sb.Append("largely the medieval concept of Hell, guided by the Roman poet Virgil. In the poem, Hell is depicted as nine \n");
			sb.Append("circles of suffering located within the Earth. Allegorically, the Divine Comedy represents the journey of the soul");
			sb.Append("towards God, with the Inferno describing the recognition and rejection of sin.\n");
			sb.Append("           </content>\n");
			sb.Append("       </part>\n");
			sb.Append("   </data>\n");
			sb.Append("</resource>\n");

			add("/resources", new Resource("100", sb.ToString()));
		}

		private int newCounter()
		{
			lock (this)
			{
				return counter++;
			}
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			string nl = Environment.NewLine;
			b.Append("Resources:[").Append(nl);
			foreach (string c in resourceDb.Keys)
			{
				b.Append(" Context(").Append(c).Append("):[").Append(nl);
				foreach (Resource r in asCollection(c))
				{
					b.Append(r).Append(nl);
				}
				b.Append(" ]").Append(nl);
			}
			b.Append("]").Append(nl);
			return b.ToString();
		}

	}

}