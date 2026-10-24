using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("MetaData")]
    public class MetaDataController : BaseController<DomainModel.MetaData, EntityModel.MetaData, Dto.MetaData>
    {
        [HttpPost]
        [Route("UpdateMetadata")]
        public void UpdateMetadata([FromBody]Dto.MetaData presentationMetaData)
        {
            DomainModel.MetaData.UpdateMetadata(DtoUnMap(presentationMetaData),User.Identity.Name);
        }

        [HttpGet]
        [Route("GetMetadata")]
        public  IList<Dto.MetaData> GetMetadata(string MetadataType)
        {
          return DtoMapList(DomainModel.MetaData.GetMetadata(MetadataType));
        }
    }
}
