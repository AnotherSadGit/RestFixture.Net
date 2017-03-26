using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using RestClient.Data;
using RestClient.Helpers;
using RestSharp;

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
namespace RestClient
{
    /// <summary>
    /// A client for making HTTP requests.
    /// </summary>
    /// <remarks>Very loosely based on the original Java version of RestClient.RestClientImpl.</remarks>
	public class RestClientImpl
	{
        private static NLog.Logger LOG = LogManager.GetCurrentClassLogger();

        private string _baseAddress;
        public string BaseAddress
        {
            get { return _baseAddress; }
            set
            {
                string _baseUri = value;
                if (!string.IsNullOrWhiteSpace(_baseUri))
                {
                    Client.BaseUrl = new Uri(_baseUri);
                }
            }
        }

        private RestSharp.IRestClient _client;
        public RestSharp.IRestClient Client
        {
            get
            {
                if (_client == null)
                {
                    if (!string.IsNullOrWhiteSpace(_baseAddress))
                    {
                        _client = new RestSharp.RestClient(_baseAddress);
                    }
                    else
                    {
                        _client = new RestSharp.RestClient();
                    }
                }

                return _client;
            }
        }

        /// <summary>
        /// Initializes a new instance of the HttpClient class.
        /// </summary>
        public RestClientImpl()
        {

        }

        /// <summary>
        /// Initializes a new instance of the HttpClient class with the specified URL.
        /// </summary>
        public RestClientImpl(string baseAddress)
        {
            this.BaseAddress = baseAddress;
        }

        // Unfortunately both RestClient, used by the Java RestFixture which is being ported 
        //  to .NET, and RestSharp have classes called RestRequest and RestResponse.  So we must 
        //  always specify which RestRequest or RestResponse class we're using.

        public Data.RestResponse execute(Data.RestRequest requestDetails)
        {
            return execute(this.Client, requestDetails);
        }

        public Data.RestResponse execute(string hostAddress, 
            Data.RestRequest requestDetails)
        {
            Uri hostUri = GetHostUri(hostAddress);
            IRestClient client = new RestSharp.RestClient(hostUri);
            return execute(client, requestDetails);
        }

