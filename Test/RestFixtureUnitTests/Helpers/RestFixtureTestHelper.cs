using System;
using System.Collections.Generic;
using System.Text;
using fit;
using Moq;
using restFixture.Net;
using restFixture.Net.Fixtures;
using restFixture.Net.Support;
using restFixture.Net.TableElements;
using restFixture.Net.TypeAdapters;
using RestClient;
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
namespace smartrics.rest.fitnesse.fixture
{
	public class RestFixtureTestHelper
	{
		public virtual IRowWrapper<string> createTestRow(params string[] cells)
		{
            TestStubRowWrapper rowWrapper = new TestStubRowWrapper(cells);
            return rowWrapper;
		}

		public virtual Parse createSingleRowFitTable(params string[] cells)
		{
			return createFitTable(cells);
		}

		public virtual Parse createFitTable(params string[][] cellsArray)
		{
			Parse t = null;
			StringBuilder rBuff = new StringBuilder();
			rBuff.Append("<table>");
			foreach (string[] cells in cellsArray)
			{
				rBuff.Append(createFitRow(cells));
			}
			rBuff.Append("</table>");
            t = new Parse(rBuff.ToString());
			return t;
		}

		private string createFitRow(params string[] cells)
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append("<tr>");
			foreach (string c in cells)
			{
				buffer.Append("<td>").Append(c).Append("</td>");
			}
			buffer.Append("</tr>");
			return buffer.ToString();
		}

		public virtual Parse buildEmptyParse()
		{
			return createSingleRowFitTable("&nbsp;");
		}

		public virtual IList<IList<string>> createSingleRowSlimTable(params string[] cells)
		{
			IList<IList<string>> table = new List<IList<string>>();
			table.Add(cells);
			return table;
		}

        //public virtual void wireMocks(Config conf, Mock<PartsFactory> mockPartsFactory, 
        //    Mock<IRestClient> mockRestClient, RestRequest request, RestResponse response, 
        //    ICellFormatter<string> cellFormatter, BodyTypeAdapter bodyTypeAdapter)
        //{
        //    mockRestClient.Setup(rc => rc.Execute(request)).Returns(response);
        //    mockPartsFactory.Setup(pf => pf.buildRestClient(conf)).Returns(mockRestClient.Object);
        //    mockPartsFactory.Setup(pf => pf.buildRestRequest()).Returns(request);

        //    // Couldn't get the following working:
        //    //mockPartsFactory.Setup(pf => pf.buildCellFormatter<string>(It.IsAny<Runner>()).
        //    //mockPartsFactory.Setup(pf => pf.buildBodyTypeAdapter()).Returns(bodyTypeAdapter);

        //    // Original Java Mockito code:
        //    //when(pf.buildRestClient(conf)).thenReturn(rc);
        //    //when(pf.buildRestRequest()).thenReturn(req);
        //    //when(rc.execute(req)).thenReturn(resp);
        //    //when(pf.buildCellFormatter(any(typeof(CommonRestFixture.Runner)))).thenReturn(cf);
        //    //when(pf.buildBodyTypeAdapter(isA(typeof(ContentType)), isA(typeof(string)))).thenReturn(bta);
        //}
	}


}