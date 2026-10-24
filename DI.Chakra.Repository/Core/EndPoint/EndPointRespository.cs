using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel.Presentation;

namespace DI.Chakra.Repository
{
    public class EndPointRepository : RepositoryBase<EndPoint>
    {
        public EndPointRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public Dictionary<long, string> GetByTrustedSource()
        {
            return (from EPS in CurrentContext.EndPoint
                    where EPS.IsTrustedSource == false && !string.IsNullOrEmpty(EPS.DisplayName) && EPS.IsActive 
                    select new { EPS.DisplayName, EPS.Id })
                        .Distinct().OrderBy(c => c.DisplayName)
                        .ToDictionary(c => c.Id, c => c.DisplayName);
        }

        //public Dictionary<long, string> GetRequestableEndpoints()
        //{
        //    return (from EPS in CurrentContext.EndPoint
        //            join ECD in CurrentContext.EndpointConfigurationDetails
        //            on EPS.Id equals ECD.EndpointConfigurationId
        //            join ECK in CurrentContext.EndpointConfigurationKeys
        //            on ECD.KeyId equals ECK.Id
        //            where EPS.IsTrustedSource == false && ECK.KeyName=="IsWOG" && 
        //                  ECD.Value!=Boolean.TrueString && 
        //                  !string.IsNullOrEmpty(EPS.DisplayName) && 
        //                  EPS.IsActive
        //            select new { EPS.DisplayName, EPS.Id })
        //                .Distinct().OrderBy(c => c.DisplayName)
        //                .ToDictionary(c => c.Id, c => c.DisplayName);
        //}

        public bool CheckRequestableADEndpoint(long endpointId)
        {
            EndpointConfiguration ec = (from EC in CurrentContext.EndpointConfigurations
                    join ECD in CurrentContext.EndpointConfigurationDetails
                    on EC.Id equals ECD.EndpointConfigurationId
                    join ECK in CurrentContext.EndpointConfigurationKeys
                    on ECD.KeyId equals ECK.Id
                    where EC.EndpointSystemId == endpointId //&& ECK.KeyName == "IsWOG" && ECD.Value != Boolean.TrueString
                    select EC).FirstOrDefault();
            bool result = ec != null ? true : false;
            return result;
        }

        public List<EndPoint> GetByEndpointConfiguration(string key, string value)
        {
            return (from EPS in CurrentContext.EndPoint
                    join EC in CurrentContext.EndpointConfigurations
                    on EPS.Id equals EC.EndpointSystemId
                    join ECD in CurrentContext.EndpointConfigurationDetails
                    on EC.Id equals ECD.EndpointConfigurationId
                    join ECK in CurrentContext.EndpointConfigurationKeys
                    on ECD.KeyId equals ECK.Id
                    where ECK.KeyName == key && ECD.Value == value && EPS.IsActive
                    select EPS).ToList();
        }

        public Dictionary<long, string> GetEndPoints()
        {
            return (from EPS in CurrentContext.EndPoint
                    where !string.IsNullOrEmpty(EPS.DisplayName) && EPS.IsActive
                    select new { EPS.DisplayName, EPS.Id })
                        .Distinct().OrderBy(c => c.DisplayName)
                        .ToDictionary(c => c.Id, c => c.DisplayName);
        }

        public Dictionary<long, string> GetInhouseEndPoints()
        {
            return (from EPS in CurrentContext.EndPoint.Include("EndPointSystemType")
                    where !string.IsNullOrEmpty(EPS.DisplayName) && EPS.IsActive
                    && EPS.EndPointSystemType.EndpointSystemTypeName == EndpointType.InhouseSystems
                    select new { EPS.DisplayName, EPS.Id })
                        .Distinct().OrderBy(c => c.DisplayName)
                        .ToDictionary(c => c.Id, c => c.DisplayName);
        }

        public Dictionary<string, string> GetInhouseEndPointsWithCode()
        {
            return (from EPS in CurrentContext.EndPoint.Include("EndPointSystemType")
                    where !string.IsNullOrEmpty(EPS.DisplayName) && !string.IsNullOrEmpty(EPS.ApplicationCode) && EPS.IsActive
                    && EPS.EndPointSystemType.EndpointSystemTypeName == EndpointType.InhouseSystems
                    select new { EPS.DisplayName, EPS.ApplicationCode })
                        .Distinct().OrderBy(c => c.DisplayName)
                        .ToDictionary(c => c.ApplicationCode, c => c.DisplayName);
        }

