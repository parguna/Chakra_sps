using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DI.Chakra.DomainModel;
using DI.Chakra.Dto;
namespace TestHarness
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
          
           

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DI.Chakra.DomainModel.AutoMapperHelper.BootStrap();


            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.Office365LicenseConfig, DI.Chakra.Dto.Office365LicenseConfig>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.Office365GroupInfo, DI.Chakra.Dto.Office365GroupInfo>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.Office365License, DI.Chakra.Dto.Office365License>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.Office365MassLicense, DI.Chakra.Dto.Office365MassLicense>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.Office365RetentionPolicy, DI.Chakra.Dto.Office365RetentionPolicy>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.Office365RoleAssignmentPolicy, DI.Chakra.Dto.Office365RoleAssignmentPolicy>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.Office365ServicePlan, DI.Chakra.Dto.Office365ServicePlan>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.Office365SharingPolicy, DI.Chakra.Dto.Office365SharingPolicy>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.Office365UserInfo, DI.Chakra.Dto.Office365UserInfo>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.ServicePlanDetail, DI.Chakra.Dto.ServicePlanDetail>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.AdHocLicenseInfo, DI.Chakra.Dto.AdHocLicenseInfo>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.AdHocO365Request, DI.Chakra.Dto.AdHocO365Request>().ReverseMap();
            AutoMapper.Mapper.CreateMap<DI.Chakra.DomainModel.AdHocO365SharedAccount, DI.Chakra.Dto.AdHocO365SharedAccount>().ReverseMap();

            Application.Run(new Form1());
            //Application.Run(new ResolveExceptions());
            //Application.Run(new LNSForm());
            //Application.Run(new TestForm());
        }
    }
}
