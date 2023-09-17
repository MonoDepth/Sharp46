using CustomRestClient;
using Sharp46.Client;
using Sharp46.PhoneNumber;
using Sharp46.SMS;

#pragma warning disable CS8321 // Local function is declared but never used

static async Task<SendSmsResponse> SendSms()
{
    var restClient = new MyCustomRestClient();
    var client = new Sharp46Client(restClient);

    var smsReq = new SmsRequest()
    {
        From = "Sharp46",
        To = "+46700000000",
        Message = "TestMessage",
        DryRun = true
    };

    return await client.SendSms(smsReq);
}


static async Task<List<Sms>> GetOutgoingSmsHistory()
{
    var restClient = new MyCustomRestClient();
    var client = new Sharp46Client(restClient);

    var history = client.GetSmsHistory("2023-09-17T23:59:59.00", null, null);

    var outgoingList = new List<Sms>();

    await foreach (var sms in history)
    {
        if (sms.Direction == Sms.SmsDirection.Outgoing)
        {
            outgoingList.Add(sms);
        }
    }

    return outgoingList;
}

static async Task<Sms?> GetSingleMessage()
{
    var restClient = new MyCustomRestClient();
    var client = new Sharp46Client(restClient);

    return await client.GetSms("smsid");
}

static void FormatAndVerifyNumber()
{
    var number = "0700000000";

    var formattedNumber = Sharp46Client.FormatNumber(number, CountryCodes.SE);

    var isValid = Sharp46Client.VerifyNumberFormat(formattedNumber);

    Console.WriteLine($"Number {formattedNumber} is valid: {isValid}");
}

#pragma warning restore CS8321 // Local function is declared but never used