        public Dictionary<long, string> GetByEndpointSystemTypeIdandTrustedSource(long endpointSystemTypeId, bool ignoreTrustedSource)
        {
            return (from EPS in CurrentContext.EndPoint
                    where !EPS.IsTrustedSource && EPS.EndpointSystemTypeId == endpointSystemTypeId
                    select new { key= EPS.Id ,value= EPS.Description})
                        .Distinct().OrderBy(c => c.value)
                        .ToDictionary(c => c.key, c => c.value);
        }

        public Dictionary<long, string> GetByEndpointSystemTypeId(long endpointSystemTypeId)
        {
            return (from EPS in CurrentContext.EndPoint
                    where EPS.EndpointSystemTypeId == endpointSystemTypeId
                    select new { EPS.Description, EPS.Id })
                                    .Distinct().OrderBy(c => c.Description)
                                    .ToDictionary(c => c.Id, c => c.Description);
        }
        public List<EndPoint> GetAttestionEnabled()
        {
            return (from EPS in CurrentContext.EndPoint
                    join EPST in CurrentContext.EndpointSystemTypes
                       on EPS.EndpointSystemTypeId equals EPST.Id
                    where EPS.IncludeInAttestation
                    select EPS).ToList();
        }

        public Dictionary<long, string> GetByMenuName(string descriptionSearchString)
        {
            return (from EP in CurrentContext.EndPoint
                    where EP.Description.Contains(descriptionSearchString)
                    select new {EP.Id, EP.Description})
                    .Distinct().OrderBy(s=>s.Description)
                         .ToDictionary(c => c.Id, c => c.Description);
        }


        public Dictionary<long, string> GetByEndpoint(string descriptionSearchString)
        {
            return (from EP in CurrentContext.EndPoint
                    where EP.Description.Contains(descriptionSearchString) && !EP.IsTrustedSource
                    select new { EP.Id, EP.DisplayName })
                    .Distinct().OrderBy(s => s.DisplayName)
                         .ToDictionary(c => c.Id, c => c.DisplayName);
        }

        public Dictionary<long, string> GetApplicableApplications(string displayNameSearchString)
        {
            return (from EP in CurrentContext.EndPoint
                    where EP.DisplayName.Contains(displayNameSearchString)
                    select new { EP.DisplayName, EP.Id })
                    .Distinct().ToDictionary(c => c.Id, c => c.DisplayName);
        }

        public List<string> GetNonConnectedAccount(long diUserID, LockScenario lockScenario)
        {
            var iNHquery = (from endpointsys in CurrentContext.EndPoint
                            join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
                            join usr in CurrentContext.InhouseUserInfos on endpointsys.Id equals usr.EndPointSystemId
                            //where usr.DIUserId == diUserID && usr.AccountType == AccountType.User && usr.LockScenario == lockScenario
                            select new
                            {
                                SystemName = endpointsys.DisplayName,
                                UserLogin = usr.UserName
                            }).ToList();
            return iNHquery.Select(item => item.SystemName + "," + item.UserLogin).Distinct().ToList();
        }


        public Dictionary<long, string> GetPasswordPolicyApplications(string displayNameSearchString)
        {
            return (from EP in CurrentContext.EndPoint
                    where EP.DisplayName.Contains(displayNameSearchString) && EP.EndPointSystemType.EndpointSystemTypeName != EndpointType.InhouseSystems
                    select new { EP.DisplayName, EP.Id })
                    .Distinct().ToDictionary(c => c.Id, c => c.DisplayName);
        }

        public Dictionary<long, string> GetApplicationCodeById(long id,string applicationCode)
        {
            return (from EP in CurrentContext.EndPoint
                    where EP.ApplicationCode.Contains(applicationCode) && EP.Id ==id 
                    select new { EP.Id ,EP.ApplicationCode})
                    .Distinct().ToDictionary(c => c.Id, c => c.ApplicationCode);
        }


        public Dictionary<long, string> GetByEndpointSystemTypeIds(List<long> endpointSystemTypeIdList, string displayNameSearchString)
        {
            return (from EP in CurrentContext.EndPoint
                    where EP.DisplayName.Contains(displayNameSearchString) && endpointSystemTypeIdList.Contains(EP.EndPointSystemType.Id)
                    select new { EP.DisplayName, EP.Id }).Distinct().OrderBy(c => c.DisplayName)
                         .ToDictionary(c => c.Id, c => c.DisplayName);
        }

