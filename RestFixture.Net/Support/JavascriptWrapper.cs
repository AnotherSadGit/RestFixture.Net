using System;
using System.Collections.Generic;
using System.Threading;
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
namespace RestFixture.Net.Support
{

	using org.mozilla.javascript;

	/// <summary>
	/// Wrapper class to all that related to JavaScript.
	/// 
	/// @author smartrics
	/// </summary>
	public class JavascriptWrapper
	{

		/// <summary>
		/// the name of the JS object containig the http response: {@code response}.
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
		private RunnerVariablesProvider variablesProvider;

		public JavascriptWrapper(RunnerVariablesProvider variablesProvider)
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

		public virtual object evaluateExpression(RestResponse response, string expression, IDictionary<string, string> imports)
		{
			if (string.ReferenceEquals(expression, null))
			{
				return null;
			}
			Context context = Context.enter();
			removeOptimisationForLargeExpressions(response, expression, context);
			ScriptableObject scope = context.initStandardObjects();
			injectImports(context, scope, imports);
			injectFitNesseSymbolMap(scope);
			injectResponse(context, scope, response);
			object result = evaluateExpression(context, scope, expression);
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

		public virtual object evaluateExpression(string json, string expression, IDictionary<string, string> imports)
		{
			if (string.ReferenceEquals(json, null) || string.ReferenceEquals(expression, null))
			{
				return null;
			}
			Context context = Context.enter();
			ScriptableObject scope = context.initStandardObjects();
			injectImports(context, scope, imports);
			injectFitNesseSymbolMap(scope);
			injectJson(context, scope, json);
			object result = evaluateExpression(context, scope, expression);
			return result;
		}

		/// <param name="json"> the potential json string. loosely checks if the input string contains <seealso cref="JavascriptWrapper#JSON_OBJ_NAME"/>. </param>
		/// <returns> whether it's actually a json object. </returns>
		public virtual bool looksLikeAJsExpression(string json)
		{
			return !string.ReferenceEquals(json, null) && json.Contains(JSON_OBJ_NAME + ".");
		}

		private void injectFitNesseSymbolMap(ScriptableObject scope)
		{
			Variables v = variablesProvider.createRunnerVariables();
			object wrappedVariables = Context.javaToJS(v, scope);
			ScriptableObject.putProperty(scope, SYMBOLS_OBJ_NAME, wrappedVariables);
		}

		private void injectJson(Context cx, ScriptableObject scope, string json)
		{
			evaluateExpression(cx, scope, "var " + JSON_OBJ_NAME + "=" + json);
		}

		private object evaluateExpression(Context context, ScriptableObject scope, string expression)
		{
			try
			{
				return context.evaluateString(scope, expression, null, 1, null);
			}
			catch (System.Exception e) when (e is EvaluatorException || e is EcmaError)
			{
				throw new JavascriptException(e.Message);
			}
		}

		private void injectResponse(Context cx, ScriptableObject scope, RestResponse r)
		{
			try
			{
				ScriptableObject.defineClass(scope, typeof(JsResponse));
				Scriptable response = null;
				if (r == null)
				{
					scope.put(RESPONSE_OBJ_NAME, scope, response);
					return;
				}
				object[] arg = new object[1];
				arg[0] = r;
				response = cx.newObject(scope, "JsResponse", arg);
				scope.put(RESPONSE_OBJ_NAME, scope, response);
				putPropertyOnJsObject(response, "body", r.Body);
				putPropertyOnJsObject(response, JSON_OBJ_NAME, null);
				bool isJson = isJsonResponse(r);
				if (isJson)
				{
					evaluateExpression(cx, scope, RESPONSE_OBJ_NAME + "." + JSON_OBJ_NAME + "=" + r.Body);
				}
				putPropertyOnJsObject(response, "resource", r.Resource);
				putPropertyOnJsObject(response, "statusText", r.StatusText);
				putPropertyOnJsObject(response, "statusCode", r.StatusCode);
				putPropertyOnJsObject(response, "transactionId", r.TransactionId);
				foreach (Header h in r.Headers)
				{
					callMethodOnJsObject(response, "addHeader", h.Name, h.Value);
				}
			}
			catch (IllegalAccessException e)
			{
				throw new JavascriptException(e.Message);
			}
			catch (InstantiationException e)
			{
				throw new JavascriptException(e.Message);
			}
			catch (InvocationTargetException e)
			{
				throw new JavascriptException(e.Message);
			}
		}

		private void callMethodOnJsObject(Scriptable o, string mName, params object[] arg)
		{
			ScriptableObject.callMethod(o, mName, arg);
		}

		private void putPropertyOnJsObject(Scriptable o, string mName, object value)
		{
			ScriptableObject.putProperty(o, mName, value);
		}

		private bool isJsonResponse(RestResponse r)
		{
			if (ContentType.JSON.Equals(ContentType.parse(r.ContentType)))
			{
				return true;
			}
			if (r.Body != null && r.Body.Trim().matches("\\{.+\\}"))
			{
				return Tools.isValidJson(r.Body);
			}
			return false;
		}

		private void removeOptimisationForLargeExpressions(RestResponse response, string expression, Context context)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String body = response == null ? null : response.getBody();
			string body = response == null ? null : response.Body;
			if ((!string.ReferenceEquals(body, null) && body.GetBytes().length > _64K) || expression.GetBytes().length > _64K)
			{
				context.OptimizationLevel = -1;
			}
		}

