using NSubstitute;
using Sharp46.Exceptions;
using Sharp46.Rest;
using Sharp46.SMS;

namespace Sharp46_UnitTests
{
    public class SmsSendTests
    {
        [Fact]
        public async Task Sms_ShouldBeSent()
        {
            var toNumber = Environment.GetEnvironmentVariable("SHARP46_TEST_TO") ?? "";
            var apiUser = Environment.GetEnvironmentVariable("SHARP46_TEST_API_USER") ?? "";
            var apiPassword = Environment.GetEnvironmentVariable("SHARP46_TEST_API_PASSWORD") ?? "";

            // Skip test if no environment variables are set, intended to be run in CI
            if (string.IsNullOrEmpty(toNumber) || string.IsNullOrEmpty(apiUser) || string.IsNullOrEmpty(apiPassword))
            {
                Assert.True(true);
                return;
            }

            var client = new Sharp46Client(apiUser, apiPassword);

            var smsReq = new SmsRequest()
            {
                From = "UnitTest",
                To = "+46700000000",
                Message = "TestMessage",
                DryRun = true
            };

            var response = await client.SendSms(smsReq);

            Assert.True(response.SendSuccess);
        }

        [Fact]
        public async Task Mocked_BadCredentials_FailsWithException()
        {
            //var badRestClient = new TestRestClient("baduser", "testpassword");

            var badRestClient = Substitute.For<IRestClient>();
            badRestClient.PostAsync(Arg.Any<string>(), Arg.Any<SmsRequest>()).Returns(new RequestResponse()
            {
                Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
            });

            var smsReq = new SmsRequest()
            {
                From = "UnitTest",
                To = "+46000000000",
                Message = "TestMessage",
                DryRun = false
            };

            var client = new Sharp46Client(badRestClient);

            await Assert.ThrowsAsync<SendMessageException>(() => client.SendSms(smsReq));
        }

        [Fact]
        public async Task Mocked_ValidSms_IsSent()
        {

            var restClient = Substitute.For<IRestClient>();
            restClient.PostAsync(Arg.Any<string>(), Arg.Any<SmsRequest>()).Returns(new RequestResponse()
            {
                Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(@"{""id"":""s70df59406a1b4643b96f3f91e0bfb7b0"",""status"":""sent"",""from"":""YourCompany"",""to"":""+46700000000"",""parts"":1,""message"":""This is the message sent to the phone."",""created"":""2018-07-11T13:37:42.314100"",""cost"":3500,""direction"":""outgoing""}")
                }
            });

            var smsReq = new SmsRequest()
            {
                From = "UnitTest",
                To = "+46000000000",
                Message = "Testmessage",
                DryRun = false
            };

            var client = new Sharp46Client(restClient);

            var response = await client.SendSms(smsReq);

            Assert.True(response.SendSuccess);
        }

        [Fact]
        public async Task Mocked_BadSms_FailsToSend()
        {
            // The TestRestClient delivers Status = "failed" if the message contains "FailMessage"

            var restClient = Substitute.For<IRestClient>();
            restClient.PostAsync(Arg.Any<string>(), Arg.Any<SmsRequest>()).Returns(new RequestResponse()
            {
                Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(@"{""id"":""s70df59406a1b4643b96f3f91e0bfb7b0"",""status"":""failed"",""from"":""YourCompany"",""to"":""+46700000000"",""parts"":1,""message"":""This is the message sent to the phone."",""created"":""2018-07-11T13:37:42.314100"",""cost"":3500,""direction"":""outgoing""}")
                }
            });

            var smsReq = new SmsRequest()
            {
                From = "UnitTest",
                To = "+46000000000",
                Message = "FailMessage",
                DryRun = false
            };

            var client = new Sharp46Client(restClient);

