using System.Collections.Generic;

namespace Praxent.API.Client
{
    public interface IURIBuilder
    {
        string BuildUriStringForRequest(string uri, IDictionary<string, string> queryStringCollection);
    }
}