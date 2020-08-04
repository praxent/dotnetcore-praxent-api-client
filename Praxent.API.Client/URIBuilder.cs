using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;

namespace Praxent.API.Client
{
    public class URIBuilder : IURIBuilder
    {
        public string BuildUriStringForRequest(string uri, IDictionary<string, string> queryStringCollection)
        {
            if (queryStringCollection != null)
            {
                return QueryHelpers.AddQueryString(uri, queryStringCollection);
            }

            return uri;
        }
    }
}