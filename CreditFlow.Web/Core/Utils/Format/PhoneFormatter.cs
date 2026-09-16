namespace CreditFlow.Web.Core.Utils.Format;

public static class PhoneFormatter
{
    public static string Normalize(string? value) => DigitFormatter.OnlyDigits(value, 8);

    public static string Format(string? value)
    {
        var digits = Normalize(value);
        return digits.Length <= 4 ? digits : $"{digits[..4]}-{digits[4..]}";
    }
}
