using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jurassic;
using restFixture.Net.Support;
using restFixture.Net.Tools;
using restFixture.Net.Variables;
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
namespace restFixture.Net.Javascript
{
    /// <summary>
	/// Wrapper class to all that related to JavaScript.
	/// 
	/// @author smartrics
	/// </summary>
	public class JavascriptWrapper
	{
        private Jurassic.ScriptEngine _engine = new Jurassic.ScriptEngine();

        /// <summary>
        /// The name of the constructor of the JS object containing the HTTP response.
        /// </summary>
        private const string RESPONSE_CTOR_NAME = "JsResponse";
		/// <summary>
		/// the name of the JS object containing the http response: {@code response}.
		/// </summary>
		public const string RESPONSE_OBJ_NAME = "response";
		/// <summary>
		/// the name of the JS object containing the symbol table: {@code symbols}.
		/// </summary>
		public const string SYMBOLS_OBJ_NAME = "symbols";
		/// <summary>
		/// the name of the JS object containing the json body: {@code jsonbody}.
		/// </summary>
		public const string JSON_OBJ_NAME = "jsonbody";
		private const long _64K = 65534;
		private IRunnerVariablesProvider variablesProvider;

		public JavascriptWrapper(IRunnerVariablesProvider variablesProvider)
		{
			this.variablesProvider = variablesProvider;
		}

		/// <summary>
		/// evaluates a Javascript expression in the given <seealso cref="RestResponse"/>.
		/// </summary>
		/// <param name="response">   the <seealso cref="RestResponse"/> </param>
		/// <param name="expression"> the javascript expression </param>
		/// <returns> the result of the expression evaluation. </returns>
		public virtual object evaluateExpression(RestResponse response, string expression)
		{
			return evaluateExpression(response, expression, new Dictionary<string, string>());
		}

		public virtual object evaluateExpression(RestResponse response, string expression, 
            IDictionary<string, string> imports)
		{
			if (expression == null)
			{
				return null;
			}
			injectImports(imports);
			injectFitNesseSymbolMap();
			injectResponse(response);
			object result = evaluateExpression(expression);
			return result;
		}

		/// <summary>
		/// evaluates an expression on a given json object represented as string.
		/// </summary>
		/// <param name="json">       the json object. </param>
		/// <param name="expression"> the expression. </param>
		/// <returns> the result of the evaluation </returns>
		public virtual object evaluateExpression(string json, string expression)
		{
			return evaluateExpression(json, expression, new Dictionary<string, string>());
		}

		public virtual object evaluateExpression(string json, string expression, 
            IDictionary<string, string> imports)
		{
			if (json == null || expression == null)
			{
				return null;
			}
			injectImports(imports);
			injectFitNesseSymbolMap();
			injectJson(json);
			object result = evaluateExpression(expression);
			return result;
		}

		/// <param name="json"> the potential json string. loosely checks if the input string contains <seealso cref="JavascriptWrapper#JSON_OBJ_NAME"/>. </param>
		/// <returns> whether it's actually a json object. </returns>
		public virtual bool looksLikeAJsExpression(string json)
		{
			return (json != null) && json.Contains(JSON_OBJ_NAME + ".");
		}

        private void injectFitNesseSymbolMap()
		{
			Variables.Variables variables = variablesProvider.CreateRunnerVariables();
            VariablesJavaScriptWrapper wrappedVariables =
                new VariablesJavaScriptWrapper(_engine, variables);
            _engine.SetGlobalValue(SYMBOLS_OBJ_NAME, wrappedVariables);
		}

		private void injectJson(string json)
		{
			string javascript = string.Format("{0} = {1};", JSON_OBJ_NAME, json);
            _engine.Execute(javascript);
		}

		private object evaluateExpression(string expression)
		{
			try
			{
				object result = _engine.Evaluate(expression);
			    if (result is Jurassic.Null)
			    {
			        return null;
			    }

			    return result;
			}
			catch (System.Exception ex)
			{
				throw new JavascriptException(ex.Message);
			}
		}

