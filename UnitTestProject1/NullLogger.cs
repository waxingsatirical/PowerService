using IntradayServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    public class NullLogger : ILogger
    {
        public void Write(string entry)
        {
            return;
        }
    }
}
