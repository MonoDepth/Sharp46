using Sharp46.Client;
using Sharp46.PhoneNumber;
using Sharp46.SMS;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS8321 // Local function is declared but never used


const string YOUR_API_USER = "aaa";
const string YOUR_API_PASSWORD = "111";

static async Task<SendSmsResponse> SendSms()
{
    var client = new Sharp46Client(YOUR_API_USER, YOUR_API_PASSWORD);

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
    var client = new Sharp46Client(YOUR_API_USER, YOUR_API_PASSWORD);

    var outgoingList = new List<Sms>();

    await foreach (var sms in client.GetSmsHistory("2023-09-17T23:59:59.00", null, null))
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
    var client = new Sharp46Client(YOUR_API_USER, YOUR_API_PASSWORD);

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
#pragma warning restore IDE0059 // Unnecessary assignment of a value