        private Data.RestResponse execute(RestSharp.IRestClient client,
            Data.RestRequest requestDetails)
        {
            if (client == null)
            {
                client = this.Client;
            }

            if (client.BaseUrl == null || string.IsNullOrWhiteSpace(client.BaseUrl.AbsoluteUri))
            {
                client.BaseUrl = new Uri(this.BaseAddress);
            }

            // We're using RestSharp to execute HTTP requests.  RestSharp copes with trailing "/" 
            //  on BaseUrl and leading "/" on Resource, whether one or both of the "/" are present 
            //  or absent.

            ValidateRequest(client, requestDetails);

            if (requestDetails.TransactionId == null)
            {
                requestDetails.TransactionId = Convert.ToInt64(DateTimeHelper.CurrentUnixTimeMillis());
            }
            
            client.FollowRedirects = requestDetails.FollowRedirect;

            RestSharp.RestRequest request = BuildRequest(requestDetails);

            if (LOG.IsDebugEnabled)
            {
                try
                {
                    LOG.Info("Http Request : {0} {1}",
                        request.Method, client.BuildUri(request).AbsoluteUri);
                }
                catch (Exception e)
                {
                    LOG.Error(e, "Exception in debug : {0}", e.Message);
                }

                // Request Header
                LogRequestHeaders(request);

                // Request Body
                if (IsEntityEnclosingMethod(request.Method))
                {
                    try
                    {
                        Parameter body = request.Parameters
                            .Where(p => p.Type == ParameterType.RequestBody).FirstOrDefault();
                        LOG.Debug("Http Request Body : {0}", body.Value);
                    }
                    catch (IOException e)
                    {
                        LOG.Error(e, "Error in reading request body in debug : {0}", e.Message);
                    }
                }
            }

            // Prepare Response
            Data.RestResponse responseDetails = new Data.RestResponse();
            responseDetails.TransactionId = requestDetails.TransactionId;
            responseDetails.Resource = requestDetails.Resource;
            try
            {
                IRestResponse response = client.Execute(request);
                
                foreach (RestSharp.Parameter responseHeader in response.Headers)
                {
                    responseDetails.addHeader(responseHeader.Name, responseHeader.Value.ToString());
                }

                responseDetails.StatusCode = (int) response.StatusCode;
                responseDetails.StatusText = response.StatusDescription;
                // The Java byte data type is actually a signed byte, equivalent to sbyte in .NET.
                //  Data.RestResponse is a direct port of the Java class so it uses 
                //  the sbyte data type.  The strange conversion from byte[] to sbyte[] was from 
                //  the answer to Stackoverflow question http://stackoverflow.com/questions/25759878/convert-byte-to-sbyte
                // TODO: Check RestFixture code to see whether Data.RestResponse.RawBody can be converted to byte[].
                responseDetails.RawBody = (sbyte[])(Array)(response.RawBytes);

                // Debug
                if (LOG.IsDebugEnabled)
                {
                    // Not necessarily the same as the request URI.  The response URI is the URI 
                    //  that finally responds to the request, after any redirects.
                    LOG.Debug("Http Request Path : {0}", response.ResponseUri);
                    LogRequestHeaders(request);
                    // Apache HttpClient.HttpMethod.getStatusLine() returns the HTTP response 
                    //  status line.  Can't do this with RestSharp as it cannot retrieve the 
                    //  protocol version which is at the start of the status line.  So just log 
                    //  the status code and description.
                    // Looks like they added ProtocolVersion to RestSharp in issue 795 (commit 
                    //  52c18a8) but it's only available in the FRAMEWORK builds.
                    LOG.Debug("Http Response Status : {0} {1}", 
                        (int)response.StatusCode, response.StatusDescription);
                    LOG.Debug("Http Response Body : {0}", response.Content);
                }

                if (IsResponseError(response))
                {
                    LOG.Error(response.ErrorException, response.ErrorMessage);
                    string message = "Http call failed";
                    if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
                    {
                        message = response.ErrorMessage.Trim();
                    }
                    throw new System.InvalidOperationException(message, response.ErrorException);
                }

            }
            catch (Exception e)
            {
                string message = "Http call failed";
                throw new System.InvalidOperationException(message, e);
            }

            LOG.Debug("response: {0}", responseDetails);
            return responseDetails;
        }

        /// <summary>
        /// Builds the HTTP request that will be sent.
        /// </summary>
        /// <param name="requestDetails">The RestClient.RestRequest containing details of the request 
        /// to send.</param>
        /// <returns>A RestSharp.RestRequest object containing details of the request to send.</returns>
        /// <remarks>Loosely based on RestClient.RestClientImpl.configureHttpMethod, which is 
        /// designed around the Apache HttpClient library, written in Java.  This is very different 
        /// from the .NET System.Net library so cannot be ported directly; the HttpClient must be 
        /// rewritten from scratch.</remarks>
        private RestSharp.RestRequest BuildRequest(Data.RestRequest requestDetails)
        {
            Uri resourceUri = GetResourceUri(requestDetails);
            RestSharp.Method httpMethod = GetHttpMethod(requestDetails);

            RestSharp.RestRequest request = new RestSharp.RestRequest(resourceUri, httpMethod);

            AddHttpRequestHeaders(request, requestDetails);

            if (IsEntityEnclosingMethod(httpMethod))
            {
                string fileName = requestDetails.FileName;
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    if (!File.Exists(fileName))
                    {
                        throw new ArgumentException("File not found: " + fileName);
                    }
                    request.AddFile("file", fileName);
                }
                else
                {
                    // Add Multipart
                    LinkedHashMap<string, Data.RestMultipart> 
                        multipartFiles = requestDetails.MultipartFileNames;
                    if ((multipartFiles != null) && (multipartFiles.Count > 0))
                    {
                        ConfigureRequestMultipartFileUpload(request, requestDetails, multipartFiles);
                    }
                    else if (!string.IsNullOrWhiteSpace(requestDetails.Body))
                    {
                        string contentType = null;
                        if (requestDetails.ContentType.ToLower().Contains("json"))
                        {
                            request.RequestFormat = DataFormat.Json;
                            contentType = requestDetails.ContentType;
                        }
                        else
                        {
                            request.RequestFormat = DataFormat.Xml;
                            if (requestDetails.ContentType.ToLower().Contains("xml"))
                            {
                                contentType = requestDetails.ContentType;
                            }
                            else
                            {
                                // RestSharp supports only two RequestFormats: JSON and XML.
                                contentType = "application/xml";
                            }
                        }

                        // requestDetails.Body is a string, and RestRequest.AddBody, AddJsonBody 
                        //  and AddXmlBody take objects as arguments, not strings.  Although they 
                        //  won't throw exceptions, the result is not as expected if an object is 
                        //  serialized to JSON or XML before being passed to one of the AddBody 
                        //  methods.  
                        //
                        //  For example, if a Book object is serialized to JSON and the JSON string 
                        //  is passed to AddBody or AddJsonBody the body sent across the wire is:
                        //
                        //  "{\"Id\":10,\"Title\":\"The Meaning of Liff\",\"Year\":1983}"
                        //
                        //  when it should be:
                        //
                        //  {Id: 10, Title: "The Meaning of Liff", Year: 1983}

                        //  The incorrect format of the JSON in the request body will prevent it 
                        //  from being desrialized correctly at the server end.

                        // Instead of using one of the AddBody methods, use 
                        //  AddParameter(..., ..., ParameterType.RequestBody) 
                        //  which can handle objects serialized to JSON or XML strings.
                        request.AddParameter(contentType, requestDetails.Body,
                            ParameterType.RequestBody);
                    }
                }
            }

