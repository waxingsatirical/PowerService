using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntradayServices
{
    public class Totaller
    {
        private IPowerService powerService;
        private TimeZoneInfo localTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        public Totaller(IPowerService powerService)
        {
            this.powerService = powerService;
        }

        public IEnumerable<Total> Totals(DateTime queryDateTime)
        {
            bool pseExceptionCaught = false;
            IEnumerable<PowerTrade> trades = null;
            do
            {
                try
                {
                    trades = powerService.GetTrades(queryDateTime);
                }
                catch (Exception pse)
                {
                    pseExceptionCaught = pse.Message == "Error retrieving power volumes";
                    if (pseExceptionCaught)
                    {
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
                pseExceptionCaught = false;
            }
            while (pseExceptionCaught);
            var firstPeriodLocal = queryDateTime.Date.AddHours(-1);
            var tmp = firstPeriodLocal.Add(localTimeZoneInfo.GetUtcOffset(firstPeriodLocal));
            var firstPeriodUtc = new DateTime(tmp.Year, tmp.Month, tmp.Day, tmp.Hour, 0, 0, DateTimeKind.Utc);
            var localTimeHelper = trades.SelectMany(t => t.Periods).Select( p => new { PeriodTimeUtc = firstPeriodUtc.AddHours(p.Period - 1), Volume = p.Volume })
                .Select(u => new { PeriodTimeLocal = TimeZoneInfo.ConvertTimeFromUtc(u.PeriodTimeUtc, localTimeZoneInfo), Volume = u.Volume });
            var grouped = localTimeHelper.GroupBy(h => h.PeriodTimeLocal);
            var dict = grouped.ToDictionary(g => g.Key, g => g.Sum(i => i.Volume));

            for (int i = 0; i < 24; i++)
            {
                var lt = firstPeriodLocal.AddHours(i);
                double tv;
                dict.TryGetValue(lt, out tv);
                yield return new Total()
                {
                    LocalTime = lt.TimeOfDay,
                    Volume = tv
                };
            }
        }
    }
}
