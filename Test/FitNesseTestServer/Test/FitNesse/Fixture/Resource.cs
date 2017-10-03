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
namespace RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture
{

	/// <summary>
	/// A sample rest resource.
	/// 
	/// @author fabrizio
	/// 
	/// </summary>
	public class Resource
	{
		private string payload;
		private string id;
		private bool deleted;

		public Resource(string id, string payload)
		{
			this.payload = payload;
			this.id = id;
		}

		public Resource(string xmlContent) : this(null, xmlContent)
		{
		}

		public virtual string Payload
		{
			set
			{
				this.payload = value;
			}
			get
			{
				return payload;
			}
		}


		public virtual string Id
		{
			get
			{
				return id;
			}
			set
			{
				this.id = value;
			}
		}


		public virtual bool Deleted
		{
			get
			{
				return deleted;
			}
			set
			{
				this.deleted = value;
			}
		}


		public override string ToString()
		{
			if (string.ReferenceEquals(Payload, null))
			{
				return "";
			}
			return Payload;
		}

	}

}