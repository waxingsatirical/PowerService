using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Services;
using System.IO;

namespace IntradayServices
{
    public class ReportRunner : IDisposable
    {
        private Timer intervalTimer;
        private int intervalMilliseconds;
        private int runCount;
        private DateTime startTimeLocal;
        private string folderPath;
        private Totaller totaller;
        private ILogger logger { get; set; }

        public ReportRunner(int intervalMilliseconds, DateTime startTimeLocal, string folderPath, ILogger logger)
        {
            this.intervalMilliseconds = intervalMilliseconds;
            this.startTimeLocal = startTimeLocal;
            this.folderPath = folderPath;
            this.logger = logger;
        }

        public void Start()
        {
            logger.Write("Initialising PowerService");
            totaller = new Totaller(new PowerService());

            startTimeLocal = getCurrentTime();
            intervalTimer = new Timer(produceReport, null, 0, intervalMilliseconds);
        }

        public void Dispose()
        {
            if (intervalTimer != null)
            {
                intervalTimer.Dispose();
            }
        }

        private void produceReport(object state)
        {
            try
            {
                var currentTime = getCurrentTime();

                logger.Write(string.Format("Producing report at {0}", currentTime.ToString("yyyyMMdd HH:mm")));
                var totals = totaller.Totals(currentTime).ToArray();
                writeReport(totals.Select(t => formatAsCsvLine(t)), currentTime);
            }
            catch (Exception ex)
            {
                logger.Write("Unexpected exception encountered");
                logger.Write(ex.ToString());

                throw;
            }
            runCount++;
        }


        private void writeReport(IEnumerable<string> lines, DateTime queryDateTime)
        {
            var filePath = fileName(folderPath, queryDateTime);

            if (File.Exists(filePath))
            {
                filePath = fileName(folderPath, queryDateTime.AddMinutes(1));
            }

            logger.Write(string.Format("Writing report {0}", filePath));
            using (var fileWriter = new StreamWriter(filePath))
            {
                fileWriter.WriteLine("Local Time,Volume");
                foreach (var l in lines)
                {
                    fileWriter.WriteLine(l);
                }
            }
        }

        private string fileName(string folderPath, DateTime queryDateTime)
        {
            return Path.Combine(folderPath, string.Format("PowerPosition_{0}_{1}.csv", queryDateTime.ToString("yyyyMMdd"), queryDateTime.ToString("HHmm")));
        }

        private string formatAsCsvLine(Total total)
        {
            return string.Format("{0},{1}", total.LocalTime.ToString(@"hh\:mm"), total.Volume.ToString());
        }


        protected DateTime getCurrentTime()
        {
            return DateTime.Now;
        }

    }
}
