using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SkypeForBusinessUnAssignedNumber : DomainModelBase<SkypeForBusinessUnAssignedNumber, EntityModel.SkypeForBusinessUnAssignedNumber>
    {
        public int StartRange { get; set; }
        public int EndRange { get; set; }
        public int Priority { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public static bool CheckNumberGeneratedAndAssigned(long endpointId, long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                
                var exisitingData = SkypeForBusinessUnAssignedNumber.GetByEndpointId(endpointId,id);
                var isExists = new RepositoryBase<EntityModel.SkypeForBusinessUnAssignedNumberMatrix>(dataAccessTransaction).Get(i => i.Priority == exisitingData.Priority && i.Assigned).ToList();
                if(isExists.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool DeleteRangeAndGeneratedMatrix(long endpointId, long id)
        {
            bool isSuccess = false;
            try
            {
                using (var dataAccessTransaction = new DataAccessTransaction())
                {

                    var exisitingData = SkypeForBusinessUnAssignedNumber.GetByEndpointId(endpointId, id);
                    new SkypeForBusinessUnAssignedNumberRepository(dataAccessTransaction).DeleteNumberRanges(exisitingData.Priority);
                    SkypeForBusinessUnAssignedNumber.Delete(exisitingData.Id);
                    dataAccessTransaction.Commit();
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public EndPoint EndPoint { get; set; }
        public long EndPointSystemId { get; set; }
        public static void SaveUnAssignedNumber(SkypeForBusinessUnAssignedNumber number, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var exisitingData =  ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUnAssignedNumber>(dataAccessTransaction).Get(i=> i.EndPointSystemId == number.EndPointSystemId && i.StartRange == number.StartRange &&  i.EndRange == number.EndRange).FirstOrDefault());
                if (exisitingData == null)
                {
                    exisitingData = number;
                    exisitingData.CreatedBy = currentUser;
                    exisitingData.CreatedDate = DateTime.Now;
                    exisitingData.Save(currentUser);
                }
                else
                {
                    exisitingData.StartRange = number.StartRange;
                    exisitingData.EndRange = number.EndRange;
                    exisitingData.Priority = number.Priority;
                    exisitingData.LastModifiedBy = currentUser;
                    exisitingData.LastModifiedDate = DateTime.Now;
                    exisitingData.Save(currentUser);
                }
                
            }
        }
        public static int GetUnAssignedRandomNumber(long endpointId, string currentUser)
        {
            using (var trans = new DataAccessTransaction(currentUser))
            {
                return new SkypeForBusinessUnAssignedNumberRepository(trans).GetUnAssignedRandomNumber(endpointId);
            }
        }
        public static List<SkypeForBusinessUnAssignedNumber> GetAllByEndpointId(long endpointId, string currentUser)
        {
            List<SkypeForBusinessUnAssignedNumber> numbers = new List<SkypeForBusinessUnAssignedNumber>();
           using (var trans = new DataAccessTransaction(currentUser))
            {
                numbers = ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessUnAssignedNumber>(trans).Get(i=>i.EndPointSystemId == endpointId).ToList());
            }
            return numbers;
        }
        public static SkypeForBusinessUnAssignedNumber GetByEndpointId(long endpointId,long id)
        {
            using (var trans = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUnAssignedNumber>(trans).Get(i => i.EndPointSystemId == endpointId && i.Id == id).FirstOrDefault());
            }
        }
        public static void DeletedGroups(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<SkypeForBusinessUnAssignedNumber> listDeletedUsers = new List<SkypeForBusinessUnAssignedNumber>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessUnAssignedNumber>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                foreach (var deletedUsers in listDeletedUsers)
                {
                    deletedUsers.Delete();
                }
            }
        }
    }
}
