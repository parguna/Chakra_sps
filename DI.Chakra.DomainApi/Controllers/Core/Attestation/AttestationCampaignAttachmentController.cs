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
    [RoutePrefix("AttestationCampaignAttachment")]
    public class AttestationCampaignAttachmentController : BaseController<DomainModel.AttestationCampaignAttachment, EntityModel.AttestationCampaignAttachment, AttestationCampaignAttachment>
    {
        [Route("GetByAttestationCampaignId")]
        [HttpGet]
        public IList<Dto.AttestationCampaignAttachment> GetByAttestationCampaignId(long attestationCampaignId)
        {
            return DtoMapList(DomainModel.AttestationCampaignAttachment.GetByAttestationCampaignId(attestationCampaignId));
        }

        [Route("GetAttachmentById")]
        [HttpGet]
        public Dto.AttestationCampaignAttachment GetAttachmentById(long attachmentId, string fileName)
        {
            return DtoMap(DomainModel.AttestationCampaignAttachment.GetAttachmentById(attachmentId, fileName));
        }

        [Route("GetAttachmentCount")]
        [HttpGet]
        public long GetAttachmentCount(long attestationCampaignId)
        {
            return DomainModel.AttestationCampaignAttachment.GetAttachmentCount(attestationCampaignId);
        }

        [HttpPost]
        [Route("SaveAttachementList")]
        public bool SaveAttachementList([FromBody]List<AttestationCampaignAttachment> attachmentList)
        {
            var dmainList = DtoUnMapList(attachmentList);
            foreach (var item in dmainList)
            {
                item.Save(item.CreatedBy);
            }

            return true;
        }
        public AttestationCampaignAttachment DtoMap(DomainModel.AttestationCampaignAttachment tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.AttestationCampaignAttachment, AttestationCampaignAttachment>(tModel);
        }

        public List<AttestationCampaignAttachment> DtoMapList(IList<DomainModel.AttestationCampaignAttachment> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}