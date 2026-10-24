using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPTCodeObjectMap : DomainModelBase<SAPTCodeObjectMap, EntityModel.SAPTCodeObjectMap>
    {
        public string ObjectType { get; set; }

        public string FieldValue { get; set; }

        public string ObjectCode { get; set; }

        public string FieldCode { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


        public int? Version { get; set; }


        public static SAPTCodeObjectMap GetByCodes(string objectCode, string fieldCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPTCodeObjectMap>(dataAccessTransaction).Get(codeInfo => codeInfo.ObjectCode == objectCode && codeInfo.FieldCode == fieldCode && codeInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static List<SAPTCodeObjectMap> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPTCodeObjectMap>(dataAccessTransaction).Get(codeInfo => codeInfo.EndPointSystemId == endPointSystemId && codeInfo.LastModifiedDate < modifiedDate).ToList());
            }
        }

        //public static List<SAPTCodeObjectMap> GetDeletedSAPTCodeObjectMap(long endPointSystemId)
        //{
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        List<SAPTCodeObjectMap> sapTcodeObjectMap = new List<SAPTCodeObjectMap>();
        //        DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
        //        objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemIdDefault(endPointSystemId);
        //        if (objDiscoveryProcess != null)
        //        {
        //            Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
        //            if (process != null)
        //            {
        //                sapTcodeObjectMap = GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
        //            }
        //        }
        //        return sapTcodeObjectMap;
        //    }
           
        //}

        public void RemoveSAPTCodeObjectMap(SAPTCodeObjectMap mapInfo)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPTCodeObjectMap sapObject = new SAPTCodeObjectMap();
                sapObject = GetByCodes(mapInfo.ObjectCode, mapInfo.FieldCode, mapInfo.EndPointSystemId);
                sapObject.Delete(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }


        public static void DeleteSAPTCodeObjectMap(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPTCodeObjectMap>(dataAccessTransaction).Get(saptcode => saptcode.EndPointSystemId == endPointSystemId).ToList());
                 DeleteList(ItemstobeDeleted);
             }
        }

        public static void SaveSAPTCodeObjectMap(IList<SAPTCodeObjectMap> list, long endpointSystemId, string currentUser, IList<SAPTCodeObjectMap> existingObjects = null)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                IList<SAPTCodeObjectMap> listExistingObjects = new List<SAPTCodeObjectMap>();
                SAPTCodeObjectMap sapExistingObj = null;
                if(existingObjects == null)
                {
                    listExistingObjects = SAPTCodeObjectMap.GetByEndpointId(endpointSystemId);
                }
                else
                {
                    listExistingObjects = existingObjects;
                }
                
                foreach (SAPTCodeObjectMap obj in list)
                {
                    sapExistingObj = listExistingObjects.Where(t => t.ObjectType == obj.ObjectType
                   && t.FieldCode == obj.FieldCode
                   && t.FieldValue == obj.FieldValue
                   && t.ObjectCode == obj.ObjectCode
                   && t.EndPointSystemId == endpointSystemId).FirstOrDefault();
                    if (sapExistingObj != null)
                    {
                        obj.Id = sapExistingObj.Id;
                        listExistingObjects.Add(obj);
                    }
                    obj.Save(currentUser);
                }

                //dataAccessTransaction.Commit();
            }
        }

        public static void SaveSAPTCodeObjectMap(IList<SAPTCodeObjectMap> list, string currentUser)
        {
            foreach (SAPTCodeObjectMap tcObj in list)
            {
                if(tcObj!=null)
                tcObj.Save(currentUser);
            }
        }

        public static List<SAPTCodeObjectMap> GetByEndpointId(long endPointsystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPTCodeObjectMap>(dataAccessTransaction).
                    Get(codeInfo => codeInfo.EndPointSystemId == endPointsystemId).ToList());
            }
        }

        public static void DeleteTCodesByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAP_TCodeObject] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQuery(String.Format(query, endpointSystemId));
            }
        }
    }
}
