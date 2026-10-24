using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class NPACSectionInfo : DomainModelBase<NPACSectionInfo, EntityModel.NPACSectionInfo>, IEndpointEntitlementInfo, IXLData
    {
        public NPACSectionInfo()
        {
        }
        public long EndPointSystemId { get; set; }
        public string NPACSection { get; set; }
        public string HRSectionName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public string EntitlementName
        {
            get { return this.NPACSection; }
        }
        public string EntitlementDesc
        {
            get { return this.HRSectionName; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Section; }
        }
        public string ValidationMessage { get; set; }
        public bool IsValid { get; set; }

        public static string GetNPACSectionCodeFromHRSectionName(string hrSectionName)
        {
            NPACSectionInfo secInfo = new NPACSectionInfo();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                secInfo = ORMap(new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction)
                      .Get(grp => grp.HRSectionName == hrSectionName).FirstOrDefault());
            }

            if (secInfo != null)
                return secInfo.NPACSection;

            return "";
        }
        public static List<NPACSectionInfo> GetAllNPACSections(bool isValidRecords, bool isAllRecords)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (isAllRecords)
                    return ORMapList(new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction).GetAll().ToList());
                else
                {
                    if (isValidRecords)
                        return ORMapList(new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                    else
                        return ORMapList(new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                }
            }

        }

        public static NPACSectionInfo GetNPACSectionByName(string sectionName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                    return ORMap(new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction).Get(p => p.HRSectionName == sectionName).FirstOrDefault());
            }
        }
        public static NPACSectionInfo GetNPACSectionByCode(string sectionCode)
        {
            NPACSectionInfo usrInfo = null;
            if (string.IsNullOrWhiteSpace(sectionCode)) return usrInfo;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var orgName = NOrganisation.GetByOrgCode(sectionCode);
                if(orgName != null)
                {
                    usrInfo = ORMap(new NPACSectionInfoRepository(dataAccessTransaction).GetByOrganizationCode(sectionCode));
                }
                return usrInfo;
            }
        }

        public static List<NPACSectionInfo> GetAllNPACSections(long endpointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction)
                    .Get(cnfg => cnfg.EndPointSystemId == endpointId).ToList());
            }
        }
        public static bool TruncateTable()
        {
            bool returnValue = false;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction).ExecuteQuery("Truncate table DI_NPAC_SectionInfo");
                dataAccessTransaction.Commit();
                returnValue = true;
            }

            return returnValue;
        }
        public static NPACSectionInfo GetNPACXLProvisioningMatrixbyObject(NPACSectionInfo obj)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction)
                    .Get(cnfg => cnfg.HRSectionName == obj.HRSectionName).FirstOrDefault());
            }


        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
                        //string userDisplayName = User.GetByUserName(lastModifiedDate.CreatedBy)?.DisplayName;
                        //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }

        public static void saveSections(IList<NPACSectionInfo> list, long EndpointSystemId, string currentUser)
        {
            NPACSectionInfo existingSec = null;

            IList<NPACSectionInfo> listExistingSects = new List<NPACSectionInfo>();

            try
            {


                var endpoint = EndPoint.GetById(EndpointSystemId);
                var endpointSystemTypeId = endpoint.EndpointSystemTypeId;
                listExistingSects = NPACSectionInfo.GetAllNPACSections(EndpointSystemId);

                foreach (NPACSectionInfo sec in list)
                {
                    try
                    {
                        existingSec = listExistingSects.Where(t =>
                        t.NPACSection.Trim() == sec.NPACSection.Trim()
                        && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();

                        if (existingSec == null)
                        {
                            sec.NPACSection = sec.NPACSection.Trim();
                            sec.HRSectionName = sec.HRSectionName.Trim();
                            sec.CreatedBy = "System";
                            sec.CreatedDate = DateTime.Now;
                            sec.Save(currentUser);
                            listExistingSects.Add(sec);
                        }
                        else
                        {
                            sec.Id = existingSec.Id;
                            sec.NPACSection = sec.NPACSection.Trim();
                            sec.HRSectionName = sec.HRSectionName.Trim();
                            sec.LastModifiedBy = "System";
                            sec.LastModifiedDate = DateTime.Now;
                            sec.Save(currentUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while NPAC SaveSections ", ex);
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while NPAC SaveSections.", ex);
                throw ex;
            }
        }

        public static List<NPACSectionInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<NPACSectionInfo> listDeletedSetion = new List<NPACSectionInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedSetion = ORMapList(new RepositoryBase<EntityModel.NPACSectionInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedSetion;
            }
        }
    }
}
