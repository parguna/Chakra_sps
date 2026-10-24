using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationADSUserToGroups")]
    
    public class PresentationADSUserToGroupsController :ApiController
    {
        public  PresentationADSUserToGroups DtoMap(DomainModel.Presentation.PresentationADSUserToGroups tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationADSUserToGroups, PresentationADSUserToGroups>(tModel);
        }

        public  List<PresentationADSUserToGroups> DtoMapList(IList<DomainModel.Presentation.PresentationADSUserToGroups> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetADSUserToGroupsReportData")]
        public IList<PresentationADSUserToGroups> GetADSUserToGroupsReportData(long endpointsystemid)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSUserToGroups.GetADSUserToGroupsReportData(endpointsystemid));
        }

        [HttpGet]
        [Route("GetADSUserToGroupsReportDataByDepartment")]
        public IList<PresentationADSUserToGroups> GetADSUserToGroupsReportDataByDepartment(long endpointsystemid, long departmentCode, long CutOff)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSUserToGroups.GetADSUserToGroupsReportDataByDepartment(endpointsystemid, departmentCode, CutOff));
        }

        [HttpGet]
        [Route("GetADSUserToGroupsReportDataByDepartmentAndGroup")]
        public IList<PresentationADSUserToGroups> GetADSUserToGroupsReportDataByDepartmentAndGroup(long endpointsystemid, long departmentCode, string GroupName)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSUserToGroups.GetADSUserToGroupsReportDataByDepartmentAndGroup(endpointsystemid, departmentCode, GroupName));
        }
    }
}
