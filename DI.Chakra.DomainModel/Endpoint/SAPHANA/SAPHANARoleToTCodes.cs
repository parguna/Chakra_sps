using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANARoleToTCodes : DomainModelBase<SAPHANARoleToTCodes, EntityModel.SAPHANARoleToTCodes>
    {
        public string RoleCode { get; set; }

        public string TCodes { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static void UpdateRoleToTcodeAfterDiscovery(long EndpointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPHANARoleToTCodesRepository(dataAccessTransaction).GenerateRoleToTCodesByEndpoint(EndpointId);
            }
        }
        public static void RunRolesToTCodeGenertion()
        {
            var endpoints = EndPoint.GetByEndpointType(Infrastructure.Enums.EndpointType.SAP, null);
            foreach (var endpoint in endpoints)
            {
                try
                {
                    var date = DateTime.Now;
                    UpdateRoleToTcodeAfterDiscovery(endpoint.Id);
                    DeleteSapRoleTCodeMappingForEndpoint(endpoint.Id, date);
                }
                catch (Exception ex)
                {
                    DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"failed to generate TCode for endpoint:{endpoint.DisplayName}",ex);
                }
            }
        }
        public static void DeleteSapRoleTCodeMappingForEndpoint(long endpointId, DateTime? processStartAt)
        {
            var date = processStartAt.Value.ToString("dd-MMM-yyyy hh:mm:ss");
            //(obj.LastModifiedDate == null && obj.CreatedDate < modifiedDate) || obj.LastModifiedDate < modifiedDate)
            ExecuteQuery($"Delete from DI_SAPHANA_RoleToTCodes WHERE EndPointSystemId={endpointId} AND ((LastModifiedDate IS Null AND CreatedDate < '{date}') " +
                $" OR (LastModifiedDate <  '{date}') )");
        }
    }
}
