# Sharp46
Integration with the 46Elks API

Can be used to send messages through the API.

Supports using custom HTTP clients for REST operations if the default is not enough

# Usage
Send an SMS
```CSharp
var client = new Sharp46Client(YOUR_API_USER, YOUR_API_PASSWORD);

var smsReq = new SmsRequest()
{
    From = "Sharp46",
    To = "+46700000000",
    Message = "TestMessage",
    DryRun = true
};

return await client.SendSms(smsReq);
```

Get outgoing sms list
```CSharp
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
```

More examples in the [examples](Sharp46/Sharp46_Examples) folder

# Roadmap
* MMS Support
* VOIP support
* ASP.NET extensions to recieve SMS/MMS/Voice
