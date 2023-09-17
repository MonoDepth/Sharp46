using Sharp46.Exceptions;
using System.Text.RegularExpressions;

namespace Sharp46.PhoneNumber
{
    internal class NumberValidator
    {
        private static readonly Regex _strictValidationRegex = new(@"\+\d{2,15}", RegexOptions.Compiled);
        private static readonly Regex _laxValidationRegex = new(@"\+([0-9 ]+[-()]*){3}", RegexOptions.Compiled);
        //private static readonly Regex _laxValidationRegex = new(@"\+[0-9 ]+", RegexOptions.Compiled);
        private static readonly Regex _formatRegex = new(@"[^\d]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// <para>Verifies that the given phone number follows the E.164 number format.</para>
        /// <para>See <c>https://46elks.se/kb/e164</c> for more information</para>
        /// </summary>
        /// <param name="number">The phone number to test</param>
        /// <param name="strict">
        /// <para>
        /// Phone number must contain no spaces, and have a valid country code.
        /// Settings string to <c>False</c> checks the general format (with spaces, hyphens and parentheses allowed)
        /// </para>        
        /// </param>
        /// <returns><c>True</c> if all criterias are met, otherwise <c>False</c></returns>
        public static bool VerifyNumberFormat(string number, bool strict)
        {
            if (strict)
            {
                // TODO: Could probably optimize this one
                return _strictValidationRegex.IsMatch(number) && CountryCodes.All.Any(cc => number.StartsWith(cc));
            }
            else
            {
                return _laxValidationRegex.IsMatch(number);
            }
        }


        /// <summary>
        /// <para>Formats the given <paramref name="number"/> to the E.164 format and appends the <paramref name="countryCode"/></para>
        /// <para>The <paramref name="countryCode"/> will only be appended if the input is missing one</para>
        /// </summary>
        /// <param name="number">The number to format</param>
        /// <param name="countryCode">The country code to append if the number doesn't already contain one</param>
        /// <returns>The formatted number</returns>
        /// <exception cref="InvalidNumberException">If the number cannot be formatted</exception>
        public static string FormatNumber(string number, string countryCode)
        {
            string formattedNumber = _formatRegex.Replace(number, "");

            if (formattedNumber.StartsWith("0"))
            {
                formattedNumber = formattedNumber[1..];
            }

            if (string.IsNullOrEmpty(formattedNumber))
            {
                throw new InvalidNumberException($"{number} cannot be formatted");
            }

            if (number.Trim().StartsWith("+"))
            {
                formattedNumber = $"+{formattedNumber}";
            }
            else
            {
                formattedNumber = $"{countryCode}{formattedNumber}";
            }

            return formattedNumber;
        }

        /// <summary>
        /// <para>Formats the given <paramref name="number"/> to the E.164 format and appends the <paramref name="countryCode"/></para>
        /// See also <seealso cref="FormatNumber(string, string)"/>
        /// </summary>
        /// <param name="number">The number to format</param>
        /// <param name="countryCode">The country code to append if the number doesn't already contain one</param>
        /// <param name="formattedNumber">The formatted number, if successful, otherwise an empty string</param>
        /// <returns>true if successful, otherwise false</returns>
        public static bool TryFormatNumber(string number, string countryCode, out string formattedNumber)
        {
            try
            {
                formattedNumber = FormatNumber(number, countryCode);
                return true;
            }
            catch (InvalidNumberException)
            {
                formattedNumber = "";
                return false;
            }
        }

        /// <summary>
        /// Extracts the country code from a given valid E-164 number
        /// </summary>
        /// <param name="number">The number to extract the country code from</param>
        /// <returns>The country code if present, otherwise <see cref="null"/></returns>
        public static string? ExtractCountryCode(string number)
        {
            // TODO: Could probably optimize this one
            var countryCode = CountryCodes.All.OrderBy(x => x.Length).FirstOrDefault(cc => number.StartsWith(cc));
            return countryCode;
        }
    }
}
