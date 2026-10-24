using AutoMapper;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationAttetstationCampaign")]
    public class PresentationAttetstationCampaingController : ApiController
    {
        [HttpGet]
        [Route("GetCampaignStatus")]
        public IList<PresentationAttetstationCampaign> GetCampaignStatus(long AttestationCampaignId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAttetstationCampaign.GetCampaignStatus(AttestationCampaignId));
        }

        [HttpGet]
        [Route("GetAttestationOverallStatus")]
        public IList<PresentationAttetstationCampaign> GetAttestationOverallStatus()
        {
            return DtoMapList(DomainModel.Presentation.PresentationAttetstationCampaign.GetAttestationOverallStatus());
        }

        [HttpGet]
        [Route("GetCampaignReport")]
        public IList<PresentationAttetstationCampaign> GetCampaignReport()
        {
            return DtoMapList(DomainModel.Presentation.PresentationAttetstationCampaign.GetCampaignReport());
        }

        public PresentationAttetstationCampaign DtoMap(DomainModel.Presentation.PresentationAttetstationCampaign tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationAttetstationCampaign, PresentationAttetstationCampaign>(tModel);
        }

        public List<PresentationAttetstationCampaign> DtoMapList(IList<DomainModel.Presentation.PresentationAttetstationCampaign> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
        [HttpPost]
        [Route("GetPagedCampaignReport")]
        public PresentationGridPagedData<PresentationAttetstationCampaign> GetPagedCampaignReport(PresentationGridPagedDataParameter param)
        {
            var dataResult = new PresentationGridPagedData<PresentationAttetstationCampaign>();
            var result = DomainModel.Presentation.PresentationAttetstationCampaign.GetPresentationGridPagedData(param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression);
            dataResult.Data = DtoMapList(result.Data);
            dataResult.TotalCount = result.TotalCount;
            return dataResult;
        }
        [HttpPost]
        [Route("UserProfileComparisionCampaignReport")]
        public PresentationGridPagedData<PresentationAttetstationCampaign> GetUserProfileComparisionCampaignReport(PresentationGridPagedDataParameter param)
        {
            var dataResult = new PresentationGridPagedData<PresentationAttetstationCampaign>();
            var result = DomainModel.Presentation.PresentationAttetstationCampaign.GetUserProfileComparisionCampaignReport(param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression);
            dataResult.Data = DtoMapList(result.Data);
            dataResult.TotalCount = result.TotalCount;
            return dataResult;
        }
    }
}