		private void injectImports(Context context, ScriptableObject scope, IDictionary<string, string> imports)
		{
			foreach (KeyValuePair<string, string> e in imports.SetOfKeyValuePairs())
			{
				injectImports(context, scope, e.Key, e.Value);
			}

		}

		private void injectImports(Context context, ScriptableObject scope, string name, string importUrl)
		{
			System.IO.Stream @is = null;
			try
			{
				@is = parseImport(importUrl);
				System.IO.StreamReader @in = new System.IO.StreamReader(@is);
				context.evaluateReader(scope, @in, name, 1, null);
			}
			catch (Exception e)
			{
				throw new JavascriptException("Invalid url: " + importUrl + " for '" + name + "'", e);
			}
			finally
			{
				if (@is != null)
				{
					try
					{
						@is.Close();
					}
					catch (IOException)
					{
						// ignore
					}
				}
			}

		}

		private System.IO.Stream parseImport(string name)
		{
			try
			{
				return (new URL(name)).openStream();
			}
			catch (Exception)
			{
				File f = new File(name);
				if (f.exists())
				{
					if (f.File && f.canRead())
					{
						try
						{
							return new System.IO.FileStream(f, System.IO.FileMode.Open, System.IO.FileAccess.Read);
						}
						catch (Exception)
						{
							throw new System.ArgumentException("Invalid import file: " + name + ", path: " + f.AbsolutePath);
						}
					}
					else
					{
						throw new System.ArgumentException("Import file not accessible: " + name + ", path: " + f.AbsolutePath);
					}
				}
				else
				{
					try
					{
						return Thread.CurrentThread.ContextClassLoader.getResource(name).openStream();
					}
					catch (Exception)
					{
						throw new System.ArgumentException("Import resource not valid: " + name);
					}
				}
			}
		}

		/// <summary>
		/// Wrapper class for Response to be embedded in the Rhino Context.
		/// 
		/// @author smartrics
		/// </summary>
		public class JsResponse : ScriptableObject
		{
			internal const long serialVersionUID = 5441026774653915695L;

			internal IDictionary<string, IList<string>> headers;

			/// <summary>
			/// def ctor.
			/// </summary>
			public JsResponse()
			{

			}

			/// <summary>
			/// initialises internal headers map.
			/// </summary>
			public virtual void jsConstructor()
			{
				headers = new Dictionary<string, IList<string>>();
			}

			public override string ClassName
			{
				get
				{
					return "JsResponse";
				}
			}

			/// <param name="name">  the header name </param>
			/// <param name="value"> the value </param>
			public virtual void jsFunction_addHeader(string name, string value)
			{
				IList<string> vals = headers[name];
				if (vals == null)
				{
					vals = new List<string>();
					headers[name] = vals;
				}
				vals.Add(value);
			}

			/// <param name="name">  the header name </param>
			/// <param name="value"> the value </param>
			public virtual void jsFunction_putHeader(string name, string value)
			{
				IList<string> vals = new List<string>();
				vals.Add(value);
				headers[name] = vals;
			}

			/// <param name="name"> the header name </param>
			/// <returns> the headers list size </returns>
			public virtual int jsFunction_headerListSize(string name)
			{
				IList<string> vals = headers[name];
				if (vals == null || vals.Count == 0)
				{
					return 0;
				}
				return vals.Count;
			}

			/// <returns> the total number of headers in the response. </returns>
			public virtual int jsFunction_headersSize()
			{
				int sz = 0;
				foreach (IList<string> hList in headers.Values)
				{
					sz += hList.Count;
				}
				return sz;
			}

			/// <param name="name"> the header name </param>
			/// <returns> the value of the header name in position 0 </returns>
			public virtual string jsFunction_header0(string name)
			{
				return jsFunction_header(name, 0);
			}

			/// <param name="name"> the header name </param>
			/// <returns> all headers with the given name </returns>
			public virtual IList<string> jsFunction_headers(string name)
			{
				int sz = jsFunction_headerListSize(name);
				if (sz > 0)
				{
					return headers[name];
				}
				else
				{
					return new List<string>();
				}
			}

			/// <param name="name"> the header name </param>
			/// <param name="pos">  the pos </param>
			/// <returns> the value of the header with name at pos 0 </returns>
			public virtual string jsFunction_header(string name, int pos)
			{
				if (jsFunction_headerListSize(name) > 0)
				{
					return headers[name][pos];
				}
				else
				{
					return null;
				}
			}

		}

	}

}