            return request;
        }

        private void ValidateRequest(RestSharp.IRestClient client,
            Data.RestRequest requestDetails)
        {
            // Client cannot be null.
            if (client.BaseUrl == null || string.IsNullOrWhiteSpace(client.BaseUrl.AbsoluteUri))
            {
                throw new ArgumentException("Host address is not set: "
                    + "Please pass a valid host address.");
            }

            if (requestDetails == null)
            {
                throw new System.ArgumentException("HTTP request details not supplied.");
            }

            if (requestDetails.HttpMethod == Data.RestRequest.Method.NotSet)
            {
                throw new System.ArgumentException(
                    "HTTP request method is not set (eg \"GET\", \"POST\").");
            }

            if (string.IsNullOrWhiteSpace(requestDetails.Resource))
            {
                throw new System.ArgumentException("Relative URL of resource is not set.");
            }
        }

        private Uri GetHostUri(string hostAddress)
        {
            if (string.IsNullOrWhiteSpace(hostAddress))
            {
                return null;
            }

            // Ensure there is a trailing "/".
            hostAddress = hostAddress.TrimEnd('/');
            hostAddress = hostAddress + '/';

            return new Uri(hostAddress);
        }

        private Uri GetResourceUri(Data.RestRequest requestDetails)
        {
            string queryString = GetHttpRequestQueryString(requestDetails);
            // C# behaves differently from Java when concatenating nulls:
            //  Java: "string" + null = "stringnull" (the string "null" is substituted for null);
            //  C#: "string" + null = "string" (the empty string is substituted for null).
            string uriString = requestDetails.Resource + queryString;
            // .NET System.Uri constructor will always check whether all characters are escaped 
            //  and escape those that aren't so no need to specify whether the URI is escaped or 
            //  not.
            Uri uri = null;
            try
            {
                // Assume this is a relative URL as host address, passed into the Execute method 
                //  separately, is invalid if null or blank.
                uri = new Uri(uriString, UriKind.Relative);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Problem when building URI: " + uriString, ex);
            }

            return uri;
        }

        private string GetHttpRequestQueryString(Data.RestRequest requestDetails)
        {
            string queryString = requestDetails.Query;
            if (StringHelper.IsNullOrBlank(queryString))
            {
                return string.Empty;
            }

            // Ensure query string starts with "?".
            queryString = queryString.TrimStart('?').Trim();
            queryString = "?" + queryString;
            return queryString;
        }

        private RestSharp.Method GetHttpMethod(Data.RestRequest restRequest)
        {
            // No way to tell if restRequest.Method has not been set.  If not set explicitly it 
            //  will have value 0 = Get.
            RestSharp.Method httpMethod;
            bool ignoreCase = true;
            if (Enum.TryParse(restRequest.HttpMethod.ToString(), ignoreCase, out httpMethod))
            {
                return httpMethod;
            }

            // As at v105.2.3, on 26 Feb 2017, RestSharp is unable to handle TRACE HTTP methods, 
            //  although the RestClient in the Java RestFixture can.
            string errorMessage = string.Format("RestRequst.HttpMethod {0} cannot be handled "
                                                + "by RestSharp.",
                                                restRequest.HttpMethod);
            throw new ArgumentException(errorMessage);
        }

