using Microsoft.Kiota.Abstractions;
using MsGraphEmailsFramework.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MsGraphEmailsFramework
{
    internal abstract class MsGraphMailHandler : MsGraphService
    {
        private static HttpClient _httpClient;

        protected MsGraphMailHandler()
        {
            var httpClientHandler = HttpClientHandlerRetriever.Execute(MailConfiguration.MsGraph.UseProxy, true);

            _httpClient = HttpClientRetriever.Execute(httpClientHandler);
        }

        protected async Task<string> GetJson(RequestInformation requestInformation)
        {
            var httpRequestMessage = await GraphServiceClient
                .RequestAdapter
                .ConvertToNativeRequestAsync<HttpRequestMessage>(requestInformation)
                .ConfigureAwait(false);

            var results = string.Empty;

            try
            {
                using (var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                {
                    switch (httpResponseMessage.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            results = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                            //Trace.TraceInformation(releaseNumber, type, questionId, results);
                            break;

                        case HttpStatusCode.Unauthorized:
                            throw new HttpRequestException($"Unauthorized request ({httpResponseMessage.StatusCode})");

                        default:
                            var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new HttpRequestException($"Bad request ({httpResponseMessage.StatusCode}, {contentAsString})");
                    }
                }
            }
            catch (WebException webException)
            {
                Trace.TraceError($"Error! " + webException);
                Trace.TraceError(ExceptionMessageRetriever.Execute(webException));

                var responseStream = webException.Response?.GetResponseStream();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        var responseText = await reader.ReadToEndAsync().ConfigureAwait(false);

                        Trace.TraceError(responseText);

                        results = responseText;
                    }
                }
            }
            catch (Exception exception)
            {
                Trace.TraceError($"Error! " + exception);
                Trace.TraceError(ExceptionMessageRetriever.Execute(exception));

                throw;
            }

            return results;
        }
    }
}
