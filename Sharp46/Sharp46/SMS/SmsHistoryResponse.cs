namespace Sharp46.SMS
{
    public class SmsHistoryResponse
    {
        /// <summary>
        /// List of SMS.
        /// </summary>
        public List<Sms> Data { get; set; } = new();

        /// <summary>
        /// Timestamp to the next page if more SMS are available.
        /// </summary>
        public string? Next { get; set; }
    }
}
