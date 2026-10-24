using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{
    public class VPNApplicationEnvironment : DomainModelBase<VPNApplicationEnvironment, EntityModel.VPNApplicationEnvironment>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public long MatrixId { get; set; }
        [Required]
        public VPNMatrixType MatrixType { get; set; }
        [Required]
        public VPNMatrixEnviornmentType EnviornmentType { get; set; }
        public string ADGroups { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public static Dictionary<VPNMatrixEnviornmentType, bool> GetApplicationEnviornment(long endPointUserId, bool excludeApplication, long matrixId)
        {
            var returnValue = new Dictionary<VPNMatrixEnviornmentType, bool>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                returnValue = new VPNApplicationEnvironmentRepository(dataAccessTransaction).GetApplicationEnviornment(endPointUserId, excludeApplication, matrixId);
            }
            return returnValue;
        }
        public static List<VPNApplicationEnvironment> GetByMatrixId(long matrixId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.VPNApplicationEnvironment>(dataAccessTransaction).Get(i => i.MatrixId == matrixId).ToList());
            }
        }

        public static Dictionary<long, string> GetApplicationForBulkRequest(string searchString, VPNType vpnType)
        {
            var returnValue = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                returnValue = new VPNApplicationEnvironmentRepository(dataAccessTransaction).GetVPNMatrixByType( searchString, vpnType);
            }
            return returnValue;
        }

        public static void TruncateApplicationEnviornment()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new VPNUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString("DELETE FROM dbo.DI_VPN_ApplicationEnviornment"));
                dataAccessTransaction.Commit();
            }
        }
        public static Dictionary<long, string> GetActiveVPNMatrix(long endPointUserId, long endpointId, bool isFulltimeEmployee, string searchString, AdHocActionType adHocActionType,VPNType vpnType)
        {
            var returnValue = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                returnValue = new VPNApplicationEnvironmentRepository(dataAccessTransaction).GetApplication(endPointUserId, endpointId, isFulltimeEmployee, searchString, adHocActionType, vpnType);
            }
            return returnValue;
        }
        public static VPNApplicationEnvironment GetByEntitlementAndMatrixType(string entitlement,int entitlementType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var vpnType = (VPNMatrixType)entitlementType;
                return ORMap(new RepositoryBase<EntityModel.VPNApplicationEnvironment>(dataAccessTransaction).Get(i => i.Name == entitlement && i.MatrixType == vpnType).FirstOrDefault());
            }
        }
    }
}
