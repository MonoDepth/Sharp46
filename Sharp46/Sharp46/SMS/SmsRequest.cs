using Sharp46.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp46.SMS
{
    public class SmsRequest: IFormRequest
    {
        /// <summary>
        /// <para>The sender of the SMS as seen by the recipient.</para>
        /// <para>Either a text sender ID or a phone number in E.164 format if you want to be able to receive replies. </para>
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// The phone number of the recipient in E.164 format. 
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// The message to send.
        /// </summary>
        public string Message { get; set; } = string.Empty; 

        /// <summary>
        /// This webhook URL will receive a POST request every time the delivery status changes. 
        /// </summary>
        public string WhenDelivered { get; set; } = string.Empty; 

        /// <summary>
        /// Send the message as a Flash SMS. The message will be displayed immediately upon arrival and not stored. 
        /// </summary>
        public bool FlashSms { get; set; } = false;

        /// <summary>
        /// <para>Enable to avoid storing the message text in your history.</para>
        /// <para>The other parameters will still be stored.</para>
        /// </summary>
        public bool DontLog { get; set; } = false;

        /// <summary>
        /// <para>Enable when you want to verify your API request without actually sending an SMS to a mobile phone.</para>
        /// <para>No SMS message will be sent when this is enabled. </para>
        /// </summary>
        public bool DryRun { get; set; } = false;

        public FormUrlEncodedContent ToFormEncoded()
        {
           var content = new List<KeyValuePair<string, string>>() {
                new("to", To),
                new("from", From),
                new("message", Message),
            };

            if (!string.IsNullOrWhiteSpace(WhenDelivered))
            {
                content.Add(new("whendelivered", WhenDelivered));
            }

            if (FlashSms)
            {
                content.Add(new("flashsms", "yes"));
            }

            if (DontLog)
            {
                content.Add(new("dontlog", "message"));
            }

            if (DryRun)
            {
                content.Add(new("dryrun", "yes"));
            }

            return new(content);
        }
    }
}
