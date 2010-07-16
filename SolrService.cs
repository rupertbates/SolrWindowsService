using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using Guardian.Configuration;
using System.IO;

namespace SolrWindowsService
{
    partial class SolrService : ServiceBase
    {
        static Process process = new Process();

        public SolrService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log("service starting");
            try
            {
                process.StartInfo.FileName = ConfigurationHelper.GetConfigValue("JavaExecutable");

                var workingDirectory = ConfigurationHelper.GetConfigValue("WorkingDirectory");
                var jarFile = string.Format(@"{0}\start.jar", workingDirectory);
                if (!File.Exists(jarFile))
                    throw new ConfigurationErrorsException("Couldn't find the start.jar file at " + jarFile);

                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.Arguments = string.Format(@"-Dsolr.solr.home={0} -jar {1}", ConfigurationHelper.GetConfigValueOrDefault("Solr.Home", "solr"), jarFile);
                process.StartInfo.UseShellExecute = ConfigurationHelper.GetConfigValueOrDefault("ShowConsole", false);

                var result = process.Start();
                Log("result of batch start: " + result);
            }
            catch (Exception ex)
            {
                Log("An error occurred: " + ex.Message);
                throw;
            }
            
        }
        protected void Log(string message)
        {
            const string source = "SolrService";
            const string log = "Application";

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, log);
            
            EventLog.WriteEntry(message);
        }
        protected override void OnStop()
        {
            //Log("stopping service");
            process.Kill();
            process.Dispose();
        }
    }
}