        public Dictionary<long, string> GetAttestationByEndpointSystemTypeIds(List<long> endpointSystemTypeIdList, string displayNameSearchString)
        {
            return (from EP in CurrentContext.EndPoint
                    where EP.DisplayName.Contains(displayNameSearchString) && endpointSystemTypeIdList.Contains((int)EP.EndPointSystemType.EndpointSystemTypeName) && EP.IncludeInAttestation && EP.IsActive
                    select new { EP.DisplayName, EP.Id }).Distinct().OrderBy(c => c.DisplayName)
                         .ToDictionary(c => c.Id, c => c.DisplayName);
        }

    

        //public List<PresentationEndPoint> GetADSEndpointByDiUserid(long diUserId)
        //{
        //    return (from endpointsys in CurrentContext.EndPoint
        //            join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //            join usr in CurrentContext.ADSUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //            where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked && !usr.IsAccountDisabled

        //            select new PresentationEndPoint
        //            {
        //                Endpointsystemid = endpointsys.Id,
        //                Endpointsystemdescription = endpointsys.Description,
        //                EndpointSystemTypeId = endpointsys.EndpointSystemTypeId,
        //                HostName = endpointsys.DisplayName,
        //                EndpointsystemType = endpointsystype.EndpointSystemTypeName.ToString(),
        //                UserName = usr.sAMAccountName
        //            }).ToList();
        //}

        //public List<PresentationEndPoint> GetSAPEndpointByDiUserid(long diUserId)
        //{
        //    return (from endpointsys in CurrentContext.EndPoint
        //            join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //            join usr in CurrentContext.SAPUserinfos on endpointsys.Id equals usr.EndPointSystemId
        //            where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
        //            select new PresentationEndPoint
        //            {
        //                Endpointsystemid = endpointsys.Id,
        //                Endpointsystemdescription = endpointsys.Description,
        //                EndpointSystemTypeId = endpointsys.EndpointSystemTypeId,
        //                HostName = endpointsys.DisplayName,
        //                EndpointsystemType = endpointsystype.EndpointSystemTypeName.ToString(),
        //                UserName = usr.UserLogonName
        //            }).ToList();
        //}

        //public List<PresentationEndPoint> GetLotusEndpointByDiUserid(long diUserId)
        //{
        //    return (from endpointsys in CurrentContext.EndPoint
        //            join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //            join usr in CurrentContext.LotusUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //            where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
        //            select new PresentationEndPoint
        //            {
        //                Endpointsystemid = endpointsys.Id,
        //                Endpointsystemdescription = endpointsys.Description,
        //                EndpointSystemTypeId = endpointsys.EndpointSystemTypeId,
        //                HostName = endpointsys.DisplayName,
        //                EndpointsystemType = endpointsystype.EndpointSystemTypeName.ToString(),
        //                UserName = usr.ShortName
        //            }).ToList();
        //}

        //public List<PresentationEndPoint> GetSAPEPEndpointByDiUserid(long diUserId)
        //{
        //    return (from endpointsys in CurrentContext.EndPoint
        //            join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //            join usr in CurrentContext.SAPEPUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //            where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
        //            select new PresentationEndPoint
        //            {
        //                Endpointsystemid = endpointsys.Id,
        //                Endpointsystemdescription = endpointsys.Description,
        //                EndpointSystemTypeId = endpointsys.EndpointSystemTypeId,
        //                HostName = endpointsys.DisplayName,
        //                EndpointsystemType = endpointsystype.EndpointSystemTypeName.ToString(),
        //                UserName = usr.UserName
        //            }).ToList();
        //}

        //public List<PresentationEndPoint> GetINHEndpointByDiUserid(long diUserId)
        //{
        //    return (from endpointsys in CurrentContext.EndPoint
        //            join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //            join usr in CurrentContext.InhouseUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //            where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
        //            select new PresentationEndPoint
        //            {
        //                Endpointsystemid = endpointsys.Id,
        //                Endpointsystemdescription = endpointsys.Description,
        //                EndpointSystemTypeId = endpointsys.EndpointSystemTypeId,
        //                HostName = endpointsys.DisplayName,
        //                EndpointsystemType = endpointsystype.EndpointSystemTypeName.ToString(),
        //                UserName = usr.UserName
        //            }).ToList();
        //}

