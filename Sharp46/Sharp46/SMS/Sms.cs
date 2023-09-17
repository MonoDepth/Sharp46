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

        public string Id { get; set; } = string.Empty;
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
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public SmsStatus Status { get; set; } = SmsStatus.Unkown;
        public DateTime? Delivered { get; set; }
        public int Cost { get; set; }
    }
}
