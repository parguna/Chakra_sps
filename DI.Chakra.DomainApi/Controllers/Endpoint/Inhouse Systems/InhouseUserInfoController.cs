using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Infrastructure.CustomObjects;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers.Endpoint.Inhouse_Systems
{
    [RoutePrefix("InhouseUserInfo")]
    public class InhouseUserInfoController : BaseController<DomainModel.InhouseUserInfo, EntityModel.InhouseUserInfo, Dto.InhouseUserInfo>
    {
        [HttpGet]
        [Route("DeleteByUserid")]
        public void DeleteByUserid(long userId, string currentUser)
        {
            DomainModel.InhouseUserInfo.DeleteByUserid(userId, currentUser);
        }

        [HttpGet]
        [Route("EndpointHasAnyRoleAndAssignedToUser")]
        public bool EndpointHasAnyRoleAndAssignedToUser(long endpointId)
        {
            return DomainModel.InhouseUserInfo.EndpointHasAnyRoleAndAssignedToUser(endpointId);
        }

        [HttpGet]
        [Route("GetByIdIncludeEndPoint")]
        public List<InhouseUserInfo> GetByIdIncludeEndPoint(long id)
        {
            return DtoMapList(DomainModel.InhouseUserInfo.GetByIdIncludeEndPoint(id));
        }

        [HttpGet]
        [Route("GetByUserName")]
        public List<InhouseUserInfo> GetByUserName(string userName, string inhousesystemcode)
        {
            return DtoMapList(DomainModel.InhouseUserInfo.GetByUserName(userName, inhousesystemcode));
        }
        [HttpGet]
        [Route("GetByEmployeeId")]
        public InhouseUserInfo GetByEmployeeId(string employeeId, long endpointId)
        {
            return DtoMap(DomainModel.InhouseUserInfo.GetByEmployeeId(endpointId, employeeId));
        }
        [HttpGet]
        [Route("GetAllAccountsByEmployeeId")]
        public List<InhouseUserInfo> GetAllAccountsByEmployeeId(string employeeId, long endpointId)
        {
            return DtoMapList(DomainModel.InhouseUserInfo.GetAllAccountsByEmployeeId(endpointId, employeeId));
        }
        [HttpGet]
        [Route("GetByUserName")]
        public InhouseUserInfo GetByUserName(long endPointSystemId, string userName)
        {
            return DtoMap(DomainModel.InhouseUserInfo.GetByUserName(endPointSystemId, userName));
        }

        [HttpPost]
        [Route("MapUser")]
        public void MapUser([FromBody] Dto.EndPoint endPoint)
        {
            DomainModel.InhouseUserInfo.MapUser(AutoMapperHelper.DtoUnMap<Dto.EndPoint, DomainModel.EndPoint>(endPoint));
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(long endPointSystemId, string userName)
        {
            return DomainModel.InhouseUserInfo.DoesExists(endPointSystemId, userName);
        }

        [HttpGet]
        [Route("EmailCompose")]
        public bool EmailCompose(long inHouseUserId)
        {
            //new EmailComposer().Compose(EmailTemplate.InHouseAccountCreation, inHouseUserId);
            return true;
        }

        [HttpGet]
        [Route("GetByUserNameAndEndPointSystemId")]
        public InhouseUserInfo GetByUserNameAndEndPointSystemId(string userName, long endPointSystemId)
        {
            return DtoMap(DomainModel.InhouseUserInfo.GetByUserNameAndEndPointSystemId(userName, endPointSystemId));
        }

        [HttpGet]
        [Route("CheckUserNamesExists")]
        public string CheckUserNamesExists(string userNames, long endPointSystemId)
        {
            return DomainModel.InhouseUserInfo.CheckUserNames(userNames, endPointSystemId);
        }

        [HttpPost]
        [Route("PopulateUserInfos")]
        public bool PopulateUserInfos(List<InhouseUserCustomInfo> inhCustomUserInfos)
        {
            return DomainModel.InhouseUserInfo.PopulateUserInfos(inhCustomUserInfos);
        }

        [HttpGet]
        [Route("DeleteByAccReconId")]
        public bool DeleteByAccReconId(long accReconId, string currentUser)
        {
            return DomainModel.InhouseUserInfo.DeleteByAccReconId(accReconId, currentUser);
        }
        [HttpPost]
        [Route("GetLazyLoadOtherSystemAuditTrial")]
        public Dto.Core.PresentationGridPagedData<Dto.PresentationAccountEntitlementAuditTrial> GetLazyLoadOtherSystemAuditTrial(PresentationGridPagedDataParameter param, string departmentCode = "", bool showAllNonEmployees = false)
        {
            var responseData = new Dto.Core.PresentationGridPagedData<Dto.PresentationAccountEntitlementAuditTrial>();
            var result = DomainModel.PresentationAccountEntitlementAuditTrial.GetLazyLoadOtherSystemAuditTrial(param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression, param.Ids, param.FromDate, param.ToDate);
            responseData.Data = DtoMapList(result.Data);
            responseData.TotalCount = result.TotalCount;
            return responseData;
        }
        [HttpPost]
        [Route("DownloadOtherSystemAuditTrial")]
        public byte[] DownloadOtherSystemAuditTrial(PresentationGridPagedDataParameter param, string exportFileType)
        {
            return DomainModel.PresentationAccountEntitlementAuditTrial.DownloadOtherSystemAuditTrial(param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression, exportFileType, param.Ids, param.FromDate, param.ToDate);
        }
        public List<DI.Chakra.Dto.PresentationAccountEntitlementAuditTrial> DtoMapList(IList<DomainModel.PresentationAccountEntitlementAuditTrial> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
        public DI.Chakra.Dto.PresentationAccountEntitlementAuditTrial DtoMap(DomainModel.PresentationAccountEntitlementAuditTrial tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.PresentationAccountEntitlementAuditTrial, DI.Chakra.Dto.PresentationAccountEntitlementAuditTrial>(tModel);
        }
    }
}
