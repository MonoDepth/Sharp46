using Sharp46.Exceptions;
using Sharp46.Rest;

namespace Sharp46.SMS
{
    internal class SmsSender
    {
        /// <summary>
        /// Sends an SMS using the given <paramref name="smsRequest"/> and <paramref name="restClient"/> to the <paramref name="endpoint"/> endpoint
        /// </summary>
        /// <param name="smsRequest">The sms request</param>
        /// <param name="restClient">restClient to use for REST operations</param>
        /// <param name="endpoint">The endpoint</param>
        /// <returns>The SMS response</returns>
        /// <exception cref="SendMessageException">Thrown if the message fails to send with the inner exception detailing why</exception>
        public static async Task<SendSmsResponse> Send(SmsRequest smsRequest, IRestClient restClient, string endpoint)
        {
            try
            {
                var result = await restClient.PostAsync(endpoint, smsRequest);

                if (result.Response.IsSuccessStatusCode)
                {
                    var response = await result.Deserialize<SendSmsResponse>();

                    if (!response.SendSuccess)
                    {
                        throw new SendStatusIsFailedException($"Message sent but 46Elks responded with status {response.Status}");
                    }
                    return response;
                }
                else
                {
                    throw new SendMessageException($"SMS attempt to {smsRequest.To} failed with reason: HTTP Status {result.Response.StatusCode}", null);
                }
            }
            catch (Exception ex)
            {
                if (ex is SendMessageException)
                    throw;

                throw new SendMessageException($"SMS attempt to {smsRequest.To} failed with reason: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the SMS history for the account, automatically fetching the next page as needed
        /// </summary>
        /// <param name="restClient">REST client to use</param>
        /// <param name="endpoint">Endpoint which to call to fetch the list</param>
        /// <param name="start">(Optional) Retrieve SMS before this date</param>
        /// <param name="end">(Optional) Retrieve SMS after this date</param>        
        /// <param name="numberFilter">Optionally filter on recipient phonenumber</param>
        /// <returns>Async enumerable which can be used to iterate the messages</returns>
        /// <exception cref="FetchException">Thrown when we recieve a non OK status code</exception>
        public static async IAsyncEnumerable<Sms> GetHistory(IRestClient restClient, string endpoint, string? start, string? end, string? numberFilter)
        {
            string next = start ?? "";

            List<string> queryParams = new();

            if (!string.IsNullOrWhiteSpace(end))
            {
                queryParams.Add("end");
                queryParams.Add(end);
            }

            if (!string.IsNullOrWhiteSpace(numberFilter))
            {
                queryParams.Add("to");
                queryParams.Add(numberFilter);
            }

            do
            {
                var result = await restClient.GetAsync(endpoint, queryParams, "start", next);

                if (result.Response.IsSuccessStatusCode)
                {
                    var history = await result.Deserialize<SmsHistoryResponse>();

                    foreach (var sms in history.Data)
                    {
                        yield return sms;
                    }

                    next = history.Next ?? "";
                }
                else
                {
                    throw new FetchException($"Failed to fetch sms history with reason: HTTP Status {result.Response.StatusCode}", null);
                }
            } while (!string.IsNullOrWhiteSpace(next));

            yield break;
        }

        /// <summary>
        /// Gets a specific historic SMS by its ID
        /// </summary>
        /// <param name="restClient">The REST client to use</param>
        /// <param name="endpoint">The endpoint to call to fetch the SMS</param>
        /// <returns>The SMS if found, otherwise null</returns>
        public static async Task<Sms?> GetSms(IRestClient restClient, string endpoint)
        {
            var result = await restClient.GetAsync(endpoint);

            if (result.Response.IsSuccessStatusCode)
            {
                return await result.Deserialize<Sms>();
            }
            else
            {
                if (result.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                throw new FetchException($"Failed to fetch sms with reason: HTTP Status {result.Response.StatusCode}", null);
            }
        }
    }
}
