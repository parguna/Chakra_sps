using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.DomainModel
{
    public class SAPHANASectionMappingInfo : DomainModelBase<SAPHANASectionMappingInfo, EntityModel.SAPHANASectionMappingInfo>
    {
        public string HRMSSectionCode { get; set; }
        public string SAPSectionCode { get; set; }
        public string VBUKR { get; set; } 
        public bool IsActive { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static SAPHANASectionMappingInfo GetBySAPSectionCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANASectionMappingInfo>(dataAccessTransaction).Get(org => org.SAPSectionCode == code, true).FirstOrDefault());
            }
        }
        public static List<SAPHANASectionMappingInfo> GetByHRMSSectionCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANASectionMappingInfo>(dataAccessTransaction).Get(org => org.HRMSSectionCode == code, true).ToList());
            }
        }

        public static void SetAllInActive()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                string cmd = string.Format(" UPDATE DI_SAPHANA_SectionMappingInfo set IsActive = 0 ");
                SAPHANASectionMappingInfo.ExecuteQuery(cmd); 
                dataAccessTransaction.Commit();
            }
        }

        public static bool TruncateTable(long endpointId)
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPHANASectionMappingInfo>(dataAccessTransaction).ExecuteQuery($"DELETE FROM DI_SAPHANA_SectionMappingInfo WHERE EndpointSystemId = {endpointId}");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
    }
}
