using AutoMapper;
using DI.Chakra.DomainModel.Presentation;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using DI.Chakra.Dto.Core;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationTaskdelegation")]
    public class PresentationTaskdelegationController : ApiController
    {
        //[HttpGet]
        //[Route("GetAllDelegationByActionUser")]
        //public List<Dto.Presentation.PresentationTaskdelegation> GetAllDelegationByActionUser(long actionUser, Nullable<bool> isAdmin = null)
        //{
        //    return DtoMapList(DomainModel.Presentation.PresentationTaskdelegation.GetAllDelegationByActionUser(actionUser, isAdmin));
        //}
        [HttpPost]
        [Route("GetPagedAllDelegationByActionUser")]
        public PresentationGridPagedData<Dto.Presentation.PresentationTaskdelegation> GetPagedAllDelegationByActionUser(long actionUser, Dto.Core.PresentationGridPagedDataParameter param, Nullable<bool> isAdmin = null,bool? includeInActive=null)
        {
            var response = new PresentationGridPagedData<Dto.Presentation.PresentationTaskdelegation>();
            var result = DomainModel.Presentation.PresentationTaskdelegation.GetAllDelegationByActionUser(actionUser, isAdmin !=null ?isAdmin.Value : false, includeInActive ?? false,param.SortExpression, param.FilterExpression, param.StartRowIndex, param.MaximumRows);
            response.TotalCount = result.TotalCount;
            response.Data = DtoMapList(result.Data);
            return response;
        }
        [HttpPost]
        [Route("DownloadDelegation")]
        public byte[] DownloadDelegation(long actionUser, Dto.Core.PresentationGridPagedDataParameter param,string exportFileType, Nullable<bool> isAdmin = null,bool? includeInactive=null)
        {
            return DomainModel.Presentation.PresentationTaskdelegation.DownloadDelegation(actionUser, isAdmin != null ? isAdmin.Value : false, includeInactive ?? false,param.SortExpression, param.FilterExpression, param.StartRowIndex, param.MaximumRows, exportFileType);
        }
        public Dto.Presentation.PresentationTaskdelegation DtoMap(DomainModel.Presentation.PresentationTaskdelegation tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationTaskdelegation, Dto.Presentation.PresentationTaskdelegation>(tModel);
        }
        public List<Dto.Presentation.PresentationTaskdelegation> DtoMapList(IList<DomainModel.Presentation.PresentationTaskdelegation> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
