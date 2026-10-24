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
    [RoutePrefix("PresentationComplianceProcess")]
    public class PresentationComplianceProcessController : ComplianceProcessController
    {
        [HttpGet]
        [Route("GetComplianceProcessJobs")]
        public List<PresentationComplianceProcess> GetComplianceProcessJobs(int startRow, int PageSize, string filter)
        {
            return DtoMapList(DomainModel.Presentation.PresentationComplianceProcess.GetComplianceProcessJobs(startRow,PageSize,filter));
        }

        public  PresentationComplianceProcess DtoMap(DomainModel.Presentation.PresentationComplianceProcess tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationComplianceProcess, PresentationComplianceProcess>(tModel);
        }

        public  List<PresentationComplianceProcess> DtoMapList(IList<DomainModel.Presentation.PresentationComplianceProcess> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