		private void injectResponse(RestResponse r)
		{
		    try
		    {
		        if (r == null)
		        {
		            _engine.SetGlobalValue(RESPONSE_OBJ_NAME, Null.Value);
		            return;
		        }


		        _engine.SetGlobalValue(RESPONSE_CTOR_NAME, new JsResponseConstructor(_engine));
		        string javascript = string.Format("{0} = new {1}();",
		            RESPONSE_OBJ_NAME, RESPONSE_CTOR_NAME);
		        _engine.Execute(javascript);


		        putPropertyOnJsObject(RESPONSE_OBJ_NAME, "body", r.Body);
		        putPropertyOnJsObject(RESPONSE_OBJ_NAME, JSON_OBJ_NAME, null);
		        bool isJson = isJsonResponse(r);
		        if (isJson)
		        {
                    bool valueIsJson = true;
                    putPropertyOnJsObject(RESPONSE_OBJ_NAME, JSON_OBJ_NAME, r.Body, valueIsJson);
		        }
		        putPropertyOnJsObject(RESPONSE_OBJ_NAME, "resource", r.Resource);
		        putPropertyOnJsObject(RESPONSE_OBJ_NAME, "statusText", r.StatusText);
		        putPropertyOnJsObject(RESPONSE_OBJ_NAME, "statusCode", r.StatusCode);
		        putPropertyOnJsObject(RESPONSE_OBJ_NAME, "transactionId", r.TransactionId);

		        foreach (RestData.Header h in r.Headers)
		        {
		            callMethodOnJsObject(RESPONSE_OBJ_NAME, "addHeader", h.Name, h.Value);
		        }
		    }
		    catch (Exception ex)
		    {
                throw new JavascriptException(ex.Message);
		    }
		}

		private void callMethodOnJsObject(string jsObjectName, string methodName, 
            params object[] args)
		{
			// StringBuilder is more efficient once you get past about 5 or 6 arguments.  But most 
            //  methods will have less than that so just concatenate.
		    string javaScript = string.Format("{0}.{1}(", jsObjectName, methodName);
		    if (args != null)
		    {
		        string separator = "";
		        foreach (object arg in args)
		        {
		            string argString = arg.ToString();
		            if (arg is string)
		            {
		                argString = string.Format("'{0}'", arg);
		            }

                    javaScript += separator + argString;
                    separator = ", ";
		        }
		    }
		    javaScript += ");";

            _engine.Execute(javaScript);
		}

        private void putPropertyOnJsObject(string jsObjectName, string propertyName, object value)
        {
            bool valueIsJson = false;
            putPropertyOnJsObject(jsObjectName, propertyName, value, valueIsJson);
        }

        private void putPropertyOnJsObject(string jsObjectName, string propertyName, object value, 
            bool valueIsJson)
        {
            bool valueIsNull = value == null;

            string javaScriptTemplate = "{0}.{1} = {2};";
            if (value is string && !valueIsJson && !valueIsNull)
            {
                javaScriptTemplate = "{0}.{1} = '{2}';";
            }

            if (valueIsNull)
            {
                value = "null";
            }

            string javaScript = string.Format(javaScriptTemplate,
                jsObjectName, propertyName, value);

            _engine.Execute(javaScript);
        }

		private bool isJsonResponse(RestResponse r)
		{

			if (ContentType.parse(r.ContentType) == ContentType.JSON)
			{
				return true;
			}

			if (r.Body != null && StringTools.regex(r.Body.Trim(), "\\{.+\\}"))
			{
				return XmlTools.isValidJson(r.Body);
			}
			return false;
		}

		private void injectImports(IDictionary<string, string> imports)
		{
			foreach (KeyValuePair<string, string> e in imports.SetOfKeyValuePairs())
			{
				injectImports(e.Key, e.Value);
			}

		}

		private void injectImports(string name, string importUrl)
		{
		    string importContents = null;
			try
			{
                importContents = parseImport(importUrl);
                // At first glance doesn't seem to do anything but will set any global variables 
                //  defined in the JavaScript.  These globals can be accessed later.
                _engine.Execute(importContents);
			}
			catch (Exception e)
			{
				throw new JavascriptException("Invalid url: " + importUrl + " for '" + name + "'", e);
			}
		}

		private string parseImport(string name)
		{
			try
			{
                using (System.Net.WebClient webClient = new System.Net.WebClient())
                {
                    string dowloadedData = webClient.DownloadString(name);
                    return dowloadedData;
                }
			}
			catch (Exception)
			{
                if (File.Exists(name))
				{
                    string fileContents = null;
                    // Assume UTF-8 encoding.
				    try
				    {
				        using (StreamReader reader = new StreamReader(name, Encoding.UTF8))
				        {
				            fileContents = reader.ReadToEnd();
				            return fileContents;
				        }
				    }
				    catch (Exception)
				    {
				        string errorMessage = string.Format("Invalid import file: {0}, path: {1}",
				            name, Path.GetFullPath(name));
                        throw new System.ArgumentException(errorMessage);
				    }
				}

                throw new System.ArgumentException("Import resource not valid: " + name);
			}
		}
	}

}