        private void AddHttpRequestHeaders(RestSharp.RestRequest requestToBuild,
            Data.RestRequest requestDetails)
        {
            // NB: RestSharp will default content type to XML if not set explicitly.

            foreach (Data.RestData.Header header in requestDetails.Headers)
            {
                requestToBuild.AddHeader(header.Name, header.Value);
            }
        }

        private bool IsEntityEnclosingMethod(RestSharp.Method httpMethod)
        {
            // Original Java code in RestClientImpl.configureHttpMethod:
            //  if (m is EntityEnclosingMethod) ... 
            //  where m is of type org.apache.commons.httpclient.HttpMethod
            // Looked up derived types of EntityEnclosingMethod via 
            //  http://grepcode.com/search/usages?id=repo1.maven.org$maven2@com.ning$metrics.action@0.2.7@org$apache$commons$httpclient$methods@EntityEnclosingMethod&type=type&k=u 
            // There were only two: PutMethod and PostMethod.
            // We'll add Patch as well.
            if (httpMethod == RestSharp.Method.PUT || httpMethod == RestSharp.Method.POST
                || httpMethod == RestSharp.Method.PATCH)
            {
                return true;
            }
            return false;
        }

        private void ConfigureRequestMultipartFileUpload(RestSharp.RestRequest request, 
            Data.RestRequest requestDetails,
            LinkedHashMap<string, Data.RestMultipart> multipartFiles)
        {
            request.AddHeader("Content-Type", "multipart/form-data");

            foreach (KeyValuePair<string, RestMultipart> multipartFile in multipartFiles)
            {
                ConfigureMultipart(request, multipartFile.Key, multipartFile.Value);
            }
        }

        private void ConfigureMultipart(RestSharp.RestRequest request, 
            string fileParamName, RestMultipart restMultipart)
        {
            RestMultipart.RestMultipartType type = restMultipart.Type;
            switch (type)
            {
                case RestMultipart.RestMultipartType.FILE:
                    string fileName = null;
                    try
                    {
                        fileName = restMultipart.Value;
                        using (var fileContent = new FileStream(fileName, FileMode.Open))
                        {
                            request.AddFile(fileName, fileContent.CopyTo, fileName, 
                                restMultipart.ContentType);
                        }

                        LOG.Info("Configure Multipart file upload paramName={0} :  ContentType={1} for  file={2} ", 
                            new string[] { fileParamName, restMultipart.ContentType, fileName });
                        break;
                    }
                    catch (FileNotFoundException e)
                    {
                        throw new System.ArgumentException("File not found: " + fileName, e);
                    }
                case RestMultipart.RestMultipartType.STRING:
                    // According to a comment on RestSharp issue 524, 
                    //  https://github.com/restsharp/RestSharp/issues/524 , it's only possible to 
                    //  add string data in multipart form data using ParameterType GetOrPost.  
                    //  ParameterType RequestBody doesn't work.
                    request.AddParameter(fileParamName, restMultipart.Value,
                        restMultipart.ContentType, ParameterType.GetOrPost);
                    LOG.Info("Configure Multipart String upload paramName={0}:  ContentType={1} ", 
                        fileParamName, restMultipart.ContentType);
                    break;
                default:
                    throw new System.ArgumentException("Unknonw Multipart Type: " + type);
                    break;
            }

        }

        private void LogRequestHeaders(RestSharp.RestRequest request)
        {
            if (LOG.IsDebugEnabled)
            {
                LOG.Debug("Http Request Headers : [{0}]",
                    string.Join(", ",
                        request.Parameters.Where(p => p.Type == ParameterType.HttpHeader)));
            }
        }

        private bool IsResponseError(RestSharp.IRestResponse response)
        {
            if (response == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(response.ErrorMessage)
                && response.ErrorException == null)
            {
                return false;
            }

            return true;
        }
    }
}
