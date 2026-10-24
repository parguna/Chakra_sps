using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainService.Controllers
{
    using DI.Chakra.DomainModel;
    [RoutePrefix("InhouseService")]
    public class InhouseServiceController : ApiController
    {
        //[HttpPost]
        //[Route("CreateUserPostAction")]
        //public void CreateUserPostAction([FromBody]InhouseUserInfo userInfo)
        //{
        //    try
        //    {
        //        var endpoint = EndPoint.GetByIdIncludingOwner(userInfo.EndPointSystemId);
        //        var connector = ConnectorFactory.GetConnector(endpoint, null);
        //        connector.CreateUserInReconTable(userInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Exception, string.Format
        //            ("Error while creating inhouse system post user create action for the user - {0}", userInfo.AccountName), ex);
        //    }
        //}

        //[HttpPost]
        //[Route("AddEntitlement")]
        //public void AddEntitlement([FromBody]InhouseUserInfo userInfo, EntitlementType entitlementType)
        //{
        //    try
        //    {
        //        var endpoint = EndPoint.GetByIdIncludingOwner(userInfo.EndPointSystemId);
        //        var connector = ConnectorFactory.GetConnector(endpoint, null);
        //        connector.AddEntitlement(userInfo, entitlementType);
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Exception, string.Format
        //            ("Error while adding entitlement from inhouse system for the user - {0}", userInfo.AccountName), ex);
        //    }
        //}

        //[HttpPost]
        //[Route("RemoveEntitlement")]
        //public void RemoveEntitlement([FromBody]InhouseUserInfo userInfo, EntitlementType entitlementType)
        //{
        //    try
        //    {
        //        var endpoint = EndPoint.GetByIdIncludingOwner(userInfo.EndPointSystemId);
        //        var connector = ConnectorFactory.GetConnector(endpoint, null);
        //        connector.RemoveEntitlement(userInfo, entitlementType);
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Exception, string.Format
        //            ("Error while removing entitlement from inhouse system for the user - {0}", userInfo.AccountName), ex);
        //    }
        //}

        //[HttpPost]
        //[Route("DisableUser")]
        //public void DisableUser(InhouseUserInfo userInfo)
        //{
        //    try
        //    {
        //        var endpoint = EndPoint.GetByIdIncludingOwner(userInfo.EndPointSystemId);
        //        var connector = ConnectorFactory.GetConnector(endpoint, null);
        //        connector.DisableUser(userInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Exception, string.Format
        //            ("Error while disabling inhouse system user - {0}", userInfo.AccountName), ex);
        //    }
        //}
    }
}
