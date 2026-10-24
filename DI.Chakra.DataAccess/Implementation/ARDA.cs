using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DataAccess
{
    public class ARDA
    {
        public List<AccessRequestItem> GetAllApplicationAccessRequest(string searchString, List<EndpointType> EndpointSystem)
        {
            try
            {
                EndPointDA endpointDA = new EndPointDA();

                if (EndpointSystem == null || EndpointSystem.Count == 0)
                {
                    return (from items in endpointDA.GetAllEndpoint()
                            where items.DisplayName.ToUpper().Contains(searchString.ToUpper())
                            orderby items.DisplayName
                            select new AccessRequestItem
                            {
                                AccessRequestItemType = AccessRequestItemType.Application,
                                EndpointType = (EndpointType)items.EndpointSystemTypeId,
                                KeyId = items.Id,
                                EndpointSystemId = items.Id,
                                EndpointSystemTypeId = items.EndpointSystemTypeId.Value,
                                ItemName = items.DisplayName,
                                ItemDescription = items.Description,
                                ItemDetail=items.Description
                            }).ToList();

                }
                List<AccessRequestItem> ReturnList = new List<AccessRequestItem>();
                foreach (EndpointType Endpoint in EndpointSystem)
                {
                    ReturnList.AddRange((from items in endpointDA.GetAllEndpoint()
                                         where items.EndpointSystemTypeId == (int)Endpoint && items.DisplayName.ToUpper().Contains(searchString.ToUpper())
                                         orderby items.DisplayName
                                         select new AccessRequestItem
                                         {
                                             AccessRequestItemType = AccessRequestItemType.Application,
                                             EndpointType = (EndpointType)items.EndpointSystemTypeId,
                                             KeyId = items.Id,
                                             EndpointSystemId = items.Id,
                                             EndpointSystemTypeId = items.EndpointSystemTypeId.Value,
                                             ItemName = items.DisplayName,
                                             ItemDescription = items.Description,
                                             ItemDetail=items.Description

                                         }).ToList());
                }
                return ReturnList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<AccessRequestItem> GetAllEntitlementForAccessRequest(string searchString, DI.Chakra.Model.AccessItemType accessType, List<EndpointType> EndpointSystem)
        {
            try
            {
                bool isCatalog = false;

                if (accessType == Model.AccessItemType.Catalog)
                    isCatalog = true;

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    List<AccessRequestItem> ReturnList = new List<AccessRequestItem>();
                    if (EndpointSystem == null || EndpointSystem.Count == 0)
                    {
                        var EntitlementAD = new ActiveDirectoryDA().GetAccessRequestItem(searchString,isCatalog);
                        ReturnList.AddRange(EntitlementAD);

                        var EntitlementLNS = new LotusNotesDA().GetAccessRequestItem(searchString, isCatalog);
                        ReturnList.AddRange(EntitlementLNS);

                        var EntitlementSAP = new SAPConnectorDA().GetAccessRequestItem(searchString, isCatalog);
                        ReturnList.AddRange(EntitlementSAP);

                        var EntitlementLINUX = new LinuxDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(EntitlementLINUX);

                        var EntitlementOEBS = new  OracleEBSDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(EntitlementOEBS);

                        var EntitlementORCLUM = new ORCLUMDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(EntitlementORCLUM);
                    }
                    else
                    {
                        foreach (EndpointType Endpoint in EndpointSystem)
                        {
                            if (Endpoint == EndpointType.ActiveDirectory)
                            {
                                var Entitlement = new ActiveDirectoryDA().GetAccessRequestItem(searchString, isCatalog);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.LotusNotes)
                            {
                                var Entitlement = new LotusNotesDA().GetAccessRequestItem(searchString, isCatalog);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.SAP)
                            {
                                var Entitlement = new SAPConnectorDA().GetAccessRequestItem(searchString, isCatalog);
                                ReturnList.AddRange(Entitlement);
                            }

                            else if (Endpoint == EndpointType.Linux)
                            {
                                var Entitlement = new LinuxDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.OracleEBS)
                            {
                                var Entitlement = new OracleEBSDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.ORCLUM)
                            {
                                var Entitlement = new ORCLUMDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                        }
                    }
                    return ReturnList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<AccessRequestItem> GetAllEntitlementForRemoveRequest(string searchString, List<EndpointType> EndpointSystem, long DIUserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    List<AccessRequestItem> ReturnList = new List<AccessRequestItem>();
                    if (EndpointSystem == null || EndpointSystem.Count == 0)
                    {
                        var EntitlementAD = new ActiveDirectoryDA().GetAccessRequestItemForRemoveRequest(searchString,DIUserId).Where(t=>t.AccessRequestItemType== AccessRequestItemType.Entitlement);
                        ReturnList.AddRange(EntitlementAD);

                        var EntitlementLNS = new LotusNotesDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                        ReturnList.AddRange(EntitlementLNS);

                        var EntitlementSAP = new SAPConnectorDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                        ReturnList.AddRange(EntitlementSAP);

                        var EntitlementLINUX = new LinuxDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(EntitlementLINUX);

                        var EntitlementOEBS = new OracleEBSDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(EntitlementOEBS);

                        var EntitlementORCLUM = new ORCLUMDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(EntitlementORCLUM);


                        
                    }
                    else
                    {
                        foreach (EndpointType Endpoint in EndpointSystem)
                        {
                            if (Endpoint == EndpointType.ActiveDirectory)
                            {
                                var Entitlement = new ActiveDirectoryDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.LotusNotes)
                            {
                                var Entitlement = new LotusNotesDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.SAP)
                            {
                                var Entitlement = new SAPConnectorDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                                ReturnList.AddRange(Entitlement);
                            }


                            else if (Endpoint == EndpointType.Linux)
                            {
                                var Entitlement = new LinuxDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.OracleEBS)
                            {
                                var Entitlement = new OracleEBSDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.ORCLUM)
                            {
                                var Entitlement = new ORCLUMDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                        }
                    }
                    return ReturnList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<AccessRequestItem> GetAllApplicationForRemoveRequest(string searchString, List<EndpointType> EndpointSystem, long DIUserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    List<AccessRequestItem> ReturnList = new List<AccessRequestItem>();
                    if (EndpointSystem == null || EndpointSystem.Count == 0)
                    {
                        var appAD = new ActiveDirectoryDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                        ReturnList.AddRange(appAD);

                        var appLNS = new LotusNotesDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                        ReturnList.AddRange(appLNS);

                        var appSAP = new SAPConnectorDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                        ReturnList.AddRange(appSAP);

                        var appLINUX = new LinuxDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(appLINUX);

                        var appOEBS = new OracleEBSDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(appOEBS);

                        var appORCLUM = new ORCLUMDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(appORCLUM);
                    }
                    else
                    {
                        foreach (EndpointType Endpoint in EndpointSystem)
                        {
                            if (Endpoint == EndpointType.ActiveDirectory)
                            {
                                var Entitlement = new ActiveDirectoryDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.LotusNotes)
                            {
                                var Entitlement = new LotusNotesDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.SAP)
                            {
                                var Entitlement = new SAPConnectorDA().GetAccessRequestItemForRemoveRequest(searchString, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                                ReturnList.AddRange(Entitlement);
                            }


                            else if (Endpoint == EndpointType.Linux)
                            {
                                var Entitlement = new LinuxDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.OracleEBS)
                            {
                                var Entitlement = new OracleEBSDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.ORCLUM)
                            {
                                var Entitlement = new ORCLUMDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                        }
                    }
                    return ReturnList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<AccessRequestItem> GetAllApplicationForRemoveRequestSpecailACR(AccessRequestFilterCriteria Criteria, List<EndpointType> EndpointSystem, long DIUserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    List<AccessRequestItem> ReturnList = new List<AccessRequestItem>();
                    if (EndpointSystem == null || EndpointSystem.Count == 0)
                    {
                        var appAD = new ActiveDirectoryDA().GetAccessRequestItemForRemoveRequestSpecailACR(Criteria, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                        ReturnList.AddRange(appAD);

                        var appLNS = new LotusNotesDA().GetAccessRequestItemForRemoveRequestSpecailACR(Criteria, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                        ReturnList.AddRange(appLNS);

                        var appSAP = new SAPConnectorDA().GetAccessRequestItemForRemoveRequestSpecailACR(Criteria, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                        ReturnList.AddRange(appSAP);

                        //var appLINUX = new LinuxDA().GetAccessRequestItemSpecailACR(Criteria);
                        //ReturnList.AddRange(appLINUX);

                        //var appOEBS = new OracleEBSDA().GetAccessRequestItemSpecailACR(Criteria);
                        //ReturnList.AddRange(appOEBS);

                        //var appORCLUM = new ORCLUMDA().GetAccessRequestItemSpecailACR(Criteria);
                        //ReturnList.AddRange(appORCLUM);
                    }
                    else
                    {
                        foreach (EndpointType Endpoint in EndpointSystem)
                        {
                            if (Endpoint == EndpointType.ActiveDirectory)
                            {
                                var Entitlement = new ActiveDirectoryDA().GetAccessRequestItemForRemoveRequestSpecailACR(Criteria, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.LotusNotes)
                            {
                                var Entitlement = new LotusNotesDA().GetAccessRequestItemForRemoveRequestSpecailACR(Criteria, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.SAP)
                            {
                                var Entitlement = new SAPConnectorDA().GetAccessRequestItemForRemoveRequestSpecailACR(Criteria, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Application);
                                ReturnList.AddRange(Entitlement);
                            }


                            //else if (Endpoint == EndpointType.Linux)
                            //{
                            //    var Entitlement = new LinuxDA().GetAccessRequestItemSpecailACR(Criteria);
                            //    ReturnList.AddRange(Entitlement);
                            //}
                            //else if (Endpoint == EndpointType.OracleEBS)
                            //{
                            //    var Entitlement = new OracleEBSDA().GetAccessRequestItemSpecailACR(Criteria);
                            //    ReturnList.AddRange(Entitlement);
                            //}
                            //else if (Endpoint == EndpointType.ORCLUM)
                            //{
                            //    var Entitlement = new ORCLUMDA().GetAccessRequestItemSpecailACR(Criteria);
                            //    ReturnList.AddRange(Entitlement);
                            //}
                        }
                    }
                    return ReturnList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AccessRequestItem> GetAllEntitlementForRemoveRequestSpecailACR(AccessRequestFilterCriteria Criteria, List<EndpointType> EndpointSystem, long DIUserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    List<AccessRequestItem> ReturnList = new List<AccessRequestItem>();
                    if (EndpointSystem == null || EndpointSystem.Count == 0)
                    {
                        var EntitlementAD = new ActiveDirectoryDA().GetAccessRequestItemForRemoveRequestSpecailACR(Criteria, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                        ReturnList.AddRange(EntitlementAD);

                        var EntitlementLNS = new LotusNotesDA().GetAccessRequestItemForRemoveRequestSpecailACR(Criteria, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                        ReturnList.AddRange(EntitlementLNS);

                        var EntitlementSAP = new SAPConnectorDA().GetAccessRequestItemForRemoveRequestSpecailACR(Criteria, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                        ReturnList.AddRange(EntitlementSAP);

                        //var EntitlementLINUX = new LinuxDA().GetAccessRequestItem(searchString);
                        //ReturnList.AddRange(EntitlementLINUX);

                        //var EntitlementOEBS = new OracleEBSDA().GetAccessRequestItem(searchString);
                        //ReturnList.AddRange(EntitlementOEBS);

                        //var EntitlementORCLUM = new ORCLUMDA().GetAccessRequestItem(searchString);
                        //ReturnList.AddRange(EntitlementORCLUM);



                    }
                    else
                    {
                        foreach (EndpointType Endpoint in EndpointSystem)
                        {
                            if (Endpoint == EndpointType.ActiveDirectory)
                            {
                                var Entitlement = new ActiveDirectoryDA().GetAccessRequestItemForRemoveRequest(Criteria.SearchKey, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.LotusNotes)
                            {
                                var Entitlement = new LotusNotesDA().GetAccessRequestItemForRemoveRequest(Criteria.SearchKey, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.SAP)
                            {
                                var Entitlement = new SAPConnectorDA().GetAccessRequestItemForRemoveRequest(Criteria.SearchKey, DIUserId).Where(t => t.AccessRequestItemType == AccessRequestItemType.Entitlement);
                                ReturnList.AddRange(Entitlement);
                            }


                            //else if (Endpoint == EndpointType.Linux)
                            //{
                            //    var Entitlement = new LinuxDA().GetAccessRequestItem(searchString);
                            //    ReturnList.AddRange(Entitlement);
                            //}
                            //else if (Endpoint == EndpointType.OracleEBS)
                            //{
                            //    var Entitlement = new OracleEBSDA().GetAccessRequestItem(searchString);
                            //    ReturnList.AddRange(Entitlement);
                            //}
                            //else if (Endpoint == EndpointType.ORCLUM)
                            //{
                            //    var Entitlement = new ORCLUMDA().GetAccessRequestItem(searchString);
                            //    ReturnList.AddRange(Entitlement);
                            //}
                        }
                    }
                    return ReturnList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IsSecondartAccount(AdHocAccessRequest AdHocAccessRequest)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                if (AdHocAccessRequest.EndpointSystemType == EndpointType.ActiveDirectory)
                {
                    return new ActiveDirectoryDA().IsSecondartAccount(AdHocAccessRequest.RequesteeUserId);
                }
                else if (AdHocAccessRequest.EndpointSystemType == EndpointType.LotusNotes)
                {
                    return new LotusNotesDA().IsSecondartAccount(AdHocAccessRequest.RequesteeUserId);
                }
                else if (AdHocAccessRequest.EndpointSystemType == EndpointType.SAP)
                {
                    return new SAPConnectorDA().IsSecondartAccount(AdHocAccessRequest.RequesteeUserId);
                }
                return false;
            }
        }
    }
}
