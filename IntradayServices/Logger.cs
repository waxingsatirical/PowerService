using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntradayServices
{
    public class Logger : ILogger
    {
        private LogWriter logWriter;

        private Logger()
        { }

        public static Logger Create()
        {
            LogWriterFactory logWriterFactory = new LogWriterFactory();
            var logger = new Logger();
            logger.logWriter = logWriterFactory.Create();
            return logger;
        }

        public void Write(string entry)
        {
            logWriter.Write(entry);
        }
    }
}
