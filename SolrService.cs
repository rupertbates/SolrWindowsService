using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using Guardian.Configuration;
using System.IO;

namespace SolrWindowsService
{
    internal class SolrService
    {
        static Process process = new Process();

        public void Start()
        {
            Log("service starting");
            try
            {
                process.StartInfo.FileName = ConfigurationHelper.GetConfigValue("JavaExecutable");
                var port = ConfigurationHelper.GetConfigValueOrDefault("Port", 8983);
                var workingDirectory = ConfigurationHelper.GetConfigValue("WorkingDirectory");
                var jarFile = string.Format(@"{0}\start.jar", workingDirectory);
                if (!File.Exists(jarFile))
                    throw new ConfigurationErrorsException("Couldn't find the start.jar file at " + jarFile);
                var solrHome = ConfigurationHelper.GetConfigValueOrDefault("Solr.Home", "solr");
                var commandLineArgs = ConfigurationHelper.GetConfigValueOrDefault("CommandLineArgs", "");
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.Arguments = string.Format(@"-Dsolr.solr.home={0} -Djetty.port={3} {1} -jar {2}", solrHome, commandLineArgs, jarFile, port);
                process.StartInfo.UseShellExecute = ConfigurationHelper.GetConfigValueOrDefault("ShowConsole", false);

                var result = process.Start();
                Log("result of batch start: " + result);
                //process.WaitForExit();
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
            
            //EventLog.WriteEntry(message);
        }
        public void Stop()
        {
            //Log("stopping service");
            process.Kill();
            process.Dispose();
        }
    }
}
