using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AttestationCampaignReport")]
    public class AttestationCampaignReportController : BaseController<DomainModel.AttestationCampaignReport, EntityModel.AttestationCampaignReport, AttestationCampaignReport>
    {
        [Route("GetByAttestationCampaignId")]
        [HttpGet]
        public IList<Dto.AttestationCampaignReport> GetByAttestationCampaignId(long attestationCampaignId)
        {
            return DtoMapList(DomainModel.AttestationCampaignReport.GetByAttestationCampaignId(attestationCampaignId));
        }

        [Route("GetReportByType")]
        [HttpGet]
        public Dto.AttestationCampaignReport GetReportByType(long attestationCampaignId, AttestationReportType reportType)
        {
            return DtoMap(DomainModel.AttestationCampaignReport.GetReportByType(attestationCampaignId, reportType));
        }

        public AttestationCampaignReport DtoMap(DomainModel.AttestationCampaignReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.AttestationCampaignReport, AttestationCampaignReport>(tModel);
        }

        public List<AttestationCampaignReport> DtoMapList(IList<DomainModel.AttestationCampaignReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}