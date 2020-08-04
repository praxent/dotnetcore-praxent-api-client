using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Praxent.API.Client
{
    public interface IRESTAPIClient
    {
        HttpRequestMessage CreateRequestMessage(HttpMethod method, string uri, Dictionary<string, string> headers, string requestBody = null);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string apiName);
        Task<TResponse> SendAsync<TRequest, TResponse>(HttpMethod method, string uri, Dictionary<string, string> headers, TRequest requestObj, string apiName);
        Task<TResponse> SendAsync<TResponse>(HttpMethod method, string uri, Dictionary<string, string> headers, string apiName);
    }
}