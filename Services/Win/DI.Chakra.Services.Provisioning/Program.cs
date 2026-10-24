using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Services.Provisioning
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
            AutoMapper.Mapper.CreateMap<DomainModel.Office365LicenseConfig, Dto.Office365LicenseConfig>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.Office365GroupInfo, Dto.Office365GroupInfo>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.Office365License, Dto.Office365License>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.Office365MassLicense, Dto.Office365MassLicense>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.Office365RetentionPolicy, Dto.Office365RetentionPolicy>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.Office365RoleAssignmentPolicy, Dto.Office365RoleAssignmentPolicy>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.Office365ServicePlan, Dto.Office365ServicePlan>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.Office365SharingPolicy, Dto.Office365SharingPolicy>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.Office365UserInfo, Dto.Office365UserInfo>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.ServicePlanDetail, Dto.ServicePlanDetail>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.AdHocLicenseInfo, Dto.AdHocLicenseInfo>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.AdHocO365SharedAccount, Dto.AdHocO365SharedAccount>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.AdHocO365Request, Dto.AdHocO365Request>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DomainModel.AdHocO365SharedAccount, Dto.AdHocO365SharedAccount>().ReverseMap();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ProvisioningService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
