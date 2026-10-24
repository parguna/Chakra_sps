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
    [RoutePrefix("PresentationExportAttestationReportForUI")]
    public class PresentationExportAttestationReportForUIController : ApiController
    {
        [HttpGet]
        [Route("GetAttestationTaskReviewForExportFlat")]
        public PresentationExportAttestationReportForUI GetAttestationTaskReviewForExport(long id, bool isInitialLoad, string currentUser)
        {
            var output = DomainModel.Presentation.PresentationExportAttestationEntitlementReviewDetail.GetAttestationTaskReviewForExportFlat(id, isInitialLoad, currentUser);
            return DtoMap(output);
        }

        [HttpGet]
        [Route("GetAttestationTaskReviewForExportHierarchy")]
        public PresentationExportAttestationReportForUI GetAttestationTaskReviewForExportHierarchy(long id, bool isInitialLoad, string currentUser)
        {
            var output = DomainModel.Presentation.PresentationExportAttestationEntitlementReviewDetail.GetAttestationTaskReviewForExportHierarchy(id, isInitialLoad, currentUser);
            return DtoMap(output);
        }

        public PresentationExportAttestationReportForUI DtoMap(DomainModel.Presentation.PresentationExportAttestationReportForUI tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationExportAttestationReportForUI, PresentationExportAttestationReportForUI>(tModel);
        }

        public List<PresentationExportAttestationReportForUI> DtoMapList(IList<DomainModel.Presentation.PresentationExportAttestationReportForUI> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}