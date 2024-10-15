using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business
{
    public static class DateTimeExtensions
    {
        public static string ToTimeAgo(this DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime.ToUniversalTime();

            if (timeSpan.TotalSeconds < 60)
                return $"{timeSpan.Seconds} saniye önce";
            if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.Minutes} dakika önce";
            if (timeSpan.TotalHours < 24)
                return $"{timeSpan.Hours} saat önce";
            if (timeSpan.TotalDays < 30)
                return $"{timeSpan.Days} gün önce";
            if (timeSpan.TotalDays < 365)
                return $"{timeSpan.Days / 30} ay önce";

            return $"{timeSpan.Days / 365} yıl önce";
        }
    }
}