        //public List<PresentationEndPoint> GetLinuxEndpointByDiUserid(long diUserId)
        //{
        //    return (from endpointsys in CurrentContext.EndPoint
        //            join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //            join usr in CurrentContext.LinuxUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //            where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked

        //            select new PresentationEndPoint
        //            {
        //                Endpointsystemid = endpointsys.Id,
        //                Endpointsystemdescription = endpointsys.Description,
        //                EndpointSystemTypeId = endpointsys.EndpointSystemTypeId,
        //                HostName = endpointsys.DisplayName,
        //                EndpointsystemType = endpointsystype.EndpointSystemTypeName.ToString(),
        //                UserName = usr.UserName
        //            }).ToList();
        //}

        //public List<PresentationExceptionReconData> GetADSExceptionReconByendPointSystemId(long endPointSystemId)
        //{
        //    return (from ad in CurrentContext.ADSUserInfos

        //            where ad.Version == 2
        //            select new PresentationExceptionReconData
        //            {
        //                EndpointSystemType = ad.EndPointSystem.EndPointSystemType.DisplayName,
        //                EndpointSystemName = ad.EndPointSystem.DisplayName,
        //                AccountName = ad.sAMAccountName,
        //                DisplayName = ad.displayName,
        //                CreatedDate = ad.whenCreated.Value,
        //                LastModifiedDate = null,
        //                ExceptionType = "Created",
        //                EndpointSystemId = ad.EndPointSystem.Id,
        //                EndpointUserId = ad.Id,
        //                UserId = ad.DIUserId,
        //                Version = ad.Version

        //            }).Union(from adHistory in CurrentContext.ADSUserInfoHistorys
        //                     from ep in CurrentContext.EndPoint
        //                     where adHistory.EndPointSystemId == ep.Id
        //                     from eptype in CurrentContext.EndpointSystemTypes
        //                     where ep.EndpointSystemTypeId == eptype.Id
        //                     where adHistory.Version == 2
        //                     select new PresentationExceptionReconData
        //                     {
        //                         EndpointSystemType = ep.EndPointSystemType.DisplayName,
        //                         EndpointSystemName = ep.DisplayName,
        //                         AccountName = adHistory.sAMAccountName,
        //                         DisplayName = adHistory.displayName,
        //                         CreatedDate = adHistory.whenCreated.Value,
        //                         LastModifiedDate = adHistory.AuditDate,
        //                         ExceptionType = "Deleted",
        //                         EndpointSystemId = adHistory.EndPointSystemId,
        //                         EndpointUserId = adHistory.UserId,
        //                         UserId = adHistory.DIUserId,
        //                         Version = adHistory.Version
        //                     }
        //                   ).ToList();

        //}

        //public List<PresentationExceptionReconData> GetSAPExceptionReconByendPointSystemId(long endPointSystemId)
        //{
        //    return (from sapep in CurrentContext.SAPEPUserInfos

        //            where sapep.Version == 2
        //            select new PresentationExceptionReconData
        //            {
        //                EndpointSystemType = sapep.EndPointSystem.EndPointSystemType.DisplayName,
        //                EndpointSystemName = sapep.EndPointSystem.DisplayName,
        //                AccountName = sapep.UserName,
        //                DisplayName = sapep.DisplayName,
        //                CreatedDate = sapep.CreatedDate,
        //                LastModifiedDate = null,
        //                ExceptionType = "Created",
        //                EndpointSystemId = sapep.EndPointSystem.Id,
        //                EndpointUserId = sapep.Id,
        //                UserId = sapep.DIUserId,
        //                Version = sapep.Version
        //            }).Union(from sapepHistory in CurrentContext.SAPEPUserInfoHistorys
        //                     from ep in CurrentContext.EndPoint
        //                     where sapepHistory.EndPointSystemId == ep.Id
        //                     from eptype in CurrentContext.EndpointSystemTypes
        //                     where ep.EndpointSystemTypeId == eptype.Id
        //                     where sapepHistory.Version == 2
        //                     select new PresentationExceptionReconData
        //                     {
        //                         EndpointSystemType = ep.EndPointSystemType.DisplayName,
        //                         EndpointSystemName = ep.DisplayName,
        //                         AccountName = sapepHistory.UserName,
        //                         DisplayName = sapepHistory.DisplayName,
        //                         LastModifiedDate = sapepHistory.AuditDate,
        //                         ExceptionType = "Deleted",
        //                         EndpointSystemId = sapepHistory.EndPointSystemId,
        //                         EndpointUserId = sapepHistory.UserId,
        //                         UserId = sapepHistory.DIUserId,
        //                         Version = sapepHistory.Version
        //                     }
        //                   ).ToList();

