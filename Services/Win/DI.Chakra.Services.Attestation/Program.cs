using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Services.Attestation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            AutoMapperHelper.BootStrap();
            AutoMapper.Mapper.CreateMap<DomainModel.AdHocO365Request, Dto.AdHocO365Request>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.AdHocO365SharedAccount, Dto.AdHocO365SharedAccount>().ReverseMap();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new AttestationService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
