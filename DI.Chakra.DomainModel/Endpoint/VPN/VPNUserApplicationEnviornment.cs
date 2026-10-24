using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class VPNUserApplicationEnviornment : DomainModelBase<VPNUserApplicationEnviornment, EntityModel.VPNUserApplicationEnviornment>
    {
        public long EndpointUserId { get; set; }
        public long MatrixId { get; set; }
        public string Name { get; set; }
        public VPNMatrixType MatrixType { get; set; }
        public VPNMatrixEnviornmentType EnviornmentType { get; set; }
        public string ADGroups { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public VPNMatrix VPNMatrix { get; set; }

        public static VPNUserApplicationEnviornment GetByNameAndType(long endPointUserId, string name, VPNMatrixType matrixType, VPNMatrixEnviornmentType envType, long matrixId)
        {
            using (DataAccessTransaction trans = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.VPNUserApplicationEnviornment>(trans).Get(res => res.EndpointUserId == endPointUserId && res.Name == name && res.MatrixType == matrixType && res.EnviornmentType == envType && res.MatrixId == matrixId).FirstOrDefault());
            }

        }
        public static void ProcessDeletedEntitlements(long EndpointId, Process objDiscoveryProcess, string currentUser)
        {
            List<VPNUserApplicationEnviornment> listDeletedEntitlements = new List<VPNUserApplicationEnviornment>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                if (objDiscoveryProcess != null)
                {
                    listDeletedEntitlements = ORMapList(new RepositoryBase<EntityModel.VPNUserApplicationEnviornment>(dataAccessTransaction)
                           .Get(objUser => ((objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt) ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null))).ToList());
                }
            }
            foreach (var item in listDeletedEntitlements)
            {
                item.Delete();
            }
        }
        public static void DeleteByUserId(long userId, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                new VPNUserInfoRepository(dataAccessTransaction).DeleteByUserId(userId);
                dataAccessTransaction.Commit();
            }
        }
        public static List<VPNUserApplicationEnviornment> GetByIds(List<long> vpnUserApplnEnvIds, long endPointUserId)
        {
            using (DataAccessTransaction trans = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.VPNUserApplicationEnviornment>(trans).Get(res => res.EndpointUserId == endPointUserId && vpnUserApplnEnvIds.Contains(res.Id)).ToList());
            }
        }

        public static List<VPNUserApplicationEnviornment> GetUserApplicationEnviornment(long endpointUserId, string name)
        {
            using (DataAccessTransaction trans = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.VPNUserApplicationEnviornment>(trans).Get(res => res.EndpointUserId == endpointUserId && res.Name != name).ToList());
            }
        }
        public static List<VPNUserApplicationEnviornment> GetByMatrixId(long matrixId, long endPointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.VPNUserApplicationEnviornment>(dataAccessTransaction).Get(i => i.EndpointUserId == endPointUserId && i.MatrixId == matrixId).ToList());
            }
        }
        public static List<VPNUserApplicationEnviornment> GetByMatrixId(long matrixId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.VPNUserApplicationEnviornment>(dataAccessTransaction).Get(i => i.MatrixId == matrixId).ToList());
            }
        }
    }
}
