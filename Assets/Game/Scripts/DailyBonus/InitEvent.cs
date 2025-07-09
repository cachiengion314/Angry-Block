using System;

public static class InitEvent
{
    public static void Init(DateTime currentTime, EventDataNguyen data, out DateTime startTime, out DateTime endTime)
    {
        startTime = default;
        endTime = default;

        if (data.DayOfWeek)
        {
            int startDaysToAdd = (int)data.startDayOfWeek - (int)currentTime.DayOfWeek;
            startTime = currentTime.AddDays(startDaysToAdd);

            int endDaysToAdd = (int)data.endDayOfWeek - (int)currentTime.DayOfWeek;
            endTime = currentTime.AddDays(endDaysToAdd);
            if ((int)data.startDayOfWeek >= (int)data.endDayOfWeek) endTime = endTime.AddDays(7);
            endTime = endTime.AddDays(7* data.lengthEvent);

            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, data.startHour, data.startMinute,0);
            endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, data.endHour, data.endMinute,0);
        }
        else if (data.Month)
        {
            int endYear = currentTime.Year + data.lengthEvent;
            if (data.startMonth >= data.endMonth) endYear++;
            startTime = new DateTime(currentTime.Year, data.startMonth, data.startDay, data.startHour, data.startMinute, 0);
            endTime = new DateTime(endYear, data.endMonth, data.endDay, data.endHour, data.endMinute, 0);
        }
        else if (data.Day)
        {
            int endYear = currentTime.Year;
            int endMonthTemp = currentTime.Month + data.lengthEvent;
            if (data.startDay >= data.endDay) endMonthTemp++;

            endYear += endMonthTemp / 12;
            int endMonth = endMonthTemp % 12;

            if (endMonth == 0)
            {
                endMonth = 12;
                endYear--;  // Giảm năm đi một khi chúng ta cộng thêm tháng 12.
            }

            startTime = new DateTime(currentTime.Year, currentTime.Month, data.startDay, data.startHour, data.startMinute, 0);
            endTime = new DateTime(endYear, endMonth, data.endDay, data.endHour, data.endMinute, 0);
        }
        else if (data.Hour)
        {
            int endYear = currentTime.Year;
            int endMonthTemp = currentTime.Month;
            int endDayTemp = currentTime.Day + data.lengthEvent;
            if (data.startHour >= data.endHour) endDayTemp++;
            int daysInMonth = DateTime.DaysInMonth(endYear, endMonthTemp);

            endMonthTemp += endDayTemp / daysInMonth;
            int endDay = endDayTemp % daysInMonth;

            if (endDay == 0)
            {
                endDay = daysInMonth;
                endMonthTemp--;
            }

            endYear += endMonthTemp / 12;
            int endMonth = endMonthTemp % 12;

            if (endMonth == 0)
            {
                endMonth = 12;
                endYear--;  // Giảm năm đi một khi chúng ta cộng thêm tháng 12.
            }

            startTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, data.startHour, data.startMinute, 0);
            endTime = new DateTime(endYear, endMonth, endDay, data.endHour, data.endMinute, 0);
        }
        else if (data.Minute)
        {
            int endYear = currentTime.Year;
            int endMonthTemp = currentTime.Month;
            int endDayTemp = currentTime.Day;
            int endHourTemp = currentTime.Hour + data.lengthEvent;
            if (data.startMinute >= data.endMinute) endHourTemp++;
            int daysInMonth = DateTime.DaysInMonth(endYear, endMonthTemp);

            endDayTemp += endHourTemp / 24;
            int endHour = endHourTemp % 24;

            endMonthTemp += endDayTemp / daysInMonth;
            int endDay = endDayTemp % daysInMonth;

            if (endDay == 0)
            {
                endDay = daysInMonth;
                endMonthTemp--;
            }

            endYear += endMonthTemp / 12;
            int endMonth = endMonthTemp % 12;

            if (endMonth == 0)
            {
                endMonth = 12;
                endYear--;  // Giảm năm đi một khi chúng ta cộng thêm tháng 12.
            }

            startTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, data.startMinute, 0);
            endTime = new DateTime(endYear, endMonth, endDay, endHour, data.endMinute, 0);
        }
    }

    public static bool CheckEvent(DateTime currentTime, DateTime startTime, DateTime endTime)
    {
        return currentTime >= startTime && currentTime <= endTime;
    }
}
