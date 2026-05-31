using System;
using System.Globalization;

public static class DateTimeHelper
{
    public static DateTimeOffset ParseDateTimeOffset(string dateTimeString)
    {
        if (
            DateTimeOffset.TryParse(
                dateTimeString,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var result
            )
        )
        {
            return result;
        }

        throw new FormatException($"Invalid date time format: {dateTimeString}");
    }

    public static string ToIsoString(DateTimeOffset dateTime)
    {
        return dateTime.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
    }
}
