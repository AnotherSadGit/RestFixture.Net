using System;
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

namespace restFixture.Net.Support
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
        private IRunnerVariablesProvider variablesProvider;
        private Config config;

        public BodyTypeAdapterFactory(IRunnerVariablesProvider variablesProvider, Config config)
        {
            this.variablesProvider = variablesProvider;
            this.config = config;
        }

        //TODO: Rework Java Charset to use .NET Encoding instead.
        public BodyTypeAdapter getBodyTypeAdapter(ContentType content, String charset)
        {
            BodyTypeAdapter adapter = null;
            switch (content.InnerEnumValue())
            {
                case ContentType.InnerEnum.JS:
                case ContentType.InnerEnum.JSON:
                    adapter = new JSONBodyTypeAdapter(variablesProvider, config);
                    break;

                case ContentType.InnerEnum.XML:
                    adapter = new XPathBodyTypeAdapter();
                    break;

                case ContentType.InnerEnum.TEXT:
                    adapter = new TextBodyTypeAdapter();
                    break;

                default:
                    adapter = null;
                    break;
            }

            if (adapter == null)
            {
                throw new ArgumentException("Content-Type is UNKNOWN.  Unable to find a BodyTypeAdapter to instantiate.");
            }

            if (charset != null)
            {
                adapter.Charset = charset;
            }

            adapter.Charset = Encoding.Default.EncodingName;

            return adapter;
        }
    }
}