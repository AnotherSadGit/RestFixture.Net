﻿using System;
using System.Collections.Generic;
using fit;
using NLog;
using RestFixture.Net.Support;
using RestFixture.Net.TableElements;
using RestFixture.Net.Tools;

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
namespace RestFixture.Net.Fixtures
{
	/// <summary>
	/// A Fit implementation of the Rest Fixture for testing RESTful APIs.
	/// </summary>
	public class FitRestFixture : ActionFixture
	{
        private static NLog.Logger LOG = LogManager.GetCurrentClassLogger();

        private CommonRestFixture<Parse> _restFixture;

	    public FitRestFixture()
	    {
            //Required to tell base ActionFixture what class is handling the parsed table.
            actor = this;
	    }

		public override string ToString()
		{
			//return restFixture.ToString();
		    return this.GetType().FullName;
		}

		/// <summary>
		/// See <seealso cref="CommonRestFixture{T}#getLastEvaluation()"/>
		/// </summary>
		/// <returns> last JS evaluation </returns>
		public virtual string LastEvaluation
		{
			get
			{
				return _restFixture.LastEvaluation;
			}
		}

        public virtual Support.Url BaseUrl
        {
            get
            {
                return _restFixture.BaseUrl;
            }

            set { _restFixture.BaseUrl = value; }
        }

        /// <returns> delegates to <seealso cref="CommonRestFixture{T}.BaseUrlString"/> </returns>
		public virtual string BaseUrlString
		{
			get
			{
			    return _restFixture.BaseUrlString;
            }

            set
            {
                this.BaseUrl = new Url(value);
            }
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#getDefaultHeaders()"/>
		/// </summary>
		/// <returns> the default headers. </returns>
		public virtual IDictionary<string, string> DefaultHeaders
		{
			get
			{
				return _restFixture.DefaultHeaders;
			}
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#getFormatter()"/>
		/// </summary>
		/// <returns> the cell formatter for Fit. </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public smartrics.rest.fitnesse.fixture.support.CellFormatter<?> getFormatter()
		public virtual ICellFormatter<Parse> Formatter
		{
			get
			{
				return _restFixture.Formatter;
			}
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#setMultipartFileName()"/>
		/// </summary>
		public virtual void setMultipartFileName()
		{
			_restFixture.SetMultipartFileName();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#getMultipartFileName()"/>
		/// </summary>
		/// <returns> the multipart filename to upload. </returns>
		public virtual string MultipartFileName
		{
			get
			{
				return _restFixture.MultipartFileName;
			}
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#FileName()"/>
		/// </summary>
		/// <returns> the name of the file to upload </returns>
		public virtual string FileName
		{
			get
			{
				return _restFixture.FileName;
			}

		    set
		    {
                _restFixture.FileName = value;
		    }
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#setMultipartFileParameterName()"/>
		/// </summary>
		public virtual void setMultipartFileParameterName()
		{
			_restFixture.setMultipartFileParameterName();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#getMultipartFileParameterName()"/>
		/// </summary>
		/// <returns> the name of the parameter containing the multipart file to
		///         upload. </returns>
		public virtual string MultipartFileParameterName
		{
			get
			{
				return _restFixture.MultipartFileParameterName;
			}
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#setBody()"/>
		/// </summary>
		public virtual void setBody()
		{
			_restFixture.setBody();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#setHeader()"/>
		/// </summary>
		public virtual void setHeader()
		{
			_restFixture.setHeader();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#setHeaders()"/>
		/// </summary>
		public virtual void setHeaders()
		{
			_restFixture.setHeaders();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#PUT()"/>
		/// </summary>
		public virtual void PUT()
		{
			_restFixture.PUT();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#GET()"/>
		/// </summary>
		public virtual void GET()
		{
			_restFixture.GET();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#DELETE()"/>
		/// </summary>
		public virtual void DELETE()
		{
			_restFixture.DELETE();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#POST()"/>
		/// </summary>
		public virtual void POST()
		{
			_restFixture.POST();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#HEAD()"/>
		/// </summary>
		public virtual void HEAD()
		{
			_restFixture.HEAD();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#OPTIONS()"/>
		/// </summary>
		public virtual void OPTIONS()
		{
			_restFixture.OPTIONS();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#TRACE()"/>
		/// </summary>
		public virtual void TRACE()
		{
			_restFixture.TRACE();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#let()"/>
		/// </summary>
		public virtual void let()
		{
			_restFixture.let();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#comment()"/>
		/// </summary>
		public virtual void comment()
		{
			_restFixture.comment();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#evalJs()"/>
		/// </summary>
		public virtual void evalJs()
		{
			_restFixture.evalJs();
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#processRow(RowWrapper)"/>
		/// </summary>
		/// <param name="currentRow">
		///            the row to process. </param>
		public virtual void processRow(IRowWrapper<Parse> currentRow)
		{
			_restFixture.processRow(currentRow);
		}

		/// <summary>
		/// delegates to <seealso cref="CommonRestFixture{T}#getHeaders()"/>
		/// </summary>
		/// <returns> the headers. </returns>
		public virtual IDictionary<string, string> Headers
		{
			get
			{
				return _restFixture.Headers;
			}
		}

        public override void DoCells(Parse parse)
		{
			if (_restFixture == null)
			{
                _restFixture = new CommonRestFixture<Parse>(this.Symbols);
				_restFixture.Config = Config.getConfig(ConfigNameFromArgs);
				string url = BaseUrlFromArgs;
				if (url != null)
				{
					_restFixture.BaseUrl = new Url(HtmlTools.fromSimpleTag(url));
				}
                _restFixture.initialize(Runner.FIT);
				((FitFormatter) _restFixture.Formatter).ActionFixtureDelegate = this;
                _restFixture.Formatter.DisplayActual = _restFixture.displayActualOnRight;
                _restFixture.Formatter.DisplayAbsoluteURLInFull = _restFixture.displayAbsoluteURLInFull;
                _restFixture.Formatter.MinLengthForToggleCollapse = _restFixture.minLenForCollapseToggle;
			}
            IRowWrapper<Parse> currentRow = new FitRow(parse);
			try
			{
				_restFixture.processRow(currentRow);
			}
			catch (Exception exception)
			{
                // TODO: Sort out CellWrapper vs CellWrapper<Parse>.
                ICellWrapper<Parse> firstCell = currentRow.getCell(0);
				LOG.Error(exception, "Exception when processing row {0}", firstCell.text());
			    ICellFormatter<Parse> cellFormatter = _restFixture.Formatter;
                cellFormatter.exception(firstCell, exception);
			}
		}

		/// <returns> optional config name </returns>
		protected internal virtual string ConfigNameFromArgs
		{
			get
			{
				if (Args.Length >= 2)
				{
					return Args[1];
				}
				return null;
			}
		}

		/// <returns> Process Args (<seealso cref="fit.Fixture"/>) for Fit runner to extract the
		///         baseUrl of each Rest request, first parameter of each RestFixture
		///         table. </returns>
		protected internal virtual string BaseUrlFromArgs
		{
			get
			{
				if (Args.Length > 0)
				{
					return Args[0];
				}
				return null;
			}
		}

		/// <returns> the config </returns>
		public virtual Config Config
		{
			get
			{
				return _restFixture.Config;
			}
		}
	}

}