        //}

        //public List<PresentationExceptionReconData> GetLotusNotesExceptionReconByendPointSystemId(long endPointSystemId)
        //{
        //    return (from inhs in CurrentContext.InhouseUserInfos

        //            where inhs.Version == 2
        //            select new PresentationExceptionReconData
        //            {
        //                //EndpointSystemType = inhs.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
        //                //EndpointSystemName = inhs.EndPointSystem.HostName,
        //                AccountName = inhs.UserName,
        //                DisplayName = inhs.DisplayName,
        //                CreatedDate = inhs.CreatedDate,
        //                LastModifiedDate = null,
        //                ExceptionType = "Created",
        //                EndpointSystemId = inhs.EndPointSystemId,
        //                EndpointUserId = inhs.Id,
        //                UserId = inhs.DIUserId,
        //                Version = inhs.Version
        //            }
        //                 ).ToList();
        //}

        //public List<PresentationExceptionReconData> GetMSSQLExceptionReconByendPointSystemId(long endPointSystemId)
        //{
        //    return (from mssql in CurrentContext.MsSQLUMUserInfos

        //            where mssql.Version == 2
        //            select new PresentationExceptionReconData
        //            {
        //                EndpointSystemType = mssql.EndPointSystem.EndPointSystemType.DisplayName,
        //                EndpointSystemName = mssql.EndPointSystem.DisplayName,
        //                AccountName = mssql.Name,
        //                DisplayName = mssql.Name,
        //                CreatedDate = mssql.CreateDate,
        //                LastModifiedDate = null,
        //                ExceptionType = "Created",
        //                EndpointSystemId = mssql.EndPointSystem.Id,
        //                EndpointUserId = mssql.Id,
        //                UserId = mssql.DIUserId,
        //                Version = mssql.Version

        //            }).Union(from msHistory in CurrentContext.MsSQLUMUserInfoHistory
        //                     from ep in CurrentContext.EndPoint
        //                     where msHistory.EndPointSystemId == ep.Id
        //                     from eptype in CurrentContext.EndpointSystemTypes
        //                     where ep.EndpointSystemTypeId == eptype.Id
        //                     where msHistory.Version == 2
        //                     select new PresentationExceptionReconData
        //                     {
        //                         EndpointSystemType = ep.EndPointSystemType.DisplayName,
        //                         EndpointSystemName = ep.DisplayName,
        //                         AccountName = msHistory.Name,
        //                         DisplayName = msHistory.Name,
        //                         CreatedDate = msHistory.CreateDate,

        //                         ExceptionType = "Deleted",
        //                         EndpointSystemId = msHistory.EndPointSystemId,
        //                         EndpointUserId = msHistory.UserId,
        //                         UserId = msHistory.DIUserId,
        //                         Version = msHistory.Version
        //                     }
        //                   ).ToList();

        //}

        //public List<PresentationEndpointInformation> GetInfromationByDIUserid(long diUserId)
        //{
        //    List<PresentationEndpointInformation> EndPointlist = new List<PresentationEndpointInformation>();
        //    var ADSquery = (from endpointsys in CurrentContext.EndPoint
        //                    join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //                    join usr in CurrentContext.ADSUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //                    where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked && !usr.IsAccountDisabled

        //                    select new
        //                    {
        //                        Endpointsystemid = endpointsys.Id,
        //                        EndPointSystemDescription = endpointsys.Description,
        //                        endpointsys.EndpointSystemTypeId,
        //                        HostName = endpointsys.DisplayName,
        //                        endpointsys.DisplayName,

        //                        Name = endpointsystype.EndpointSystemTypeName,
        //                        UserName = usr.sAMAccountName
        //                    }).ToList();

        //    var SAPquery = (from endpointsys in CurrentContext.EndPoint
        //                    join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //                    join usr in CurrentContext.SAPUserinfos on endpointsys.Id equals usr.EndPointSystemId
        //                    where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
        //                    select new
        //                    {
        //                        Endpointsystemid = endpointsys.Id,
        //                        EndPointSystemDescription = endpointsys.Description,
        //                        endpointsys.EndpointSystemTypeId,
        //                        HostName = endpointsys.DisplayName,
        //                        endpointsys.DisplayName,

