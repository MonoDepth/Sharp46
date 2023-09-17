using System.Text.Json.Serialization;

namespace Sharp46.SMS
{
    public class Sms
    {
        private string _rawStatus = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SmsStatus
        {
            Unkown,
            Created,
            Sent,
            Failed,
            Delivered
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SmsDirection
        {
            Unkown,
            Outgoing,
            Incoming,
            OutgoingReply
        }

        /// <summary>
        /// ID of the SMS.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The time in UTC when the SMS was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// <para>The raw non converted value from the API</para>
        /// <para>outgoing, incoming, outgoing-reply</para>
        /// </summary>
        [JsonPropertyName("direction")]
        public string RawDirection
        {
            get
            {
                return _rawStatus;
            }
            set
            {
                _rawStatus = value.ToLowerInvariant();
                Direction = _rawStatus switch
                {
                    "outgoing" => SmsDirection.Outgoing,
                    "incoming" => SmsDirection.Incoming,
                    "outgoing-reply" => SmsDirection.OutgoingReply,
                    _ => SmsDirection.Unkown,
                };
            }
        }

        /// <summary>
        /// The SMS direction. Enum is get only, please use RawDirection to modify the value.
        /// </summary>
        [JsonIgnore]
        public SmsDirection Direction { get; private set; } = SmsDirection.Unkown;

        /// <summary>
        /// The sender.
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// The phone number of the recipient, in E.164 format.
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        ///  The content of the SMS, same as the message parameter. Not included in the response if dontlog=message is set.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// created (recieved by our servers), sent (sent from us to the carrier), delivered (confirmed delivered to the recipient) or failed (could not be delivered).
        /// </summary>
        public SmsStatus Status { get; set; } = SmsStatus.Unkown;

        /// <summary>
        /// The time in UTC when the SMS was delivered.
        /// </summary>
        public DateTime? Delivered { get; set; }

        /// <summary>
        /// <para>The cost of sending the SMS. Specified in 10000s of the currency of the account (SEK or EUR).</para>
        /// <para>For example, for an account with currency SEK, a cost of 3500 means that it cost 0.35SEK. Learn more about the details of pricing.</para>
        /// </summary>
        public int Cost { get; set; }
    }
}
