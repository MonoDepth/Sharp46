using System.Text.Json.Serialization;
using static Sharp46.SMS.Sms;

namespace Sharp46.SMS
{
    public class SendSmsResponse
    {
        private int _cost;
        private string _rawStatus = string.Empty;

        /// <summary>
        /// <para>Current delivery status of the message.</para>
        /// <para>Possible values are "created", "sent", "failed" and "delivered". </para>
        /// </summary>
        public Sms.SmsStatus Status { get; set; } = Sms.SmsStatus.Unkown;

        /// <summary>
        /// Unique identifier for this SMS. 
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// <para>The sender of the SMS as seen by the recipient.</para>
        /// <para>String may start with a letter and contain numbers - Max 11 characters including A-Z, a-z, 0-9. </para>
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// The phone number of the recipient in E.164 format.
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// The message text.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Time in UTC when the SMS was created. 
        /// </summary>
        public DateTime Created { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Time in UTC if the SMS has been successfully delivered. 
        /// </summary>
        public DateTime? Delivered { get; set; } = DateTime.MinValue;

        /// <summary>
        /// <para>Cost of sending the SMS. Specified in 10000s of the currency of your account.</para>
        /// <para>For an account with currency SEK a cost of 3500 means that the price for sending this SMS was 0.35 SEK.</para>
        /// </summary>
        public int Cost { get { return Estimated_cost ?? _cost; } set { _cost = value; } }


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
        /// The direction of the SMS. Set to "outgoing" for sent SMS. Enum is get only, please use RawDirection to modify the value.
        /// </summary>
        [JsonIgnore]
        public SmsDirection Direction { get; private set; } = SmsDirection.Unkown;

        /// <summary>
        /// Set to "message" if dontlog was enabled. 
        /// </summary>
        public string DontLog { get; set; } = string.Empty;

        /// <summary>
        /// <para>Replaces cost in the response if dryrun was enabled.</para>
        /// <para>Use <see cref="Cost"/> instead as it defaults to this field if it's set</para>
        /// </summary>        
        public int? Estimated_cost { get; set; }

        /// <summary>
        /// Number of parts the SMS was divided into. 
        /// </summary>
        public int Parts { get; set; }

        public bool SendSuccess
        {
            get
            {
                return Status != Sms.SmsStatus.Failed && Status != Sms.SmsStatus.Unkown;
            }
        }
    }
}
