using System;
using System.Collections.Generic;
using System.Net;
using NLog;

namespace RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    public class HttpRequestHandler : HttpMethodHandlerBase, IHttpRequestHandler
    {
        public IDictionary<string, IHttpMethodHandler> MethodHandlers { get; set; }

        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public HttpRequestHandler(IDictionary<string, IHttpMethodHandler> methodHandlers)
        {
            MethodHandlers = methodHandlers;
            LOG.Info("Resources: " + this.Resources.ToString());
        }

        public override void ProcessHttpRequest(HttpListenerContext context)
        {
            Validate(context);

            LOG.Debug("Processing {0} request...", context.Request.HttpMethod);

            if (!IsHttpMethodSupported(context))
            {
                LOG.Error("Unable to process HTTP request: HTTP method {0} unsupported.",
                    context.Request.HttpMethod);
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;    // 405.
                return;
            }

            HttpListenerRequest request = context.Request;

            if (request.Url == null || string.IsNullOrWhiteSpace(request.Url.AbsoluteUri))
            {
                LOG.Error(
                    "Unable to process {0} HTTP request: No URI specified.",
                    context.Request.HttpMethod);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;  // 400.
                return;
            }

            LOG.Debug("Request.Uri: {0}", request.Url);

            // After requesting a web page modern browsers will automatically make a second 
            //  request to get the favicon.ico to display.  Return an empty response for a 
            //  favicon.ico request.
            if (request.HttpMethod.ToUpper() == HttpMethod.Get
                && request.Url.ToString().Contains("favicon.ico"))
            {
                LOG.Debug("favicon.ico request.  Response will be empty.");
                context.Response.StatusCode = (int)HttpStatusCode.OK;  // 200.
                this.WriteResponseBody(context.Response, "", DEF_CHARSET);
                return;
            }

            IHttpMethodHandler methodHandler = this.GetMethodHandler(context);
            if (methodHandler == null)
            {
                LOG.Error(
                    "Unable to process HTTP request: "
                    + "Could not retrieve method handler for {0} HTTP method.",
                    context.Request.HttpMethod);
                context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;   // 501.
                return;
            }
            
            LOG.Info("Calling {0} method handler to process {1} request for resource {2}...",
                methodHandler.GetType().Name, request.HttpMethod, request.Url.AbsolutePath);

            methodHandler.ProcessHttpRequest(context);
        }

        private void Validate(HttpListenerContext context)
        {
            LOG.Debug("Validating HTTP request...");

            string errorMessage;
            if (context == null)
            {
                errorMessage = "Unable to process HTTP request: No HTTP listener context supplied.";
                LOG.Error(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            HttpListenerRequest request = context.Request;

            if (request == null)
            {
                errorMessage = "Unable to process HTTP request: No HTTP request supplied.";
                LOG.Error(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            LOG.Debug("HTTP request validated.");
        }

        private bool IsHttpMethodSupported(HttpListenerContext context)
        {
            if (!HttpMethod.List.Contains(context.Request.HttpMethod.ToUpper()))
            {
                return false;
            }

            return true;
        }

        private IHttpMethodHandler GetMethodHandler(HttpListenerContext context)
        {
            return this.MethodHandlers.GetValueOrNull(context.Request.HttpMethod.ToUpper());
        }
    }
}