            try
            {
                await client.SendSms(smsReq);
                Assert.Fail("SendSms did not fail as expected");
            }
            catch (SendMessageException ex)
            {
                Assert.IsType<SendStatusIsFailedException>(ex.InnerException);
            }
        }

        [Fact]
        public async Task MockedHistory_ShouldReturnOldMessages()
        {
            var restClient = Substitute.For<IRestClient>();

            restClient.GetAsync(Arg.Any<string>(), Arg.Any<object[]>()).Returns(x =>
            {
                if ((x.Args()[1] as object[])!.Contains("2017-02-21T14:15:30.427000"))
                {
                    return new RequestResponse()
                    {
                        Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(@"{""data"":[{""id"":""lastMessage"",""created"":""2017-03-14T09:52:07.302000"",""direction"":""outgoing"",""from"":""MyService"",""to"":""+46704508449"",""message"":""Hello hello"",""status"":""delivered"",""delivered"":""2017-03-14T09:52:10Z"",""cost"":3500}]}")
                        }
                    };
                }
                else
                {
                    return new RequestResponse()
                    {
                        Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(@"{""data"":[{""id"":""s17a6dafb12d6b1cabc053d57dac2b9d8"",""created"":""2017-03-14T09:52:07.302000"",""direction"":""outgoing"",""from"":""MyService"",""to"":""+46704508449"",""message"":""Hello hello"",""status"":""delivered"",""delivered"":""2017-03-14T09:52:10Z"",""cost"":3500},{""id"":""s299b2d2a467945f59e1c9ea431eed9d8"",""created"":""2017-03-14T08:44:34.608000"",""direction"":""outgoing-reply"",""from"":""+46766861069"",""to"":""+46704508449"",""message"":""We are open until 19:00 today. Welcome!"",""status"":""delivered"",""delivered"":""2017-03-14T08:44:36Z"",""cost"":3500},{""id"":""s292d2a459e967945fb1c9ea431eed9d8"",""created"":""2017-03-14T08:44:34.135000"",""direction"":""incoming"",""from"":""+46704508449"",""to"":""+46766861069"",""message"":""Hours?"",""cost"":3500}],""next"":""2017-02-21T14:15:30.427000""}")
                        }
                    };
                }

            });

            var client = new Sharp46Client(restClient);
            var historyAsync = client.GetSmsHistory(null, null, null);

            List<Sms> history = new();

            await foreach (var sms in historyAsync)
            {
                history.Add(sms);
            }

            Assert.True(history.Count == 4);
            Assert.Equal("lastMessage", history.Last().Id);
        }

        [Theory]
        [MemberData(nameof(SmsList))]
        public async Task MockedGetSms_ShouldFetchSms(string smsId, string json)
        {
            var restClient = Substitute.For<IRestClient>();

            restClient.GetAsync(Arg.Any<string>(), Arg.Any<object[]>()).Returns(new RequestResponse()
            {
                Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                }
            });

            var client = new Sharp46Client(restClient);
            var sms = await client.GetSms(smsId);
            Assert.NotNull(sms);
            Assert.Equal(smsId, sms.Id);
        }


        public static IEnumerable<object[]> SmsList()
        {
            yield return new object[] { "s3633fa8e62f823e52fbc67ebf6925ab5", @"{""id"":""s3633fa8e62f823e52fbc67ebf6925ab5"",""direction"":""incoming"",""from"":""SMSElk"",""created"":""2016-08-18T09:55:31.116000"",""to"":""+46700000000"",""message"":""I'm not a moose.""}" };
            yield return new object[] { "a5633fa8e62f823e52fbc67ebf6925ab5", @"{""id"":""a5633fa8e62f823e52fbc67ebf6925ab5"",""direction"":""outgoing"",""from"":""SMSElk"",""created"":""2016-08-18T09:55:31.116000"",""to"":""+46700000000"",""message"":""I'm not a moose.""}" };
            yield return new object[] { "b8633fa8e62f823e52fbc67ebf6925ab5", @"{""id"":""b8633fa8e62f823e52fbc67ebf6925ab5"",""direction"":""outgoing-reply"",""from"":""SMSElk"",""created"":""2016-08-18T09:55:31.116000"",""to"":""+46700000000"",""message"":""I'm not a moose.""}" };
        }
    }
}
