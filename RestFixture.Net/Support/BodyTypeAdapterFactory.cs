using System;
using System.Collections.Generic;
using System.Text;
using smartrics.rest.fitnesse.fixture.support;

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


	/// <summary>
	/// Depending on Content-Type passed in, it'll build the appropriate type adapter
	/// for parsing/rendering the cell content.
	/// 
	/// @author smartrics
	/// 
	/// </summary>
	public class BodyTypeAdapterFactory
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			BodyTypeAdapterCreator jsonBodyTypeAdapterCreator = new BodyTypeAdapterCreator()
			public BodyTypeAdapter createBodyTypeAdapter()
			return new JSONBodyTypeAdapter(variablesProvider, config);
		};
            
			contentTypeToBodyTypeAdapter[ContentType.JS] = jsonBodyTypeAdapterCreator;
			contentTypeToBodyTypeAdapter[ContentType.JSON] = jsonBodyTypeAdapterCreator;
			contentTypeToBodyTypeAdapter.put(ContentType.XML, new BodyTypeAdapterCreator()
			public BodyTypeAdapter createBodyTypeAdapter()
			return new XPathBodyTypeAdapter();
			);
			contentTypeToBodyTypeAdapter.put(ContentType.TEXT, new BodyTypeAdapterCreator()
			public BodyTypeAdapter createBodyTypeAdapter()
			return new TextBodyTypeAdapter();
			);
            
			public BodyTypeAdapterFactory(final RunnerVariablesProvider variablesProvider, Config config)
			this.variablesProvider = variablesProvider;
			this.config = config;
            
			/// <summary>
			/// Returns a @link <seealso cref="BodyTypeAdapter"/> for the given charset and @link <seealso cref="ContentType"/>.
			/// </summary>
			/// <param name="content"> the contentType </param>
			/// <param name="charset"> the charset. </param>
			/// <returns> an instance of <seealso cref="BodyTypeAdapter"/> </returns>
			public BodyTypeAdapter getBodyTypeAdapter(ContentType content, string charset)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BodyTypeAdapterCreator creator = contentTypeToBodyTypeAdapter.get(content);
			BodyTypeAdapterCreator creator = contentTypeToBodyTypeAdapter[content];
			if (creator == null)
			{
			throw new System.ArgumentException("Content-Type is UNKNOWN.  Unable to find a BodyTypeAdapter to instantiate.");
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BodyTypeAdapter instance = creator.createBodyTypeAdapter();
			BodyTypeAdapter instance = creator.createBodyTypeAdapter();
			if (charset != null)
			{
			instance.Charset = charset;
			}
			else
			{
			instance.Charset = Charset.defaultCharset().name();
			}
			return instance;
            
			interface BodyTypeAdapterCreator
			BodyTypeAdapter createBodyTypeAdapter();
            
	}


		private final RunnerVariablesProvider variablesProvider;
		private final Config config;

		private IDictionary<ContentType, BodyTypeAdapterCreator> contentTypeToBodyTypeAdapter = new Dictionary<ContentType, BodyTypeAdapterCreator>();
		{
			BodyTypeAdapterCreator jsonBodyTypeAdapterCreator = new BodyTypeAdapterCreatorAnonymousInnerClass(this);

			contentTypeToBodyTypeAdapter.put(ContentType.JS, jsonBodyTypeAdapterCreator);
			contentTypeToBodyTypeAdapter.put(ContentType.JSON, jsonBodyTypeAdapterCreator);
			contentTypeToBodyTypeAdapter.put(ContentType.XML, new BodyTypeAdapterCreatorAnonymousInnerClass2(this));
			contentTypeToBodyTypeAdapter.put(ContentType.TEXT, new BodyTypeAdapterCreatorAnonymousInnerClass3(this));
		}

		public BodyTypeAdapterFactory(final RunnerVariablesProvider variablesProvider, Config config)
		{
			this.variablesProvider = variablesProvider;
			this.config = config;
		}

		/// <summary>
		/// Returns a @link <seealso cref="BodyTypeAdapter"/> for the given charset and @link <seealso cref="ContentType"/>.
		/// </summary>
		/// <param name="content"> the contentType </param>
		/// <param name="charset"> the charset. </param>
		/// <returns> an instance of <seealso cref="BodyTypeAdapter"/> </returns>
		public BodyTypeAdapter getBodyTypeAdapter(ContentType content, string charset)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BodyTypeAdapterCreator creator = contentTypeToBodyTypeAdapter.get(content);
			BodyTypeAdapterCreator creator = contentTypeToBodyTypeAdapter.get(content);
			if (creator == null)
			{
				throw new System.ArgumentException("Content-Type is UNKNOWN.  Unable to find a BodyTypeAdapter to instantiate.");
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BodyTypeAdapter instance = creator.createBodyTypeAdapter();
			BodyTypeAdapter instance = creator.createBodyTypeAdapter();
			if (charset != null)
			{
				instance.Charset = charset;
			}
			else
			{
				instance.Charset = Charset.defaultCharset().name();
			}
			return instance;
		}

		interface BodyTypeAdapterCreator
		{
			BodyTypeAdapter createBodyTypeAdapter();
		}

}

}