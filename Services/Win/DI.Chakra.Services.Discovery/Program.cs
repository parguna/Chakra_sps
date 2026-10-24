using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;


namespace DI.Chakra.Services.Discovery
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 |  System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

            AutoMapperHelper.BootStrap();
            AutoMapper.Mapper.CreateMap<DomainModel.AdHocO365Request, Dto.AdHocO365Request>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.AdHocO365SharedAccount, Dto.AdHocO365SharedAccount>().ReverseMap();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new DiscoveryService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
