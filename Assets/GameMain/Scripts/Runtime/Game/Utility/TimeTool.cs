using System;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public static class TimeTool
    {
        public static long GetNowTimeStampMillisecond()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
        
        public static long GetNowTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 获得目标时间的天数(相对于格林威治时间起点)
        /// </summary>
        public static double GetDay(DateTime date)
        {
            TimeSpan timeSpan = date - new DateTime(1970, 1, 1, 0, 0, 0);
            return timeSpan.TotalDays;
        }

        public static long NowAddOffset(TimeSpan offset)
        {
            long timeStamp = GetNowTimeStamp();

            long offset2 = (long)offset.TotalSeconds;

            return timeStamp + offset2;
        }

        public static long GetTimeStampYearMonthDay(long timeStamp)
        {
            var date = ToDateTime(timeStamp);

            return timeStamp - (date.Hour * 60 * 60 + date.Minute * 60 + date.Second);
        }

        public static TimeSpan GetSubtract(long tMax, long tMin)
        {
            if (tMax <= tMin)
            {
                return TimeSpan.Zero;
            }

            TimeSpan time1 = TimeSpan.FromSeconds(tMax);

            TimeSpan time2 = TimeSpan.FromSeconds(tMin);

            return time1 - time2;
        }

        public static string FormatTimeStamp(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        public static string FormatTimeStamp2(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"mm\:ss");
        }

        public static string FormatTimeStamp3(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm");
        }
        
        public static string FormatTimeStamp4(TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 60)
            {
                return string.Format("{0:D2}秒", timeSpan.Seconds);
            }
            else if (timeSpan.TotalSeconds < 60 * 60)
            {
                return string.Format("{0:D2}分:{1:D2}秒", timeSpan.Minutes, timeSpan.Seconds);
            }
            else if (timeSpan.TotalSeconds < 60 * 60 * 24)
            {
                return string.Format("{0:D2}小时:{1:D2}分", timeSpan.Hours ,timeSpan.Minutes);
            }
            else
            {
                return string.Format("{0:D2}天", timeSpan.Days);
            }
        }

        public static string FormatTimeStamp5(TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 60)
            {
                return string.Format("{0}秒", timeSpan.Seconds);
            }
            else if (timeSpan.TotalSeconds < 60 * 60)
            {
                return string.Format("{0}分", timeSpan.Minutes);
            }
            else if (timeSpan.TotalSeconds < 60 * 60 * 24)
            {
                if (timeSpan.Minutes == 0)
                {
                    return string.Format("{0}小时", timeSpan.Hours);
                }

                return string.Format("{0}小时:{1}分", timeSpan.Hours ,timeSpan.Minutes);
            }
            else
            {
                return string.Format("{0}天", timeSpan.Days);
            }
        }
        
        public static string GetSubtractToString(long tMax, long tMin)
        {
            return FormatTimeStamp(GetSubtract(tMax, tMin));
        }

        public static DateTime ToDateTime(long timeStamp)
        {
            var date = new DateTime(1970, 1, 1).AddSeconds(timeStamp);
            return date;
        }
        
        public static long ToLong(DateTime data)
        {
            TimeSpan ts = data - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static bool IsSameDay(long timeStampMin, long timeStampMax)
        {
            return IsSameDay(timeStampMin, timeStampMax, TimeSpan.Zero);
        }

        public static long GetNextDayTimeStamp(long timeStamp1)
        {
            var dateTime = ToDateTime(timeStamp1);
            
            var nextDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(1);
            
            TimeSpan ts = nextDay - new DateTime(1970, 1, 1, 0, 0, 0);
            
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static bool IsSameDay(long timeStampMin, long timeStampMax, TimeSpan offset)
        {
            var compareMin = ToDateTime(timeStampMin);

            var compareMax = ToDateTime(timeStampMax);

            var start = new DateTime(compareMin.Year, compareMin.Month, compareMin.Day);

            if (compareMin.Year == compareMax.Year && compareMin.Month == compareMax.Month && compareMin.Day == compareMax.Day)
            {
                return true;
            }

            return false;
        }
    }
}