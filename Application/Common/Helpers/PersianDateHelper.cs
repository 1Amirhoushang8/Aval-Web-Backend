using System.Globalization;

namespace AvalWebBackend.Application.Common.Helpers;

public static class PersianDateHelper
{
    public static bool TryParsePersianDate(string persianDate, out DateTime result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(persianDate))
            return false;

        var parts = persianDate.Split('-');
        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out int year) ||
            !int.TryParse(parts[1], out int month) ||
            !int.TryParse(parts[2], out int day))
            return false;

        try
        {
            result = new PersianCalendar().ToDateTime(year, month, day, 0, 0, 0, 0);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static DateTime GetStartOfPersianWeek(DateTime date)
    {
        var pc = new PersianCalendar();
        int dayOfWeek = (int)pc.GetDayOfWeek(date); // 0 = Saturday
        return date.AddDays(-dayOfWeek).Date;
    }

    public static int GetPersianDayIndex(DateTime date)
    {
        var pc = new PersianCalendar();
        return (int)pc.GetDayOfWeek(date); // 0 = Saturday
    }
}