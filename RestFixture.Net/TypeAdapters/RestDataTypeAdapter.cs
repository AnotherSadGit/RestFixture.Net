using System;
using System.Collections.Generic;

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
namespace RestFixture.Net.Support
{
    //using TypeAdapter = fit.TypeAdapter;

	/// <summary>
	/// Base class for all Type Adapters used by RestFixture.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public abstract class RestDataTypeAdapter : TypeAdapter
	{
		private readonly IList<string> errors = new List<string>();

		//private object actual;

		private IDictionary<string, string> context;

		public override string ToString()
		{
			return ToString(this.Actual);
		}

        //public override void set(object a)
        //{
        //    this.actual = a;
        //}

        //public override object get()
        //{
        //    return actual;
        //}

	    public override object Actual { get; set; }

	    protected internal virtual void addError(string e)
		{
			errors.Add(e);
		}

		/// <returns> an unmodifiable list of errors. </returns>
        public virtual IReadOnlyList<string> Errors
		{
			get
			{
				return errors as IReadOnlyList<string>;
			}
		}

		/// <summary>
		/// Used to pass some form of context to the adapter.
		/// </summary>
		/// <param name="c"> the context </param>
		public virtual IDictionary<string, string> Context
		{
			set
			{
				this.context = value;
			}
			get
			{
				return context;
			}
		}


		public virtual object fromString(string o)
		{
			try
			{
				return this.parse(o);
			}
			catch (Exception)
			{
				throw new Exception("Unable to parse as " + this.GetType().FullName + ": " + o);
			}
		}
	}

}