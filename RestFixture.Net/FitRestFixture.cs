using System;
using System.Collections.Generic;
using NLog;

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
namespace RestFixture.Net
{

	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using Runner = smartrics.rest.fitnesse.fixture.RestFixture.Runner;
	using CellFormatter = smartrics.rest.fitnesse.fixture.support.CellFormatter;
	using Config = smartrics.rest.fitnesse.fixture.support.Config;
	using RowWrapper = smartrics.rest.fitnesse.fixture.support.RowWrapper;
	using Tools = smartrics.rest.fitnesse.fixture.support.Tools;
	using Url = smartrics.rest.fitnesse.fixture.support.Url;
	using ActionFixture = fit.ActionFixture;
	using Parse = fit.Parse;

	/// <summary>
	/// A <seealso cref="RestFixture"/> implementation for Fit runner. The class is simply an
	/// <seealso cref="ActionFixture"/> implementation that delegates to an instance of
	/// <seealso cref="RestFixture"/>.
	/// 
	/// @author smartrics
	/// </summary>
	public class FitRestFixture : ActionFixture
	{
        private static NLog.Logger LOG = LogManager.GetCurrentClassLogger();

		private RestFixture restFixture;

		public override string ToString()
		{
			return restFixture.ToString();
		}

		/// <summary>
		/// See <seealso cref="RestFixture#getLastEvaluation()"/>
		/// </summary>
		/// <returns> last JS evaluation </returns>
		public virtual string LastEvaluation
		{
			get
			{
				return restFixture.LastEvaluation;
			}
		}

		/// <returns> delegates to <seealso cref="RestFixture#getBaseUrl()"/> </returns>
		public virtual string getBaseUrl()
		{
			return restFixture.BaseUrl;
		}

		public virtual void baseUrl(string url)
		{ //mqm  - it comes as a string in a scenario.
			this.setBaseUrl(new Url(url));
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#setBaseUrl(Url)"/>
		/// </summary>
		/// <param name="url">
		///            the base url. </param>
		public virtual void setBaseUrl(Url url)
		{
			restFixture.BaseUrl = url;
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#getDefaultHeaders()"/>
		/// </summary>
		/// <returns> the default headers. </returns>
		public virtual IDictionary<string, string> DefaultHeaders
		{
			get
			{
				return restFixture.DefaultHeaders;
			}
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#getFormatter()"/>
		/// </summary>
		/// <returns> the cell formatter for Fit. </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public smartrics.rest.fitnesse.fixture.support.CellFormatter<?> getFormatter()
		public virtual CellFormatter<object> Formatter
		{
			get
			{
				return restFixture.Formatter;
			}
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#setMultipartFileName()"/>
		/// </summary>
		public virtual void setMultipartFileName()
		{
			restFixture.setMultipartFileName();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#getMultipartFileName()"/>
		/// </summary>
		/// <returns> the multipart filename to upload. </returns>
		public virtual string MultipartFileName
		{
			get
			{
				return restFixture.MultipartFileName;
			}
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#setFileName()"/>
		/// </summary>
		public virtual void setFileName()
		{
			restFixture.setFileName();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#getFileName()"/>
		/// </summary>
		/// <returns> the name of the file to upload </returns>
		public virtual string FileName
		{
			get
			{
				return restFixture.FileName;
			}
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#setMultipartFileParameterName()"/>
		/// </summary>
		public virtual void setMultipartFileParameterName()
		{
			restFixture.setMultipartFileParameterName();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#getMultipartFileParameterName()"/>
		/// </summary>
		/// <returns> the name of the parameter containing the multipart file to
		///         upload. </returns>
		public virtual string MultipartFileParameterName
		{
			get
			{
				return restFixture.MultipartFileParameterName;
			}
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#setBody()"/>
		/// </summary>
		public virtual void setBody()
		{
			restFixture.setBody();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#setHeader()"/>
		/// </summary>
		public virtual void setHeader()
		{
			restFixture.setHeader();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#setHeaders()"/>
		/// </summary>
		public virtual void setHeaders()
		{
			restFixture.setHeaders();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#PUT()"/>
		/// </summary>
		public virtual void PUT()
		{
			restFixture.PUT();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#GET()"/>
		/// </summary>
		public virtual void GET()
		{
			restFixture.GET();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#DELETE()"/>
		/// </summary>
		public virtual void DELETE()
		{
			restFixture.DELETE();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#POST()"/>
		/// </summary>
		public virtual void POST()
		{
			restFixture.POST();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#HEAD()"/>
		/// </summary>
		public virtual void HEAD()
		{
			restFixture.HEAD();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#OPTIONS()"/>
		/// </summary>
		public virtual void OPTIONS()
		{
			restFixture.OPTIONS();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#TRACE()"/>
		/// </summary>
		public virtual void TRACE()
		{
			restFixture.TRACE();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#let()"/>
		/// </summary>
		public virtual void let()
		{
			restFixture.let();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#comment()"/>
		/// </summary>
		public virtual void comment()
		{
			restFixture.comment();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#evalJs()"/>
		/// </summary>
		public virtual void evalJs()
		{
			restFixture.evalJs();
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#processRow(RowWrapper)"/>
		/// </summary>
		/// <param name="currentRow">
		///            the row to process. </param>
		public virtual void processRow<T1>(RowWrapper<T1> currentRow)
		{
			restFixture.processRow(currentRow);
		}

		/// <summary>
		/// delegates to <seealso cref="RestFixture#getHeaders()"/>
		/// </summary>
		/// <returns> the headers. </returns>
		public virtual IDictionary<string, string> Headers
		{
			get
			{
				return restFixture.Headers;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "rawtypes", "unchecked" }) public void doCells(fit.Parse parse)
		public override void doCells(Parse parse)
		{
			if (restFixture == null)
			{
				restFixture = new RestFixture();
				restFixture.Config = Config.getConfig(ConfigNameFromArgs);
				string url = BaseUrlFromArgs;
				if (!string.ReferenceEquals(url, null))
				{
					restFixture.BaseUrl = new Url(Tools.fromSimpleTag(url));
				}
				restFixture.initialize(Runner.FIT);
				((FitFormatter) restFixture.Formatter).ActionFixtureDelegate = this;
			}
			RowWrapper currentRow = new FitRow(parse);
			try
			{
				restFixture.processRow(currentRow);
			}
			catch (Exception exception)
			{
				LOG.error("Exception when processing row " + currentRow.getCell(0).text(), exception);
				restFixture.Formatter.exception(currentRow.getCell(0), exception);
			}
		}

		/// <returns> optional config name </returns>
		protected internal virtual string ConfigNameFromArgs
		{
			get
			{
				if (args.length >= 2)
				{
					return args[1];
				}
				return null;
			}
		}

		/// <returns> Process args (<seealso cref="fit.Fixture"/>) for Fit runner to extract the
		///         baseUrl of each Rest request, first parameter of each RestFixture
		///         table. </returns>
		protected internal virtual string BaseUrlFromArgs
		{
			get
			{
				if (args.length > 0)
				{
					return args[0];
				}
				return null;
			}
		}

		/// <returns> the config </returns>
		public virtual Config Config
		{
			get
			{
				return restFixture.Config;
			}
		}

	}

}