using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using AutoMapper;
using DI.Chakra.ProvisioningUtility;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationAccessSnapshots")]
    public class PresentationAccessSnapshotsController : ApiController
    {
        [HttpGet]
        [Route("GetByUserId")]
        public List<Dto.Presentation.PresentationAccessSnapshots> GetByUserId(long userID)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAccessSnapshots.GetByUserId(userID));
        }

        [HttpGet]
        [Route("ExecuteUndoProvisioningEntitlements")]
        public bool ExecuteUndoProvisioningEntitlements(long userID, long requestorID)
        {
            try
            {
                UndoProvisioningEntitlements obj = new UndoProvisioningEntitlements(userID, requestorID);
                obj.Execute();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }

        public Dto.Presentation.PresentationAccessSnapshots DtoMap(DomainModel.Presentation.PresentationAccessSnapshots tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationAccessSnapshots, Dto.Presentation.PresentationAccessSnapshots>(tModel);
        }

        public List<Dto.Presentation.PresentationAccessSnapshots> DtoMapList(IList<DomainModel.Presentation.PresentationAccessSnapshots> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }

}
