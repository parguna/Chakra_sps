using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class EndPoint : DomainModelBase<EndPoint, EntityModel.EndPoint>
    {
        public string Description { get; set; }

        public Nullable<long> EndpointSystemTypeId { get; set; }

        public string DisplayName { get; set; }

        public long? OwnerUserId { get; set; }

        public string ApplicationCode { get; set; }

        public bool IsActive { get; set; }

        public bool IsTrustedSource { get; set; }

        public bool IsPasswordResetAllowed { get; set; }
        public bool IsUnlockAllowed { get; set; }



        public bool IncludeInAttestation { get; set; }
        public bool CanAutomate { get; set; }
        public bool IsFunctionLevel { get; set; }
        public string UserObjectName { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public User Owner { get; set; }

        public List<AttributeMappingColumns> ListAttributeMappingColumns { get; set; }

        public EndPointSystemType EndPointSystemType { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public bool SupportSecondaryAccount { get; set; }
        public bool IsSodApplicable { get; set; }
        public static bool DoesExists(long id, string name, long endpointSystemTypeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if ((endpointSystemTypeId == 0) || (endpointSystemTypeId == -1))
                {
                    return new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Any(ep => ep.DisplayName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                }

                if (id == 0)
                    return new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Any(ep => ep.DisplayName.Equals(name, StringComparison.CurrentCultureIgnoreCase) && ep.EndpointSystemTypeId == endpointSystemTypeId);
                else
                    return new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Any(ep => ep.DisplayName.Equals(name, StringComparison.CurrentCultureIgnoreCase) && ep.EndpointSystemTypeId == endpointSystemTypeId && ep.Id != id);
            }
        }

        public static List<EndPoint> GetByEndpointSystemTypeIdIncludingEndPointSystemType(long endpointSystemTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.EndpointSystemTypeId == endpointSystemTypeId && !ep.IsTrustedSource, ep => ep.EndPointSystemType).ToList());
            }
        }

        public static List<EndPoint> GetByEndpointSystemByIds(List<long> endpointSystemIds)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.IsActive && endpointSystemIds.Contains(ep.Id)).ToList());
            }
        }

        public static List<EndPoint> GetActiveNonTrustedSrcNonInhouseEndpoints(bool includeInhouse)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (includeInhouse)
                    return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.IsActive && !ep.IsTrustedSource, ep => ep.EndPointSystemType).ToList());
                else
                    return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.IsActive && !ep.IsTrustedSource && ep.EndpointSystemTypeId != 9, ep => ep.EndPointSystemType).ToList());
            }
        }

        public static List<EndPoint> GetActiveEndpoints()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.IsActive, ep => ep.EndPointSystemType).ToList());
            }
        }
        public static bool IsActiveEndpointsystem(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Any(ep => ep.Id == endpointSystemId && ep.IsActive);
            }
        }

        public static bool IsUserEndpointOwner(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Any(ep => ep.IsActive && ep.OwnerUserId == userId);
            }
        }

        public static List<EndPoint> GetByEndpointSystemTypeId(long endpointSystemTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                var item = ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.IsActive && ep.EndpointSystemTypeId == endpointSystemTypeId, true).ToList());

                return item;
            }
        }

        public static List<EndPoint> GetByOwnerId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                var item = ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.OwnerUserId == userId && ep.IsActive).ToList());

                return item;
            }
        }

        public static List<EndPoint> GetOrgEndpointByType(long endpointSystemTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                var item = ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.EndpointSystemTypeId == endpointSystemTypeId
                && ep.IsActive, true).ToList());

                return item.OrderBy(i => i.DisplayName).ToList();
            }
        }

        public static EndPoint GetByEndpointSystemTypeIdIsActive(long endpointSystemTypeId, bool isActive)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.EndpointSystemTypeId == endpointSystemTypeId && isActive).FirstOrDefault());
            }
        }

        public static EndPoint GetByEndpointTypeId(long endpointSystemTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.EndpointSystemTypeId == endpointSystemTypeId && !ep.IsTrustedSource && ep.IsActive).FirstOrDefault());
            }
        }

        public static List<EndPoint> GetAllByEndpointTypeId(long endpointSystemTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.EndpointSystemTypeId == endpointSystemTypeId && !ep.IsTrustedSource && ep.IsActive).ToList());
            }
        }

        public static List<EndPoint> GetAllIncludingEndPointSystemType()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).GetAll(ep => ep.EndPointSystemType, ep => ep.Owner).ToList());
            }
        }

        public static EndPoint GetByIdIncludingOwner(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).GetById(id, ep => ep.EndPointSystemType, ep => ep.Owner));
            }
        }

        public static EndPoint GetByIdIncludingEndPointSystemType(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).GetById(id, ep => ep.EndPointSystemType, ep => ep.Owner));
            }
        }

        public static EndPoint GetEndPointByApplicationType(ApplicationType applicationType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.ApplicationType == applicationType, ep => ep.EndPointSystemType, ep => ep.Owner).FirstOrDefault());
            }
        }

        public static EndPoint GetEndPointByApplicationTypeNonTrusted(ApplicationType applicationType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.ApplicationType == applicationType && ep.IsTrustedSource == false, ep => ep.EndPointSystemType, ep => ep.Owner).FirstOrDefault());
            }
        }


        public static EndPoint GetByIdIncludingEndPointAndITLeadSystemType(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).GetById(id, ep => ep.EndPointSystemType, ep => ep.Owner));
            }
        }

        public static EndPoint GetByApplicationCode(string applicationCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.ApplicationCode == applicationCode).FirstOrDefault());
            }
        }

        public static EndPoint GetByApplicationType(ApplicationType applicationType, bool isTrustedSource = false)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.ApplicationType == applicationType && ep.IsTrustedSource == isTrustedSource).FirstOrDefault());
            }
        }

        public static EndPoint GetByEndpointType(long endpointTypeId, bool isTrustedSource = false)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.EndpointSystemTypeId == endpointTypeId && ep.IsTrustedSource == isTrustedSource).FirstOrDefault());
            }

        }

        public static List<EndPoint> GetByApplicationTypes(List<ApplicationType> applicationTypes)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => applicationTypes.Contains(ep.ApplicationType)).ToList());
            }
        }

        public static EndPoint GetByApplicationCode(string applicationCode, long endpointSystemTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.ApplicationCode == applicationCode
                && ep.EndpointSystemTypeId == endpointSystemTypeId).FirstOrDefault());
            }
        }

        public static bool DoesExists(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Any(ep => ep.Id == id);
            }
        }

        public static int DoesExistsbyApplicationCode(string applicationCode, long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (id == 0)
                    return new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.ApplicationCode == applicationCode)
                    .Select(task => task.ApplicationCode).Count();
                else
                    return new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.ApplicationCode == applicationCode && ep.Id != id)
                       .Select(task => task.ApplicationCode).Count();
            }
        }

        public static Dictionary<long, string> GetInhouseEndpoints()
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                Dictionary<long, string> getEndPoints = new Dictionary<long, string>();

                getEndPoints = new EndPointRepository(dataAccessTransaction).GetInhouseEndPoints();

                return getEndPoints;
            }
        }

        public static Dictionary<string, string> GetInhouseEndPointsWithCode()
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetInhouseEndPointsWithCode();
            }
        }

        public static Dictionary<long, string> GetByTrustedSource(bool ignoreTrustedSource)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                Dictionary<long, string> getEndPoints = new Dictionary<long, string>();

                if (ignoreTrustedSource)
                    getEndPoints = new EndPointRepository(dataAccessTransaction).GetByTrustedSource();
                else
                    getEndPoints = new EndPointRepository(dataAccessTransaction).GetEndPoints();

                return getEndPoints;
            }
        }

        public static List<PresentationEndPointForFilter> GetRequestableEndpointsAsDict(bool IsAddEntitlement)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                Dictionary<long, string> endPoints = new Dictionary<long, string>();
                List<PresentationEndPointForFilter> requestableEndPoints = GetRequestableEndpoints(IsAddEntitlement);
                return requestableEndPoints;
            }
        }

        public static List<PresentationEndPointForFilter> GetRequestableEndpoints(bool IsAddEntitlement = true)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<EndPoint> eps = GetActiveNonTrustedSrcNonInhouseEndpoints(true);
                List<PresentationEndPointForFilter> resultEps = new List<PresentationEndPointForFilter>();
                if (eps != null && eps.Count > 0)
                {
                    foreach (EndPoint item in eps)
                    {
                        PresentationEndPointForFilter pEndpoint = new PresentationEndPointForFilter();
                        pEndpoint.EndpointSystemTypeId = item.EndpointSystemTypeId;
                        pEndpoint.Id = item.Id;
                        pEndpoint.DisplayName = item.DisplayName;
                        pEndpoint.Description = item.Description;
                        pEndpoint.HasEntitlementTypes = false;// (pEndpoint.EntitlementTypes.Count > 0);

                        if (item.EndpointSystemTypeId == Convert.ToInt64(EndpointType.ActiveDirectory))
                        {
                            if (new EndPointRepository(dataAccessTransaction).CheckRequestableADEndpoint(item.Id))
                                resultEps.Add(pEndpoint);
                        }
                        else
                            resultEps.Add(pEndpoint);
                    }
                }
                return resultEps;
            }
        }

        public static List<EndPoint> GetByEndpointConfiguration(string key, string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new EndPointRepository(dataAccessTransaction).GetByEndpointConfiguration(key, value));
            }
        }

        public static Dictionary<long, string> GetByEndpointSystemTypeIdTrustedSource(long endpointSystemTypeId, bool ignoreTrustedSource)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (ignoreTrustedSource)
                    return new EndPointRepository(dataAccessTransaction).GetByEndpointSystemTypeIdandTrustedSource(endpointSystemTypeId, ignoreTrustedSource);
                else
                    return new EndPointRepository(dataAccessTransaction).GetByEndpointSystemTypeId(endpointSystemTypeId);
            }
        }

        public static List<EndPoint> GetAttestionEnabled()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new EndPointRepository(dataAccessTransaction).GetAttestionEnabled());
            }
        }

        public static List<string> GetNonConnectedAccount(long diUserID, LockScenario lockScenario)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetNonConnectedAccount(diUserID, lockScenario);
            }
        }

        public static Dictionary<long, string> GetByEndpointSystemTypeIds(List<long> endpointSystemTypeIdList, string displayNameSearchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetByEndpointSystemTypeIds(endpointSystemTypeIdList, CheckNull.AsteriskToString(displayNameSearchString));
            }
        }

        public static Dictionary<long, string> GetAttestationByEndpointSystemTypeIds(List<long> endpointSystemTypeIdList, string displayNameSearchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetAttestationByEndpointSystemTypeIds(endpointSystemTypeIdList, CheckNull.AsteriskToString(displayNameSearchString));
            }
        }

        public static List<EndPoint> GetByEndpointSystemTypeId(long endPointSystemTypeId, string displayName)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                List<EndPoint> endPointList = new List<EndPoint>();

                if (!string.IsNullOrEmpty(displayName) && endPointSystemTypeId > -1)
                    endPointList = ORMapList(new RepositoryBase<EntityModel.EndPoint>(transaction).Get(ep => ep.EndpointSystemTypeId == endPointSystemTypeId && ep.DisplayName == displayName, ep => ep.EndPointSystemType, ep => ep.Owner).ToList());
                else
                {
                    if (endPointSystemTypeId > -1)
                        endPointList = ORMapList(new RepositoryBase<EntityModel.EndPoint>(transaction).Get(ep => ep.EndpointSystemTypeId == endPointSystemTypeId, ep => ep.EndPointSystemType, ep => ep.Owner).ToList());
                }
                return endPointList;
            }
        }

        public static List<EndPoint> GetNonTrustedSource(long endpointSystemTypeId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                List<EndPoint> endPointList = new List<EndPoint>();
                if (endpointSystemTypeId > -1)
                {
                    endPointList = ORMapList(new RepositoryBase<EntityModel.EndPoint>(transaction).Get(ep => ep.EndpointSystemTypeId == endpointSystemTypeId && !ep.IsTrustedSource, ep => ep.EndPointSystemType).ToList());
                }
                return endPointList;
            }
        }

        public static Dictionary<long, string> GetByDescriptionSearchString(string descriptionSearchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetByMenuName(CheckNull.AsteriskToString(descriptionSearchString));
            }
        }

        public static Dictionary<long, string> GetByEndpointDescSearchString(string descriptionSearchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetByEndpoint(CheckNull.AsteriskToString(descriptionSearchString));
            }
        }

        public static Dictionary<long, string> GetApplicationCodeById(long id, string applicationCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetApplicationCodeById(id, CheckNull.AsteriskToString(applicationCode));
            }
        }

        public static Dictionary<long, string> GetApplicableApplications(string displayNameSearchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetApplicableApplications(CheckNull.AsteriskToString(displayNameSearchString));
            }
        }

        public static Dictionary<long, string> GetSingleEntitlements(string searchString, long endpointSystemId, long? endpointUserId, bool existingEntitlements)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                EndPoint ep = GetByIdIncludingOwner(endpointSystemId);
                return EndPointFactory.GetEndPoint(ep.EndPointSystemType.EndpointSystemTypeName).GetEntitlements(searchString, endpointSystemId, endpointUserId, existingEntitlements);
                //if (ep.EndPointSystemType.EndpointSystemTypeName == EndpointType.ActiveDirectory)
                //{
                //    IList<ADSGroupInfo> list = null;

                //    if (existingEntitlements)
                //    {
                //        list = ADSGroupInfo.GetADGroupsByUserId(EndpointUserId.Value);
                //        result = list.Where(item => item.sAMAccountName.Contains(searchString))
                //        .Select(x => new { Key = x.Id, Value = x.name })
                //        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                //    }
                //    else
                //    {
                //        result = ADSGroupInfo.GetAllByName(searchString);
                //    }
                //}
                //if (ep.EndPointSystemType.EndpointSystemTypeName == EndpointType.MSSQL)
                //{
                //    IList<MSSQLUMRoleInfo> list = null;

                //    if (existingEntitlements)
                //    {
                //        list = MSSQLUMRoleInfo.GetmssqlroleByUserId(EndpointUserId.Value);
                //        result = list.Where(item => item.RoleName.Contains(searchString))
                //        .Select(x => new { Key = x.Id, Value = x.RoleName })
                //        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                //    }
                //    else
                //    {
                //        result = MSSQLUMRoleInfo.GetAllByName(searchString);
                //    }
                //}
                //if (ep.EndPointSystemType.EndpointSystemTypeName == EndpointType.LotusNotes)
                //{
                //    IList<LotusGroupInfo> list = null;
                //    if (existingEntitlements)
                //        list = LotusGroupInfo.GetByDIUserId(EndpointUserId.Value, EndpointSystemId);
                //    else
                //        list = LotusGroupInfo.GetAll();

                //    result = list.Where(item => item.Group.Contains(searchString))
                //        .Select(x => new { Key = x.Id, Value = x.Group })
                //        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                //}
                //if (ep.EndPointSystemType.EndpointSystemTypeName == EndpointType.Linux)
                //{
                //    IList<LinuxGroupInfo> list = null;
                //    list = LinuxGroupInfo.GetListByGroupNameEndPointSystemId(searchString, EndpointSystemId);
                //    result = list.Where(item => item.GroupName.Contains(searchString))
                //       .Select(x => new { Key = x.Id, Value = x.GroupName })
                //       .Distinct().ToDictionary(o => o.Key, o => o.Value);
                //}
            }

            //return result;
        }


        //public static Dictionary<long, string> GetRoleLevelOwnerByEndPoint(long endpointSystemId, bool IsLevel1, bool IsGroup )
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        EndPoint ep = GetByIdIncludingOwner(endpointSystemId);
        //        return EndPointFactory.GetEndPoint(ep.EndPointSystemType.EndpointSystemTypeName).GetEntitlements(searchString, endpointSystemId, endpointUserId, existingEntitlements);
        //        //if (ep.EndPointSystemType.EndpointSystemTypeName == EndpointType.ActiveDirectory)
        //        //{
        //        //    IList<ADSGroupInfo> list = null;

        //        //    if (existingEntitlements)
        //        //    {
        //        //        list = ADSGroupInfo.GetADGroupsByUserId(EndpointUserId.Value);
        //        //        result = list.Where(item => item.sAMAccountName.Contains(searchString))
        //        //        .Select(x => new { Key = x.Id, Value = x.name })
        //        //        .Distinct().ToDictionary(o => o.Key, o => o.Value);
        //        //    }
        //        //    else
        //        //    {
        //        //        result = ADSGroupInfo.GetAllByName(searchString);
        //        //    }
        //        //}
        //        //if (ep.EndPointSystemType.EndpointSystemTypeName == EndpointType.MSSQL)
        //        //{
        //        //    IList<MSSQLUMRoleInfo> list = null;

        //        //    if (existingEntitlements)
        //        //    {
        //        //        list = MSSQLUMRoleInfo.GetmssqlroleByUserId(EndpointUserId.Value);
        //        //        result = list.Where(item => item.RoleName.Contains(searchString))
        //        //        .Select(x => new { Key = x.Id, Value = x.RoleName })
        //        //        .Distinct().ToDictionary(o => o.Key, o => o.Value);
        //        //    }
        //        //    else
        //        //    {
        //        //        result = MSSQLUMRoleInfo.GetAllByName(searchString);
        //        //    }
        //        //}
        //        //if (ep.EndPointSystemType.EndpointSystemTypeName == EndpointType.LotusNotes)
        //        //{
        //        //    IList<LotusGroupInfo> list = null;
        //        //    if (existingEntitlements)
        //        //        list = LotusGroupInfo.GetByDIUserId(EndpointUserId.Value, EndpointSystemId);
        //        //    else
        //        //        list = LotusGroupInfo.GetAll();

        //        //    result = list.Where(item => item.Group.Contains(searchString))
        //        //        .Select(x => new { Key = x.Id, Value = x.Group })
        //        //        .Distinct().ToDictionary(o => o.Key, o => o.Value);
        //        //}
        //        //if (ep.EndPointSystemType.EndpointSystemTypeName == EndpointType.Linux)
        //        //{
        //        //    IList<LinuxGroupInfo> list = null;
        //        //    list = LinuxGroupInfo.GetListByGroupNameEndPointSystemId(searchString, EndpointSystemId);
        //        //    result = list.Where(item => item.GroupName.Contains(searchString))
        //        //       .Select(x => new { Key = x.Id, Value = x.GroupName })
        //        //       .Distinct().ToDictionary(o => o.Key, o => o.Value);
        //        //}
        //    }

        //    //return result;
        //}

        public static List<EndPoint> GetByEndpointType(EndpointType endpointType, string hostName)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                long endPointSystemTypeId = new RepositoryBase<EntityModel.EndPointSystemType>(transaction).Get(eps => eps.EndpointSystemTypeName == endpointType).FirstOrDefault().Id;

                List<EndPoint> endPointList = new List<EndPoint>();

                if (!string.IsNullOrEmpty(hostName) && endPointSystemTypeId > -1)
                    endPointList = ORMapList(new RepositoryBase<EntityModel.EndPoint>(transaction).Get(ep => ep.EndpointSystemTypeId == endPointSystemTypeId && ep.DisplayName == hostName).ToList());

                else
                {
                    if (endPointSystemTypeId > -1)
                        endPointList = ORMapList(new RepositoryBase<EntityModel.EndPoint>(transaction).Get(ep => ep.EndpointSystemTypeId == endPointSystemTypeId).ToList());
                }
                return endPointList;
            }
        }

        public static bool IsValid(string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Any(ep => ep.ApplicationCode == value);
            }
        }

        public static List<EndPoint> GetByEndpointTypeIncludingOwner(long endPointSystemTypeId, string hostName)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                List<EndPoint> list = new List<EndPoint>();

                if (!string.IsNullOrEmpty(hostName) && endPointSystemTypeId > -1)
                    list = ORMapList(new RepositoryBase<EntityModel.EndPoint>(transaction).Get(ep => ep.EndpointSystemTypeId == endPointSystemTypeId && ep.DisplayName == hostName, ep => ep.EndPointSystemType, ep => ep.Owner).ToList());
                else
                {
                    if (endPointSystemTypeId > -1)
                        list = ORMapList(new RepositoryBase<EntityModel.EndPoint>(transaction).Get(ep => ep.EndpointSystemTypeId == endPointSystemTypeId, ep => ep.EndPointSystemType, ep => ep.Owner).ToList());
                }
                return list;
            }
        }

        public static EndPoint GetByDisplayName(string displayName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(transaction).Get(ep => ep.DisplayName == displayName).FirstOrDefault());
            }
        }

        public static EndPoint GetByDisplayNameIncludingEndPointSystemType(string displayName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.DisplayName == displayName, ep => ep.EndPointSystemType).FirstOrDefault());
            }
        }

        public static EndPoint GetTrustedSource()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.IsTrustedSource && ep.IsActive, ep => ep.EndPointSystemType).FirstOrDefault());
            }
        }

        public static List<EndPoint> GetAllTrustedSource()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => ep.IsTrustedSource, ep => ep.EndPointSystemType).ToList());
            }
        }

        public new static bool Delete(long id)
        {
            try
            {
                using (DataAccessTransaction transaction = new DataAccessTransaction())
                {
                    var EndpointConfig = DomainModel.EndpointConfiguration.GetByEndpointSystemId(id);
                    if (EndpointConfig == null)
                        return false;

                    EndpointConfig.ConfigurationDetails.ForEach(lq =>
                        {
                            lq.Delete(transaction);
                        });
                    EndpointConfig.Delete(transaction);
                    EndpointConfig.EndpointSystem.ListAttributeMappingColumns.ForEach(lq =>
                        {
                            lq.Delete(transaction);
                        });
                    EndpointConfig.EndpointSystem.Delete(transaction);
                    transaction.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public void UnMapEndpointUsers(string userName)
        //{
        //    using (var transaction = new DataAccessTransaction())
        //    {
        //        EndpointType endpointTypeName = EndPointSystemType.GetEndPointTypeName(this.EndpointSystemTypeId.Value);

        //        if (endpointTypeName == EndpointType.ActiveDirectory)
        //        {
        //            ADSUserInfo aDSUserInfo = ADSUserInfo.GetById(this.Id);
        //            aDSUserInfo.LastModifiedBy = userName;
        //            aDSUserInfo.LastModifiedDate = DateTime.Now.Date;
        //            aDSUserInfo.EndPointSystemId = this.Id;
        //            aDSUserInfo.Save(transaction);
        //        }
        //        else if (endpointTypeName == EndpointType.SAP)
        //        {
        //            SAPUserinfo sapUserInfo = SAPUserinfo.GetById(this.Id);
        //            sapUserInfo.LastModifiedBy = userName;
        //            sapUserInfo.LastModifiedDate = DateTime.Now.Date;
        //            sapUserInfo.EndPointSystemId = this.Id;
        //            sapUserInfo.Save(transaction);
        //        }
        //        else if (endpointTypeName == EndpointType.SAPEnterprisePortal)
        //        {
        //            SAPEPUserInfo sapepUserInfo = SAPEPUserInfo.GetByEndPointSystemId(this.Id).FirstOrDefault();
        //            sapepUserInfo.LastModifiedBy = userName;
        //            sapepUserInfo.LastModifiedDate = DateTime.Now.Date;
        //            sapepUserInfo.EndPointSystemId = this.Id;
        //            sapepUserInfo.Save(transaction);
        //        }
        //        else if (endpointTypeName == EndpointType.LotusNotes)
        //        {
        //            LotusUserInfo lotusUserInfo = LotusUserInfo.GetByEndPointSystemId(this.Id);
        //            lotusUserInfo.LastModifiedBy = userName;
        //            lotusUserInfo.LastModifiedDate = DateTime.Now.Date;
        //            lotusUserInfo.EndPointSystemId = this.Id;
        //            lotusUserInfo.Save(transaction);
        //        }

        //        if (endpointTypeName == EndpointType.MSSQL)
        //        {
        //            MSSQLUMUserInfo aDSUserInfo = MSSQLUMUserInfo.GetById(this.Id);
        //            aDSUserInfo.LastModifiedBy = userName;
        //            aDSUserInfo.LastModifiedDate = DateTime.Now.Date;
        //            aDSUserInfo.EndPointSystemId = this.Id;
        //            aDSUserInfo.Save(transaction);
        //        }

        //        else if (endpointTypeName == EndpointType.Exchange)
        //        {
        //            ExUserInfo ExUserInfo = ExUserInfo.GetByEndPointSystemId(this.Id);
        //            ExUserInfo.LastModifiedBy = userName;
        //            ExUserInfo.LastModifiedDate = DateTime.Now.Date;
        //            ExUserInfo.EndPointSystemId = this.Id;
        //            ExUserInfo.Save(transaction);
        //        }

        //        transaction.Commit();
        //    }
        //}

        public static void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            using (var transaction = new DataAccessTransaction())
            {
                long _endPointSystemTypeId = EndPoint.GetById(endpointId).EndpointSystemTypeId.Value;
                EndpointType _endPointName = EndPointSystemType.GetById(_endPointSystemTypeId).EndpointSystemTypeName;
                EndPointFactory.GetEndPoint(_endPointName).UpdatePasswordresetcount(endpointId, userName, currentUser);
                /*
                if (_endPointName == EndpointType.ActiveDirectory)
                    {
                        ADSUserInfo adsUserInfo = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(userName, endpointId);
                        if (adsUserInfo != null)
                        {
                            if (adsUserInfo.PwdResetCnt == null)
                                adsUserInfo.PwdResetCnt = 1;
                            else
                                adsUserInfo.PwdResetCnt++;
                            adsUserInfo.Save(currentUser);
                        }
                    }
                    else if (_endPointName == EndpointType.SAP)
                    {
                        SAPUserinfo sapUserInfo = SAPUserinfo.GetByUserLogonName(userName, endpointId);
                        if (sapUserInfo != null)
                        {
                            if (sapUserInfo.PwdResetCnt == null)
                                sapUserInfo.PwdResetCnt = 1;
                            else
                                sapUserInfo.PwdResetCnt++;
                            sapUserInfo.Save(currentUser);
                        }
                    }
                    else if (_endPointName == EndpointType.LotusNotes)
                    {
                    LotusUserInfo lnsUserInfo = LotusUserInfo.GetBysAMAccountNameEndPointSystemId(userName, endpointId);
                        if (lnsUserInfo != null)
                        {
                            if (lnsUserInfo.PwdResetCnt == null)
                                lnsUserInfo.PwdResetCnt = 1;
                            else
                                lnsUserInfo.PwdResetCnt++;
                            lnsUserInfo.Save(currentUser);
                        }
                    }
                    else if (_endPointName == EndpointType.SAPEnterprisePortal)
                    {
                        SAPEPUserInfo sapEPUserInfo = SAPEPUserInfo.GetBysAMAccountNameEndPointSystemId(userName, endpointId);
                        if (sapEPUserInfo != null)
                        {
                            if (sapEPUserInfo.PwdResetCnt == null)
                                sapEPUserInfo.PwdResetCnt = 1;
                            else
                                sapEPUserInfo.PwdResetCnt++;
                            sapEPUserInfo.Save(currentUser);
                        }
                    }
                transaction.Commit();    
            */
            }
        }


        #region IEndPointConfiguration
        //Switch case revamp finished
        public static EndpointConfiguration GetConfiguration(EndPoint endpoint)
        {
            return EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            /*
            using (var transaction = new DataAccessTransaction())
            {
                if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.ActiveDirectory)
                {
                    //endpointConfig = ADSConfiguration.GetByEndpointSystemID(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.SAP)
                {
                   // endpointConfig = SAPConfiguration.GetEndpointConfiguration(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.SAPEnterprisePortal)
                {
                    //endpointConfig = SAPEPConfiguration.GetEndpointConfiguration(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.LotusNotes)
                {
                    //endpointConfig = LNSConfiguration.GetEndpointConfiguration(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.InhouseSystems)
                {
                    //endpointConfig = InhouseSystemConfiguration.GetByEndPointSystemType(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.OracleEBS)
                {
                    //endpointConfig = OracleEBSConfiguration.GetEndpointConfiguration(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.Linux)
                {
                    //endpointConfig = LinuxConfiguration.GetEndpointConfiguration(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.ORCLUM)
                {
                    //endpointConfig = OracleUMConfiguration.GetEndpointConfiguration(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.CustomApplication)
                {
                    endpointConfig = SqlConfiguration.GetEndpointConfiguration(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.CopHRMS)
                {
                    endpointConfig = COPConfiguration.GetByEndpointSystemID(endpointSystemId);
                }                
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.Exchange)
                {
                    //endpointConfig = ExConfiguration.GetByEndpointSystemID(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.Peoplesoft)
                {
                    //endpointConfig = PeopleSoftConfiguration.GetByEndPointSystemId(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.Windows)
                {
                    //endpointConfig = WindowsConfiguration.GetByEndpointSystemID(endpointSystemId);
                }

                else if (endpoint.EndPointSystemType.EndpointSystemTypeName== EndpointType.MSSQL)
                {
                    //endpointConfig = MSSQLUMConfiguration.GetByEndpointSystemID(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.FileHRMS)
                {
                    endpointConfig = FileHRMSConfiguration.GetByEndpointSystemID(endpointSystemId);
                }
                else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.CMS)
                {
                    endpointConfig = CMSConfiguration.GetByEndpointSystemID(endpointSystemId);
                }
                return endpointConfig;
            }
            */
        }

        public static void SaveConfiguration(EndpointConfiguration endPointConfig, string currentUser)
        {
            bool isNew = endPointConfig.EndpointSystem.Id <= 0;
            using (DataAccessTransaction dataAccessTrasaction = new DataAccessTransaction(currentUser))
            {
                if (!isNew)
                {
                    using (DataAccessTransaction transactionChild = new DataAccessTransaction())
                    {
                        var existingEP = EndPoint.GetById(endPointConfig.EndpointSystemId.Value);
                        List<AttributeMappingColumns> dbAttributeMappingColumnsList = AttributeMappingColumns.GetByEndpointSystemId(Convert.ToInt32(endPointConfig.EndpointSystemId));
                        dbAttributeMappingColumnsList.ForEach(lq =>
                            {
                                lq.Delete();
                            });
                        List<EndpointConfigurationDetail> dbEndpointConfigurationDetailList = EndpointConfigurationDetail.GetByEndpointConfigurationId(endPointConfig.Id);
                        dbEndpointConfigurationDetailList.ForEach(lq =>
                        {
                            lq.Delete();
                        });
                        if (endPointConfig.EndpointSystem.ListAttributeMappingColumns != null)
                        {
                            foreach (var listAttributeMappings in endPointConfig.EndpointSystem.ListAttributeMappingColumns)
                            {
                                listAttributeMappings.Id = 0;
                            }
                            AttributeMappingColumns.SaveList(dataAccessTrasaction, endPointConfig.EndpointSystem.ListAttributeMappingColumns.ToList());
                        }
                        endPointConfig.EndpointSystem.Save(dataAccessTrasaction);
                        endPointConfig.Save(dataAccessTrasaction);
                        endPointConfig.ConfigurationDetails.ForEach(lq =>
                        {
                            lq.EndpointConfigurationId = endPointConfig.Id;
                            lq.Save(dataAccessTrasaction);
                        });

                        transactionChild.Commit();
                        dataAccessTrasaction.Commit();
                    }
                }
                else
                {
                    endPointConfig.Save(dataAccessTrasaction);
                    dataAccessTrasaction.Commit();
                }
            }
        }
        #endregion

        //public static List<PresentationEndpointInformation> GetInfromationByDIUserid(long diUserId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return new EndPointRepository(dataAccessTransaction).GetInfromationByDIUserid(diUserId);
        //    }
        //}

        public static Dictionary<long, string> GetPasswordPolicyApplications(string displayNameSearchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetPasswordPolicyApplications(CheckNull.AsteriskToString(displayNameSearchString));
            }
        }
        public static EndPoint GetTrustedSourceEndPoint()
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(item => item.IsTrustedSource && item.IsActive, ep => ep.EndPointSystemType).FirstOrDefault());
            }
        }

        public static List<EndPoint> GetByApplicationCodes(List<string> applicationCodes)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EndPoint>(dataAccessTransaction).Get(ep => applicationCodes.Contains(ep.ApplicationCode)).ToList());
            }
        }
        public static List<EndPoint> GetDefaultApplication()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new EndPointRepository(dataAccessTransaction).GetDefaultApplication());
            }
        }

        public static List<EndPoint> GetActiveOtherSystemAutomatedEndpoints()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new EndPointRepository(dataAccessTransaction).GetActiveOtherSystemAutomatedEndpoints());
            }
        }

        public static Dictionary<long, string> GetInhouseEndPointsAutomateSystem()
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EndPointRepository(dataAccessTransaction).GetInhouseEndPointsAutomateSystem();
            }
        }
        
    }
    public class PresentationOtherSystemUser
    {
        public PresentationOtherSystemUser(long accountReconcilationId, string userName, string permission, long endpointUserId, string displayName, AccountType accountType)
        {
            AccountReconcilationId = accountReconcilationId;
            UserName = userName;
            Permission = permission;
            EndpointUserId = endpointUserId;
            DisplayName = displayName;
            AccountType = accountType;
        }
        public long AccountReconcilationId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Permission { get; set; }
        public long EndpointUserId { get; set; }
        public AccountType AccountType { get; set; }
    }
    public class PresentationOtherSystemData
    {
        public PresentationOtherSystemData(long permissionId, string permission, string fullName, AccountType accountType, MappingType mappingType, string entitlement, string employeeId,Nullable<long> userId,string accountName, string firstName, string lastName, string description)
        {
            this.PermissionId = permissionId;
            this.Permission = permission;
            this.FullName = fullName;
            this.AccountType = accountType;
            this.MappingType = mappingType;
            this.Entitlement = entitlement;
            this.EmployeeId = employeeId;
            this.UserId = userId;
            this.AccountName = accountName;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Description = description;
        }
        public long PermissionId { get; set; }
        public string Permission { get; set; }
        public string FullName { get; set; }
        public string AccountName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AccountType AccountType { get; set; }
        public MappingType MappingType { get; set; }
        public string Entitlement { get; set; }
        public string EmployeeId { get; set; }
        public long? UserId { get; set; }
        public string Description { get; set; }
    }
}
