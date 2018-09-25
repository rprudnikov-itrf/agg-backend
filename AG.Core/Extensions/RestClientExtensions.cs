using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Net;

namespace RestSharp
{
    public static class RestClientExtensions
    {
        private const int MAX_RETRY = 15;
        private const int SLEEP_RETRY = 1000;

        public static IRestResponse PostRetryNotFound(this IRestClient client, IRestRequest request, int maxRetry = MAX_RETRY)
        {
            IRestResponse response = null;

            for (int i = 0; i < maxRetry; i++)
            {
                response = client.Post(request);
                if (response.StatusCode == HttpStatusCode.OK)
                    break;
            }

            return response;
        }

        public static IRestResponse<T> PostRetry<T>(this IRestClient client, IRestRequest request) where T : new()
        {
            IRestResponse<T> response = null;

            for (int i = 0; i < MAX_RETRY; i++)
            {
                response = client.Post<T>(request);
                if (!response.IsRetry())
                    break;
            }

            return response;
        }

        public static IRestResponse<T> GetRetry<T>(this IRestClient client, IRestRequest request) where T : new()
        {
            IRestResponse<T> response = null;

            for (int i = 0; i < MAX_RETRY; i++)
            {
                response = client.Get<T>(request);
                if (!response.IsRetry())
                    break;

                System.Threading.Thread.Sleep(SLEEP_RETRY);
            }

            return response;
        }

        public static IRestResponse<T> PutRetry<T>(this IRestClient client, IRestRequest request) where T : new()
        {
            IRestResponse<T> response = null;

            for (int i = 0; i < MAX_RETRY; i++)
            {
                response = client.Put<T>(request);
                if (!response.IsRetry())
                    break;
            }

            return response;
        }

        private static bool IsRetry<T>(this IRestResponse<T> response)
        {
            if (response.ResponseStatus == ResponseStatus.Completed)
                return false;

            var webException = response.ErrorException as WebException;
            return webException != null && (webException.Status == WebExceptionStatus.KeepAliveFailure
                || response.StatusCode == HttpStatusCode.BadGateway
                || response.StatusCode == HttpStatusCode.NotImplemented
                || response.StatusCode == HttpStatusCode.InternalServerError);
        }
    }
}
