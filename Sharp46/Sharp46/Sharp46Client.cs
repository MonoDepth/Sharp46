using Sharp46.PhoneNumber;
using Sharp46.Rest;
using Sharp46.SMS;

namespace Sharp46.Client
{
    public class Sharp46Client 
    {
        private readonly IRestClient _restClient;
        private const string _baseUrl = "https://api.46elks.com/";

        /// <summary>
        /// <para>Factory method to create a new REST client</para>
        /// <para>Can be set to a custom function that returns a <see cref="IRestClient"/></para>
        /// </summary>
        //public Func<string, string, IRestClient> NewClientAction { get; set; }

        /// <summary>
        /// The 46Elks API version to use
        /// </summary>
        public string ApiVersion = "a1";

        public Sharp46Client(string apiUser, string apiPassword)
        {
            _restClient = new RestClient(apiUser, apiPassword);
        }

        public Sharp46Client(IRestClient restClient)
        {
            _restClient = restClient;
            //NewClientAction = (usr, pw) => new RestClient(usr, pw);
        }

        #region SMS
        /// <summary>
        /// Sends and SMS using the given <paramref name="smsRequest"/>
        /// </summary>
        /// <param name="smsRequest">The object representing the sms to be sent</param>
        /// <returns></returns>
        public async Task<SendSmsResponse> SendSms(SmsRequest smsRequest) => await SmsSender.Send(smsRequest, _restClient, BuildUrl("sms"));

        /// <summary>
        /// Gets the SMS history for the account, automatically fetching the next page as needed
        /// </summary>
        /// <param name="start">(Optional) Retrieve SMS before this date</param>
        /// <param name="end">(Optional) Retrieve SMS after this date</param>        
        /// <param name="numberFilter">Optionally filter on recipient phonenumber</param>
        /// <returns></returns>
        public IAsyncEnumerable<Sms> GetSmsHistory(string? start, string? end, string? numberFilter) => SmsSender.GetHistory(_restClient, BuildUrl("sms"), start, end, numberFilter);

        /// <summary>
        /// Gets a specific historic SMS by its ID
        /// </summary>
        /// <param name="id">The ID of the SMS</param>
        /// <returns></returns>
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