using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Graph.Users.Item.SendMail;
using MsGraphEmailsFramework.Common;
using MsGraphEmailsFramework.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MsGraphEmailsFramework
{
    internal class MsGraphMailSender : MsGraphService
    {
        private static HttpClient _httpClient;

        public MsGraphMailSender()
        {
            var httpClientHandler = HttpClientHandlerRetriever.Execute(MailConfiguration.MsGraph.UseProxy, true);
            _httpClient = new HttpClient(httpClientHandler);
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CustomUserAgent", "1.0"));
        }

        public async Task SendMail(MailMessage mailMessage)
        {
            //Trace.TraceInformation($"{GetType().Name} -> SendMail");

            try
            {
                if (GraphServiceClientToBeInitiated())
                {
                    Trace.TraceInformation($"{GetType().Name}: Calling SetupGraphClient");

                    SetupGraphServiceClient();
                }

                Trace.TraceInformation($"{GetType().Name}: Sender: {MailConfiguration.Email.Sender}");

                var message = new Message
                {
                    From = StringToGraphRecipient(MailConfiguration.Email.Sender),
                    ToRecipients = StringsToGraphRecipients(mailMessage.ToRecipients),
                    //CcRecipients = StringsToGraphRecipients(mailMessage.CcRecipients),
                    //BccRecipients = StringsToGraphRecipients(mailMessage.BccRecipients),
                    Subject = mailMessage.Subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = mailMessage.Body
                    },
                };

                var sendMailPostRequestBody = new SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = true
                };

                var sendEmailRequestInformation = GraphServiceClient
                    .Users[MailConfiguration.Email.Sender]
                    .SendMail
                    .ToPostRequestInformation(sendMailPostRequestBody);

                var httpRequestMessage = await GraphServiceClient
                    .RequestAdapter
                    .ConvertToNativeRequestAsync<HttpRequestMessage>(sendEmailRequestInformation);

                //var responseMessage = await _httpClient.SendAsync(httpRequestMessage);

                var httpClient = GraphClientFactory.Create();
                //var responseMessage = await httpClient.SendAsync(httpRequestMessage);

                //var httpClient = GraphClientFactory.Create(finalHandler: new HttpClientHandler());
                //var responseMessage = await httpClient.SendAsync(httpRequestMessage);

                //if (responseMessage.IsSuccessStatusCode)
                //{
                //    Trace.TraceInformation($"Mail was successfully sent.");
                //}
                //else
                //{
                //    throw new Exception($"Failed to send email. Status code: {responseMessage.StatusCode}");
                //}

                var results = string.Empty;

                try
                {
                    using (var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, 
                               HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        switch (httpResponseMessage.StatusCode)
                        {
                            case HttpStatusCode.OK:
                            case HttpStatusCode.Accepted:
                                results = await httpResponseMessage.Content.ReadAsStringAsync();
                                break;

                            case HttpStatusCode.Unauthorized:
                                throw new HttpRequestException($"Unauthorized request ({httpResponseMessage.StatusCode})");

                            default:
                                var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync();
                                throw new HttpRequestException($"Bad request (StatusCode: {httpResponseMessage.StatusCode}) ([{contentAsString}])");
                        }
                    }
                }
                catch (WebException webException)
                {
                    Trace.TraceError($"{GetType().Name}: Error when sending email! " + webException);
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
                    Trace.TraceError($"{GetType().Name}: Error when sending email! " + exception);
                    Trace.TraceError(ExceptionMessageRetriever.Execute(exception));

                    throw;
                }

                Trace.TraceInformation($"[{results}]");

                //await GraphServiceClient.Users[MailConfiguration.Email.Sender]
                //    .SendMail
                //    .PostAsync(sendMailPostRequestBody);
            }
            catch (ODataError odataError)
            {
                if (odataError.Error != null)
                {
                    Trace.TraceError($"{GetType().Name}: odataError.Error.Code = {odataError.Error.Code}");
                    Trace.TraceError($"{GetType().Name}: odataError.Error.Message = {odataError.Error.Message}");
                    Trace.TraceError($"{GetType().Name}: odataError.Error = {odataError.Error}");
                }

                throw;
            }
            catch (Exception exc)
            {
                var exceptionMessage = ExceptionMessageRetriever.Execute(exc);

                Trace.TraceError($"{GetType().Name}: Exception: {exceptionMessage}");
                Trace.TraceError($"{GetType().Name}: Exception: {exc}");

                throw;
            }
        }

        private List<Recipient> StringsToGraphRecipients(IEnumerable<string> emailAddresses)
        {
            return emailAddresses?.Select(StringToGraphRecipient).ToList();
        }

        private Recipient StringToGraphRecipient(string emailAddress)
        {
            return string.IsNullOrWhiteSpace(emailAddress)
                ? null
                : new Recipient { EmailAddress = new EmailAddress { Address = emailAddress } };
        }
    }
}
