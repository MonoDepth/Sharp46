namespace Sharp46.SMS
{
    public class SmsHistoryResponse
    {
        public List<Sms> Data { get; set; } = new();
        public string? Next { get; set; }
    }
}
