using System;
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

    
    public static int GetPersianDayIndex(DateTime date)
    {
        
        return date.DayOfWeek switch
        {
            DayOfWeek.Saturday => 0,
            DayOfWeek.Sunday => 1,
            DayOfWeek.Monday => 2,
            DayOfWeek.Tuesday => 3,
            DayOfWeek.Wednesday => 4,
            DayOfWeek.Thursday => 5,
            DayOfWeek.Friday => 6,
            _ => 0
        };
    }

    
    public static DateTime GetStartOfPersianWeek(DateTime date)
    {
        int dayIndex = GetPersianDayIndex(date);
        return date.AddDays(-dayIndex).Date;
    }
}