        //                        Name = endpointsystype.EndpointSystemTypeName,
        //                        UserName = usr.UserLogonName
        //                    }).ToList();

        //    var Lotusquery = (from endpointsys in CurrentContext.EndPoint
        //                      join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //                      join usr in CurrentContext.LotusUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //                      where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
        //                      select new
        //                      {
        //                          Endpointsystemid = endpointsys.Id,
        //                          EndPointSystemDescription = endpointsys.Description,
        //                          endpointsys.EndpointSystemTypeId,
        //                          HostName = endpointsys.DisplayName,
        //                          endpointsys.DisplayName,
        //                          Name = endpointsystype.EndpointSystemTypeName,
        //                          UserName = usr.ShortName
        //                      }).ToList();

        //    var SAPEPquery = (from endpointsys in CurrentContext.EndPoint
        //                      join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //                      join usr in CurrentContext.SAPEPUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //                      where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
        //                      select new
        //                      {
        //                          Endpointsystemid = endpointsys.Id,
        //                          EndPointSystemDescription = endpointsys.Description,
        //                          endpointsys.EndpointSystemTypeId,
        //                          HostName = endpointsys.DisplayName,
        //                          endpointsys.DisplayName,

        //                          Name = endpointsystype.EndpointSystemTypeName,
        //                          usr.UserName
        //                      }).ToList();

        //    var INHquery = (from endpointsys in CurrentContext.EndPoint
        //                    join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //                    join usr in CurrentContext.InhouseUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //                    where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
        //                    select new
        //                    {
        //                        Endpointsystemid = endpointsys.Id,
        //                        EndPointSystemDescription = endpointsys.Description,
        //                        endpointsys.EndpointSystemTypeId,
        //                        HostName = endpointsys.DisplayName,
        //                        endpointsys.DisplayName,

        //                        Name = endpointsystype.EndpointSystemTypeName,
        //                        usr.UserName
        //                    }).ToList();


        //    var Linuxquery = (from endpointsys in CurrentContext.EndPoint
        //                      join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //                      join usr in CurrentContext.LinuxUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //                      where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
        //                      select new
        //                      {
        //                          Endpointsystemid = endpointsys.Id,
        //                          EndPointSystemDescription = endpointsys.Description,
        //                          endpointsys.EndpointSystemTypeId,
        //                          HostName = endpointsys.DisplayName,
        //                          endpointsys.DisplayName,
        //                          Name = endpointsystype.EndpointSystemTypeName,
        //                          UserName = usr.UserName
        //                      }).ToList();





        //    var MSSQLquery = (from endpointsys in CurrentContext.EndPoint
        //                    join endpointsystype in CurrentContext.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
        //                    join usr in CurrentContext.MsSQLUMUserInfos on endpointsys.Id equals usr.EndPointSystemId
        //                    where usr.DIUserId == diUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked 
        //                    select new
        //                    {
        //                        Endpointsystemid = endpointsys.Id,
        //                        EndPointSystemDescription = endpointsys.Description,
        //                        endpointsys.EndpointSystemTypeId,
        //                        HostName = endpointsys.DisplayName,
        //                        endpointsys.DisplayName,

        //                        Name = endpointsystype.EndpointSystemTypeName,
        //                        UserName = usr.Name
        //                    }).ToList();
        //    //var listTotal = ADSquery.Union(SAPquery).Union(Lotusquery).Union(SAPEPquery);
        //    var listTotal1 = ADSquery.Union(SAPquery);
        //    var listTotal2 = listTotal1.Union(Lotusquery);
        //    var listTotal3 = listTotal2.Union(SAPEPquery);
        //    var listTotal4 = listTotal3.Union(Linuxquery);
        //    //var listTotal = listTotal3.Union(INHquery);
        //    //var listTotal5 = listTotal4.Union(INHquery);
        //    var listTotal5 = listTotal4.Union(MSSQLquery);
        //    var listTotal = listTotal4.Union(INHquery);



