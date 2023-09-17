using Sharp46.PhoneNumber;
using Sharp46.Exceptions;
using Sharp46.Rest;
using Sharp46.SMS;

namespace Sharp46.Client
{
    public class Sharp46Client
    {
        private readonly IRestClient _restClient;
        private const string _baseUrl = "https://api.46elks.com/";

        /// <summary>
        /// The 46Elks API version to use
        /// </summary>
        public string ApiVersion = "a1";

        /// <summary>
        /// Creates a new <see cref="Sharp46Client"/> with the default <see cref="RestClient"/> using the given <paramref name="apiUser"/> and <paramref name="apiPassword"/>
        /// </summary>
        /// <param name="apiUser">The 46Elks user</param>
        /// <param name="apiPassword">The 46Elks password</param>
        public Sharp46Client(string apiUser, string apiPassword)
        {
            _restClient = new RestClient(apiUser, apiPassword);
        }

        /// <summary>
        /// Creates a new <see cref="Sharp46Client"/> with a custom <see cref="IRestClient"/>
        /// </summary>
        /// <param name="restClient">The custom <see cref="IRestClient"/> to be used for REST operations</param>
        public Sharp46Client(IRestClient restClient)
        {
            _restClient = restClient;            
        }

        #region SMS
        /// <summary>
        /// Sends and SMS using the given <paramref name="smsRequest"/>
        /// </summary>
        /// <param name="smsRequest">The object representing the sms to be sent</param>
        /// <returns>The SMS response</returns>
        /// <exception cref="SendMessageException">Thrown if the message fails to send with the inner exception detailing why</exception>
        public async Task<SendSmsResponse> SendSms(SmsRequest smsRequest) => await SmsSender.Send(smsRequest, _restClient, BuildUrl("sms"));

        /// <summary>
        /// Gets the SMS history for the account, automatically fetching the next page as needed
        /// </summary>
        /// <param name="start">(Optional) Retrieve SMS before this date</param>
        /// <param name="end">(Optional) Retrieve SMS after this date</param>        
        /// <param name="numberFilter">Optionally filter on recipient phonenumber</param>
        /// <returns>Async enumerable which can be used to iterate the messages</returns>
        /// <exception cref="FetchException">Thrown when we recieve a non OK status code</exception>
        public IAsyncEnumerable<Sms> GetSmsHistory(string? start, string? end, string? numberFilter) => SmsSender.GetHistory(_restClient, BuildUrl("sms"), start, end, numberFilter);

        /// <summary>
        /// Gets a specific historic SMS by its ID
        /// </summary>
        /// <param name="id">The ID of the SMS</param>
        /// <returns>The SMS if found, otherwise null</returns>
        public async Task<Sms?> GetSms(string id) => await SmsSender.GetSms(_restClient, BuildUrl($"sms/{id}"));
        #endregion

        #region NumberValidaton
        /// <inheritdoc cref="NumberValidator.VerifyNumberFormat(string, bool)"/>
        public static bool VerifyNumberFormat(string number, bool strict = true) => NumberValidator.VerifyNumberFormat(number, strict);

        /// <inheritdoc cref="NumberValidator.FormatNumber(string, string)"/>
        public static string FormatNumber(string number, string countryCode) => NumberValidator.FormatNumber(number, countryCode);

        /// <inheritdoc cref="NumberValidator.TryFormatNumber(string, string, out string)(string, string)"/>
        public static bool TryFormatNumber(string number, string countryCode, out string formattedNumber) => NumberValidator.TryFormatNumber(number, countryCode, out formattedNumber);

        /// <inheritdoc cref="NumberValidator.ExtractCountryCode(string)"/>
        public static string? ExtractCountryCode(string number) => NumberValidator.ExtractCountryCode(number);
        #endregion

        private string BuildUrl(string endpoint)
        {
            return $"{_baseUrl}{ApiVersion}/{endpoint.TrimStart('/')}";

        }
    }
}