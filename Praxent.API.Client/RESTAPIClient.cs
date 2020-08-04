using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Praxent.API.Client
{
    public class RESTAPIClient : IRESTAPIClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<RESTAPIClient> _logger;

        public RESTAPIClient(IHttpClientFactory clientFactory,
            ILogger<RESTAPIClient> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public HttpRequestMessage CreateRequestMessage(HttpMethod method, string uri, Dictionary<string, string> headers, string requestBody = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (requestBody != null)
            {
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            }

            foreach (var headerKey in headers.Keys)
            {
                request.Headers.Add(headerKey, headers[headerKey]);
            }

            return request;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string apiName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(apiName))
                {
                    var client = _clientFactory.CreateClient();
                    // Replacing Host
                    request.Headers.Add("Host", request.RequestUri.Host);
                    var response = await client.SendAsync(request);
                    return response;
                }
                else
                {
                    var client = _clientFactory.CreateClient(apiName);
                    // Replacing Host
                    request.Headers.Add("Host", client.BaseAddress.Host);
                    var response = await client.SendAsync(request);
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an exception occurred sending request");
                throw;
            }
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(HttpMethod method, string uri, Dictionary<string, string> headers, TRequest requestObj, string apiName)
        {
            var responseContentString = string.Empty;
            string requestBody = null;
            if (requestObj != null)
            {
                requestBody = JsonConvert.SerializeObject(requestObj);
            }
            var httpRequestMessage = CreateRequestMessage(method, uri, headers, requestBody);
            try
            {
                var responseModel = await Send<TResponse>(httpRequestMessage, apiName);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured on route: {httpRequestMessage.RequestUri}. ResponseContent : {responseContentString}");
                _logger.LogError(ex, $"an exception occurred sending request {uri}");
                //Uncomment for Troubleshooting outside of Prod.
                //throw new Exception($"an error occured on route: {httpRequestMessage.RequestUri}. RequestContent : {requestBody}. ResponseContent : {responseContentString}. Inner Exception : {ex.Message}", ex);
                throw new Exception($"an error occured on route: {httpRequestMessage.RequestUri}. Inner Exception : {ex.Message}", ex);
            }
        }

        public async Task<TResponse> SendAsync<TResponse>(HttpMethod method, string uri, Dictionary<string, string> headers, string apiName)
        {
            var responseContentString = string.Empty;
            var httpRequestMessage = CreateRequestMessage(method, uri, headers);
            try
            {
                var responseModel = await Send<TResponse>(httpRequestMessage, apiName);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured on route: {httpRequestMessage.RequestUri}. ResponseContent : {responseContentString}");
                _logger.LogError(ex, $"an exception occurred sending request {uri}");
                throw new Exception($"an error occured on route: {httpRequestMessage.RequestUri}. ResponseContent : {responseContentString}. Inner Exception : {ex.Message}", ex);
            }
        }

        private async Task<TResponse> Send<TResponse>(HttpRequestMessage httpRequestMessage, string apiName)
        {
            var httpResponseMessage = await SendAsync(httpRequestMessage, apiName);
            var responseContentString = await httpResponseMessage.Content.ReadAsStringAsync();
            httpResponseMessage = httpResponseMessage.EnsureSuccessStatusCode();
            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                throw new HttpRequestException("Response was 404 instead of Success!");
            }
            var responseModel = JsonConvert.DeserializeObject<TResponse>(responseContentString);
            return responseModel;
        }
    }
}