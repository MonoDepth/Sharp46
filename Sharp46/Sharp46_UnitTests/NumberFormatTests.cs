using Sharp46.Exceptions;
using Sharp46.PhoneNumber;

namespace Sharp46_UnitTests
{
    public class NumberFormatTests
    {
        [Theory]
        [MemberData(nameof(StrictValidNumbers))]
        public void ValidPhoneNumber_ShouldPassStrict(string number)
        {
            Assert.True(Sharp46Client.VerifyNumberFormat(number));
        }

        [Fact]
        public void InvalidPhoneNumber_ShouldFailStrict()
        {
            const string phoneNumber = "abc123";

            Assert.False(Sharp46Client.VerifyNumberFormat(phoneNumber));
        }

        [Fact]
        public void InvalidCountryCode_ShouldFailStrict()
        {
            const string phoneNumber = "+00000000000";

            Assert.False(Sharp46Client.VerifyNumberFormat(phoneNumber));
        }

        [Theory]
        [MemberData(nameof(LaxValidNumbers))]
        public void SemiValidPhoneNumber_ShouldPassLax(string number)
        {
            Assert.True(Sharp46Client.VerifyNumberFormat(number, false));
        }

        [Fact]
        public void InvalidCountryCode_ShouldPassLax()
        {
            const string phoneNumber = "+12000000000";

            Assert.True(Sharp46Client.VerifyNumberFormat(phoneNumber));
        }

        [Fact]
        public void InvalidPhoneNumber_ShouldFailLax()
        {
            const string phoneNumber = "abc123";

            Assert.False(Sharp46Client.VerifyNumberFormat(phoneNumber));
        }

        [Fact]
        public void FormattedNumber_ShouldNotChange()
        {
            const string phoneNumber = "+46000000000";

            Assert.Equal(phoneNumber, Sharp46Client.FormatNumber(phoneNumber, CountryCodes.SE));
        }

        [Theory]
        [MemberData(nameof(UnformattedNumbers))]
        public void ValidNumber_ShouldBeFormatted(string number, string countryCode, string expectedFormat)
        {
            Assert.Equal(expectedFormat, Sharp46Client.FormatNumber(number, countryCode));
        }

        [Fact]
        public void InvalidNumber_ShouldFailFormatting()
        {
            const string phoneNumber = "abc()[]}";

            Assert.Throws<InvalidNumberException>(() => Sharp46Client.FormatNumber(phoneNumber, CountryCodes.SE));
        }

        [Theory]
        [MemberData(nameof(CountryCodeNumbers))]
        public void ExtractValidCountryCode_GivesCountryCode(string number, string countryCode)
        {
            Assert.Equal(countryCode, Sharp46Client.ExtractCountryCode(number));
        }

        [Fact]
        public void ExtractInvalidCountryCode_GivesNull()
        {
            const string phoneNumber = "+00000000000";

            Assert.Null(Sharp46Client.ExtractCountryCode(phoneNumber));
        }


        public static IEnumerable<object[]> StrictValidNumbers()
        {
            yield return new object[] { "+46000000000" };
            yield return new object[] { "+47000000000" };
        }

        public static IEnumerable<object[]> LaxValidNumbers()
        {
            yield return new object[] { "+46 000 000 00" };
            yield return new object[] { "+49000000000" };
            yield return new object[] { "+4700 000 00 00" };
            yield return new object[] { "+85200-000-00-00" };

            //Also validate strict format numbers
            foreach (var number in StrictValidNumbers())
            {
                yield return number;
            }
        }

        public static IEnumerable<object[]> UnformattedNumbers()
        {
            yield return new object[] { "+4600 000 00 00", CountryCodes.SE, "+46000000000" };
            yield return new object[] { "0000000000", CountryCodes.DE, "+49000000000" };
            yield return new object[] { "000 000 00 00", CountryCodes.NO, "+47000000000" };
            yield return new object[] { "000-000-00-00", CountryCodes.HK, "+852000000000" };
        }

        public static IEnumerable<object[]> CountryCodeNumbers()
        {
            yield return new object[] { "+46000000000", CountryCodes.SE };
            yield return new object[] { "+49000000000", CountryCodes.DE };
            yield return new object[] { "+47000000000", CountryCodes.NO };
            yield return new object[] { "+852000000000", CountryCodes.HK };
        }
    }
}