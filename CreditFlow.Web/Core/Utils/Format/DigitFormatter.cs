namespace CreditFlow.Web.Core.Utils.Format;

internal static class DigitFormatter
{
    public static string OnlyDigits(string? value, int maxLength) =>
        new((value ?? string.Empty)
            .Where(char.IsDigit)
            .Take(maxLength)
            .ToArray());
}
