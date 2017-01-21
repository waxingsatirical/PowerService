using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace IntradayServices
{
    public partial class AggregatedReportService : ServiceBase
    {
        private ReportRunner runner;
        private Logger logger;
        
        public AggregatedReportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger = Logger.Create();
            logger.Write("Starting report runner");
            var startTime = DateTime.Now;
            var intervalMilliseconds = AppSettings.Default.intervalMinutes * 60 * 1000;
            runner = new ReportRunner(intervalMilliseconds, startTime, AppSettings.Default.outputDirectory, logger);
            runner.Start();
            logger.Write("Report runner started");
        }

        protected override void OnStop()
        {
            if (runner != null)
            {
                runner.Dispose();
            }
            base.OnStop();
        }
    }
}
