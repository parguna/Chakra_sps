using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ApproverExceptionInfo")]
    public class ApproverExceptionInfoController: BaseController<DomainModel.ApproverExceptionInfo, EntityModel.ApproverExceptionInfo, Dto.ApproverExceptionInfo>
    {
        [HttpGet]
        [Route("GetApproverException")]
        public List<Dto.ApproverExceptionInfo> GetApproverException(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.ApproverExceptionInfo.GetApproverException(isValidRecords, isAllRecords));
        }
    }
}