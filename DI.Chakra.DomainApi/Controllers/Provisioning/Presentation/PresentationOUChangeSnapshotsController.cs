using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using AutoMapper;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationOUChangeSnapshots")]
    public class PresentationOUChangeSnapshotController : ApiController
    {
        [HttpPost]
        [Route("GetByUserId")]
        public List<Dto.Presentation.PresentationOUChangeSnapshots> GetByUserId([FromBody]List<long> listUserID, bool includeOldRecord = false)
        {
            return DtoMapList(DomainModel.Presentation.PresentationOUChangeSnapshots.GetByUserId(listUserID, includeOldRecord));
        }

        public Dto.Presentation.PresentationOUChangeSnapshots DtoMap(DomainModel.Presentation.PresentationOUChangeSnapshots tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationOUChangeSnapshots, Dto.Presentation.PresentationOUChangeSnapshots>(tModel);
        }

        public List<Dto.Presentation.PresentationOUChangeSnapshots> DtoMapList(IList<DomainModel.Presentation.PresentationOUChangeSnapshots> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }

}
