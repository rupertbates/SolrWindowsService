using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
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
                                                     x.SetDescription("Starts up the Solr search service");
                                                     x.SetDisplayName("Solr Service");
                                                     x.SetServiceName("Solr.Service");
                                                 }
                );
            Runner.Host(cfg, args);
        }
    }
}
