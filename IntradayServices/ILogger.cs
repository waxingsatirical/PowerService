using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntradayServices
{
    public interface ILogger
    {
        void Write(string entry);
    }
}
