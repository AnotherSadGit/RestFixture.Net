using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers;
using NLog;

namespace FitNesseTestServer.Test.FitNesse.Fixture
{
    public class HttpServer
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private HttpListener _listener = null;
        private int _port;
        // Prefix must end in a forward slash.  See MSDN HttpListener class documentation:
        //  https://msdn.microsoft.com/en-us/library/system.net.httplistener(v=vs.110).aspx.
        private readonly string _prefixTemplate = "http://localhost:{0}/";

        #region Constructors **********************************************************************

        public HttpServer(int port)
            : this(port, GetRequestHandler())
        {
        }

        public HttpServer(int port, IHttpRequestHandler requestHandler)
        {
            _listener = new HttpListener();
            this.Port = port;
            this.RequestHandler = requestHandler;
        }

        #endregion

        #region Properties ************************************************************************

        // Java version: Property name was Server.
        protected internal virtual HttpListener Listener
        {
            get
            {
                return _listener;
            }
        }

        private IHttpRequestHandler RequestHandler { get; set; }

        private int Port
        {
            set
            {
                SetPrefix(value);

                this._port = value;
            }
            get
            {
                return _port;
            }
        }

        public virtual bool Started
        {
            get
            {
                return CheckIsListening();
            }
        }

        public virtual bool Stopped
        {
            get
            {
                return !CheckIsListening();
            }
        }

        #endregion

        #region Methods ***************************************************************************

        public virtual string Start()
        {
            string ret = null;
            LOG.Debug("Starting HTTP server listening on port {0}...", Port);
            try
            {
                Listener.Start();

                int timeoutSeconds = 3;
                bool isListening = CheckIsListening(timeoutSeconds);

                if (!isListening)
                {
                    LOG.Error("Failed to start HTTP server listening on port {0}.", Port);
                    return "Failed to start HTTP server";
                }

                LOG.Debug("Calling Listener.BeginGetContext from main thread...");
                RequestProcessorParameter arguments =
                    new RequestProcessorParameter(this.Listener, this.RequestHandler);
                Listener.BeginGetContext(ProcessHttpRequest, arguments);
                LOG.Debug("Continuing main thread after Listener.BeginGetContext...");

                LOG.Debug("HTTP server listening on port {0} has started.", Port);

                return "OK";
            }
            catch (Exception e)
            {
                LOG.Error(e, "Failed to start HTTP server listening on port {0}.", Port);
                return "Failed to start HTTP server: " + e.Message;
            }
        }

        public virtual string Stop()
        {
            string ret = null;
            LOG.Debug("Stopping HTTP server listening on port {0}...", Port);
            try
            {
                Listener.Stop();

                int timeoutSeconds = 3;
                bool isListening = CheckIsListening(timeoutSeconds);

                if (isListening)
                {
                    LOG.Error("Failed to stop HTTP server listening on port {0}.", Port);
                    return "Failed to stop HTTP server";
                }

                LOG.Debug("Closing listener...");
                Listener.Close();
                LOG.Debug("Listener closed.");

                LOG.Debug("HTTP server listening on port {0} has stopped.", Port);
                return "OK";
            }
            catch (Exception e)
            {
                LOG.Error(e, "Failed to stop HTTP server listening on port {0}.", Port);
                return "Failed to stop HTTP server: " + e.Message;
            }
        }

        private void SetPrefix(int port)
        {
            if (Listener == null)
            {
                LOG.Error("Unable to set listener's port to {0}: Listener does not exist.", port);
                return;
            }

            string newPrefix = string.Format(_prefixTemplate, port);

            // Check the new prefix doesn't already exist as HttpListener doesn't like that.
            if (Listener.Prefixes != null && Listener.Prefixes.Count > 0
                && Listener.Prefixes.Contains(newPrefix))
            {
                LOG.Warn("Listener is already listening on port {0}.", port);
                return;
            }

            LOG.Debug("Setting listener's port to {0}...", port);

            // Only want to listen on a single port.  If previously listening on a different port, 
            //  clear that previous config so that will now only listen on the new port.
            Listener.Prefixes.Clear();
            Listener.Prefixes.Add(newPrefix);

            LOG.Debug("Listener's port set to {0}.", port);
        }

        private static IHttpRequestHandler GetRequestHandler()
        {
            IDictionary<string, IHttpMethodHandler> methodHandlers =
                new Dictionary<string, IHttpMethodHandler>();
            methodHandlers.Add(HttpMethod.Get, new HttpGetHandler());
            methodHandlers.Add(HttpMethod.Post, new HttpPostHandler());
            methodHandlers.Add(HttpMethod.Put, new HttpPutHandler());
            methodHandlers.Add(HttpMethod.Delete, new HttpDeleteHandler());

            IHttpRequestHandler requestHandler = new HttpRequestHandler(methodHandlers);

            return requestHandler;
        }

        private bool CheckIsListening()
        {
            if (Listener == null)
            {
                return false;
            }
            return Listener.IsListening;
        }

        private bool CheckIsListening(int timeoutSeconds)
        {
            if (Listener == null)
            {
                return false;
            }

            DateTime timeoutTime = DateTime.Now.AddSeconds(timeoutSeconds);
            while (!Listener.IsListening && DateTime.Now < timeoutTime)
            {
                Task.Delay(100).Wait();
            }

            return Listener.IsListening;
        }
        void ProcessHttpRequest(IAsyncResult result)
        {
            RequestProcessorParameter arguments = (RequestProcessorParameter)result.AsyncState;
            HttpListener listener = arguments.Listener;
            IHttpRequestHandler requestHandler = arguments.RequestHandler;

            LOG.Debug("Handling HTTP request...");

            if (listener == null)
            {
                LOG.Error("Unable to handle HTTP request: No listener supplied.");
                return;
            }

            if (requestHandler == null)
            {
                LOG.Error("Unable to handle HTTP request: No request handler supplied.");
                // No point in calling listener.BeginGetContext since there is no request handler 
                //  to pass in as argument.
                return;
            }

            if (requestHandler.MethodHandlers == null || requestHandler.MethodHandlers.Count == 0)
            {
                LOG.Error("Unable to handle HTTP request: No HTTP method handlers supplied.");
                // No point in calling listener.BeginGetContext since there is no method handler 
                //  to pass in as argument.
                return;
            }

            HttpListenerContext context = null;

            try
            {
                context = listener.EndGetContext(result);
                LOG.Debug("Listener.EndGetContext complete.");
            }
            catch (Exception ex)
            {
                LOG.Debug("{0}: {1}", ex.GetType().Name, ex.Message);

                // If the main thread has closed the listener then Listener.EndGetContext throws 
                //  an ObjectDisposedException or an HttpListenerException.  There is no neat way 
                //  around this, such as checking IsDisposed or IsDisposing, since HttpListener 
                //  doesn't have any properties like that.  So the only way to deal with it is to 
                //  catch the exception and do nothing with it, allowing execution to continue.
                if (ex is ObjectDisposedException || ex is HttpListenerException)
                {
                    LOG.Debug("Exiting ProcessHttpRequest method.");
                    return;
                }
            }

            LOG.Debug("Calling HttpRequestHandler to process request...");
            requestHandler.ProcessHttpRequest(context);

            LOG.Debug("Completed handling HTTP of request.");

            LOG.Debug("Calling listener.BeginGetContext from within request handler...");
            listener.BeginGetContext(ProcessHttpRequest, arguments);
            LOG.Debug("Request handler finished.");
        }

        #endregion
    }
}