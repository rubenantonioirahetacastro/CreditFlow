namespace CreditFlow.Web.Core.Utils.Format;

public enum DocumentType
{
    Dui,
    Nit
}

public static class DocumentFormatter
{
    public static string Normalize(string? value, DocumentType type) =>
        DigitFormatter.OnlyDigits(value, type == DocumentType.Dui ? 9 : 14);

    public static string Format(string? value, DocumentType type)
    {
        var digits = Normalize(value, type);
        return type == DocumentType.Dui ? FormatDui(digits) : FormatNit(digits);
    }

    private static string FormatDui(string digits) => digits.Length <= 8
        ? digits
        : $"{digits[..8]}-{digits[8..]}";

    private static string FormatNit(string digits)
    {
        if (digits.Length <= 4)
            return digits;
        if (digits.Length <= 10)
            return $"{digits[..4]}-{digits[4..]}";
        if (digits.Length <= 13)
            return $"{digits[..4]}-{digits[4..10]}-{digits[10..]}";
        return $"{digits[..4]}-{digits[4..10]}-{digits[10..13]}-{digits[13..]}";
    }
}
