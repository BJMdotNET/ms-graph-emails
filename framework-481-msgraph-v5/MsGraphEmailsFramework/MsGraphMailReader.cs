using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Graph.Users.Item.SendMail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace MsGraphEmailsFramework
{
    internal class MsGraphMailReader : MsGraphService
    {
        private static HttpClient _httpClient;

        public MsGraphMailReader()
        {
            var httpClientHandler = HttpClientHandlerRetriever.Execute(MailConfiguration.MsGraph.UseProxy, true);

            _httpClient = HttpClientRetriever.Execute(httpClientHandler);
        }

        public async Task Execute()
        {
            Trace.TraceInformation($"{GetType().Name} -> Execute");

            try
            {
                if (GraphServiceClientToBeInitiated())
                {
                    Trace.TraceInformation("Calling SetupGraphClient");

                    SetupGraphServiceClient();
                }

                var sendEmailRequestInformation = GraphServiceClient
                    .Users[MailConfiguration.Email.Sender]
                    .MailFolders
                    .ToGetRequestInformation();

                var httpRequestMessage = await GraphServiceClient
                    .RequestAdapter
                    .ConvertToNativeRequestAsync<HttpRequestMessage>(sendEmailRequestInformation)
                    .ConfigureAwait(false);

                var results = string.Empty;

                try
                {
                    using (var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        switch (httpResponseMessage.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                results = await httpResponseMessage.Content.ReadAsStringAsync();
                                //Trace.TraceInformation(releaseNumber, type, questionId, results);
                                break;

                            case HttpStatusCode.Unauthorized:
                                throw new HttpRequestException($"Unauthorized request ({httpResponseMessage.StatusCode})");

                            default:
                                var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync();
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
                            var responseText = await reader.ReadToEndAsync();

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

                //var messages = await GraphServiceClient
                //    .Users[MailConfiguration.Email.Sender]
                //    .MailFolders[]
                //    // .MailFolders["inbox"]
                //    .Messages
                //    .Request()
                //    // .Filter($"startswith(subject, '{subject}') and receivedDateTime gt {dt}")
                //    // .Top(20)
                //    .GetAsync();

                //List<QueryOption> options = new List<QueryOption>
                //{
                //    new QueryOption("$filter", "startswith(displayName,'the specific user's mail')")
                //};
                //var users = await graphClient.Users.Request(options).GetAsync();

                //foreach (var message in messages)
                //{

                //}

                //var credentials = new ClientSecretCredential(
                //    config["GraphMail:TenantId"],
                //    config["GraphMail:ClientId"],
                //    config["GraphMail:ClientSecret"],
                //    new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });

                //// Define a new Microsoft Graph service client.            
                //GraphServiceClient _graphServiceClient = new GraphServiceClient(credentials);

                //// Get the e-mails for a specific user.
                //var messages = _graphServiceClient.Users["user@e-mail.com"].Messages.Request().GetAsync().Result;

                //-------------------
                // MOVE MESSAGES
                /*
                 IUserMailFoldersCollectionPage allMailFolders = await graphClient.Users[inbox].MailFolders.Request().GetAsync();
        foreach(MailFolder folder in allMailFolders)
        {
            if (folder.DisplayName == "01 Processed")
            {
                processedFolder = folder.Id;
            }
            if (folder.DisplayName == "02 Needs Attention")
            {
                needsAttentionFolder = folder.Id;
            }
        }

    if (needsAttention)
                {
                    needsAttentionCount++;
                    Message movedMsg = await graphClient.Users[inbox].Messages[message.Id].Move(needsAttentionFolder).Request().PostAsync(); 
                }
               else
                {
                     Message movedMsg = await graphClient.Users[inbox].Messages[message.Id].Move(processedFolder).Request().PostAsync();
                }
                 */
            }
            catch (ODataError odataError)
            {
                if (odataError.Error != null)
                {
                    Trace.TraceError($"MsGraphMailService: odataError.Error.Code = {odataError.Error.Code}");
                    Trace.TraceError($"MsGraphMailService: odataError.Error.Message = {odataError.Error.Message}");
                    Trace.TraceError($"MsGraphMailService: odataError.Error = {odataError.Error}");
                }

                throw;
            }
            catch (Exception exc)
            {
                var exceptionMessage = ExceptionMessageRetriever.Execute(exc);

                Trace.TraceError($"MsGraphMailService: Exception: {exceptionMessage}");
                Trace.TraceError($"MsGraphMailService: Exception: {exc}");

                throw;
            }
        }
    }
}
