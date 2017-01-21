using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    public class TestPowerService : IPowerService
    {
        public IEnumerable<PowerTrade> Trades { get; set; }

        public IEnumerable<PowerTrade> GetTrades(DateTime date)
        {
            return Trades;
        }

        public Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            return Task.Run<IEnumerable<PowerTrade>>(() => Trades);
        }
    }
}
