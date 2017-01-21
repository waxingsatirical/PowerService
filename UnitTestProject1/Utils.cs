using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    public static class Utils
    {
        public static PowerTrade Generate(DateTime date, Dictionary<int, double> periods)
        {
            var p = PowerTrade.Create(date, periods.Count());
            for (int i = 0; i < p.Periods.Length; i++)
            {
                p.Periods[i].Volume = periods[i + 1];
            }
            return p;
        }

        public static IEnumerable<PowerTrade> Sync(PowerService ps, DateTime date)
        {
            var tradesTask = ps.GetTradesAsync(date);
            tradesTask.Wait();
            return tradesTask.Result;
        }
    }
}
