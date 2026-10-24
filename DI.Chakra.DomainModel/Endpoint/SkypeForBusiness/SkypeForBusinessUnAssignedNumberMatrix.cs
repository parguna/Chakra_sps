using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SkypeForBusinessUnAssignedNumberMatrix : DomainModelBase<SkypeForBusinessUnAssignedNumberMatrix, EntityModel.SkypeForBusinessUnAssignedNumberMatrix>
    {
        public int Number { get; set; }
        public int Priority { get; set; }
        public bool Assigned { get; set; }
        public bool Reserved { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long EndPointSystemId { get; set; }
        public EndPoint EndPoint { get; set; }
        public static SkypeForBusinessUnAssignedNumberMatrix GetByExtension(int extension)
        {
            using (var trans = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUnAssignedNumberMatrix>(trans).Get(i => i.Number== extension).FirstOrDefault());
            }
        }
        public static List<SkypeForBusinessUnAssignedNumberMatrix> GetAllByEndpointId(long endpointId, string currentUser)
        {
            List<SkypeForBusinessUnAssignedNumberMatrix> numbers = new List<SkypeForBusinessUnAssignedNumberMatrix>();
            using (var trans = new DataAccessTransaction(currentUser))
            {
                numbers = ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessUnAssignedNumberMatrix>(trans).Get(i => i.EndPointSystemId == endpointId).ToList());
            }
            return numbers;
        }
        public static List<SkypeForBusinessUnAssignedNumberMatrix> GetSFBExtensionMatrixData(long endpointId, string currentUser)
        {
            List<SkypeForBusinessUnAssignedNumberMatrix> numbers = new List<SkypeForBusinessUnAssignedNumberMatrix>();
            using (var trans = new DataAccessTransaction(currentUser))
            {
                numbers = ORMapList(new SkypeForBusinessUnAssignedNumberRepository(trans).GetSFBExtensionMatrixData(endpointId));
            }
            return numbers;
        }
        

        public static Dictionary<long, string> GetAllReservedNumbers(long endpointId, string searchString)
        {
            var returnValue = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                returnValue = new SkypeForBusinessUnAssignedNumberRepository(dataAccessTransaction).GetAllReservedNumbers(endpointId, searchString);
            }
            return returnValue;
        }

        public static List<SkypeForBusinessUnAssignedNumberMatrix> GetAllByEndpointIdByExcludingReservedNumber(long endpointId, string currentUser)
        {
            List<SkypeForBusinessUnAssignedNumberMatrix> numbers = new List<SkypeForBusinessUnAssignedNumberMatrix>();
            using (var trans = new DataAccessTransaction(currentUser))
            {
                numbers = ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessUnAssignedNumberMatrix>(trans).Get(i => i.EndPointSystemId == endpointId && i.Reserved == false).ToList());
            }
            return numbers;
        }
        public static void UpdateNumberStatus(long endpointId, int number, string currentUser, bool assigned = false)
        {
            using (var trans = new DataAccessTransaction(currentUser))
            {
                new SkypeForBusinessUnAssignedNumberRepository(trans).UpdateNumber( endpointId, number, assigned);
                trans.Commit();
            }
        }
        public static void DeleteNumbers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<SkypeForBusinessUnAssignedNumberMatrix> listDeletedNumbers = new List<SkypeForBusinessUnAssignedNumberMatrix>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedNumbers = ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessUnAssignedNumberMatrix>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                foreach (var deletedUsers in listDeletedNumbers)
                {
                    deletedUsers.Delete();
                }
            }
        }


    }
}
