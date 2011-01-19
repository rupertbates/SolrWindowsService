using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Guardian.Configuration;
using Topshelf;
using Topshelf.Configuration.Dsl;

namespace SolrWindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var instanceName = ConfigurationHelper.GetConfigValueOrDefault("InstanceName", "");
            var displayName = string.Format("{0}.Solr.Service", instanceName);
            var cfg = RunnerConfigurator.New(x =>
                                                 {
                                                     x.ConfigureService<SolrService>(s =>
                                                                                         {
                                                                                             s.Named("solr");
                                                                                             s.HowToBuildService(
                                                                                                 name =>
                                                                                                 new SolrService());
                                                                                             s.WhenStarted(
                                                                                                 solr => solr.Start());
                                                                                             s.WhenStopped(
                                                                                                 solr => solr.Stop());
                                                                                         });
                                                     x.RunAsLocalSystem();
                                                     x.SetDescription(string.Format("Starts up the {0}", displayName));
                                                     x.SetDisplayName(displayName);
                                                     x.SetServiceName(displayName);
                                                 }
                );
            Runner.Host(cfg, args);
        }
    }
}