        //    if (listTotal.Count() > 0)
        //    {
        //        foreach (var tmpList in listTotal)
        //        {
        //            PresentationEndpointInformation EndPointSystem = new PresentationEndpointInformation();
        //            EndPointSystem.Endpointsystemid = CheckNull.AgainstInt(tmpList.Endpointsystemid);
        //            EndPointSystem.Endpointsystemdescription = CheckNull.AgainstString(tmpList.EndPointSystemDescription);
        //            EndPointSystem.EndpointsystemType = CheckNull.AgainstString(tmpList.Name);
        //            EndPointSystem.EndpointSystemTypeId = CheckNull.AgainstInt(tmpList.EndpointSystemTypeId);
        //            EndPointSystem.HostName = CheckNull.AgainstString(tmpList.HostName);
        //            EndPointSystem.UserName = CheckNull.AgainstString(tmpList.UserName);
        //            EndPointSystem.Status = string.Empty;
        //            EndPointlist.Add(EndPointSystem);
        //        }
        //    }
        //    return EndPointlist;
        //}

        public Dictionary<long, string> GetEndPointSystemTypes()
        {
            return CurrentContext.EndpointSystemTypes.Where(item => item.IsActive == true)
                       .Select(x => new { Key = x.Id, Value = x.DisplayName })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }

        public void ResolveAllExceptionsForFirstTimeDiscovery(EndPoint _endpoint)
        {

            CurrentContext.Database.ExecuteSqlCommand("Delete From DI_M_EntitlementReconcilation where EndpointSystemId=@EndpointSystemId", new System.Data.SqlClient.SqlParameter("@EndpointSystemId", _endpoint.Id));
            CurrentContext.Database.ExecuteSqlCommand("Delete From DI_M_AccountReconcilation where EndpointSystemId=@EndpointSystemId", new System.Data.SqlClient.SqlParameter("@EndpointSystemId", _endpoint.Id));

            string updateQuery = "update entRecon set entRecon.AccountReconcilationId = acctRecon.AccountReconcilationId from DI_M_EntitlementReconcilation entRecon join DI_M_AccountReconcilation acctRecon on entRecon.EndpointUserId = acctRecon.EndpointUserId AND entRecon.EndpointSystemId = acctRecon.EndpointSystemId";

            CurrentContext.Database.ExecuteSqlCommand(updateQuery);
            CurrentContext.SaveChanges();
        }

        public Dictionary<long, string> GetProvisioningApplicableEndPoints()
        {
            return (from EPS in CurrentContext.EndPoint
                    where EPS.IsTrustedSource == false && !string.IsNullOrEmpty(EPS.DisplayName) &&  EPS.EndpointSystemTypeId != (long)EndpointType.InhouseSystems
                    select new { EPS.DisplayName, EPS.Id })
                        .Distinct().OrderBy(c => c.DisplayName)
                        .ToDictionary(c => c.Id, c => c.DisplayName);
        }
        public List<EndPoint> GetDefaultApplication()
        {
            var data = (from endPoint in CurrentContext.EndPoint
                    join defaultAppln in CurrentContext.NRulesetForApplications on endPoint.Id equals defaultAppln.EndpointSystemId
                    select endPoint
                    ).ToList();
            var eps = CurrentContext.EndPoint.Where(i => i.ApplicationType == ApplicationType.MicrosoftTeams || i.ApplicationType == ApplicationType.Office365).ToList();
            //Below list are added because those system are handled manually
            if(eps != null && eps.Count > 0)
                data.AddRange(eps);
            return data;
        }

        public List<EndPoint> GetActiveOtherSystemAutomatedEndpoints()
        {
            return CurrentContext.Database.SqlQuery<EndPoint>("select Distinct E.* from DI_M_EndpointSystem E  join DI_INH_Permission P on E.Id = P.EndPointSystemId where E.IsActive = 1 and E.EndpointSystemTypeId = 9 and P.IsActive = 1 AND P.ADGroups IS NOT NULL and LEN(RTRIM(LTRIM(ISNULL(P.ADGroups,''))))> 0").ToList();
        }
        public Dictionary<long, string> GetInhouseEndPointsAutomateSystem()
        {
            return (from EPS in CurrentContext.EndPoint.Include("EndPointSystemType")
                    where EPS.IsActive && EPS.CanAutomate == true
                    && EPS.EndPointSystemType.EndpointSystemTypeName == EndpointType.InhouseSystems
                    select new { EPS.DisplayName, EPS.Id })
                        .Distinct().OrderBy(c => c.DisplayName)
                        .ToDictionary(c => c.Id, c => c.DisplayName);
        }
    }
}
