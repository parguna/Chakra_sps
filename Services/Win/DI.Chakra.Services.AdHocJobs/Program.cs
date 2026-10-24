using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Services.AdHocJobs
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            AutoMapperHelper.BootStrap();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AdHocJobService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
