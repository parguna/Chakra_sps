using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAObjectInfo : DomainModelBase<SAPHANAObjectInfo, EntityModel.SAPHANAObjectInfo>
    {
        public string SapObjectCode { get; set; }

        public string FieldCode1 { get; set; }

        public string FieldCode2 { get; set; }

        public string FieldCode3 { get; set; }

        public string FieldCode4 { get; set; }

        public string FieldCode5 { get; set; }

        public string FieldCode6 { get; set; }

        public string FieldCode7 { get; set; }

        public string FieldCode8 { get; set; }

        public string FieldCode9 { get; set; }

        public string FieldCode0 { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }



        public int? Version { get; set; }


        public static IList<SAPHANAObjectInfo> GetByEndpointId(long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAObjectInfo>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointId).ToList());
            }
        }



        public static List<SAPHANAObjectInfo> GetByObjCode( string objCode,long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<SAPHANAObjectInfo>  res =ORMapList(new RepositoryBase<EntityModel.SAPHANAObjectInfo>(dataAccessTransaction).
                    Get(obj => obj.EndPointSystemId == endPointId && obj.SapObjectCode==objCode).Take(100).ToList());
                return res;
            }
        }

        public static IList<SAPHANAObjectInfo> GetByModifiedDate(long endPointId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAObjectInfo>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointId && obj.LastModifiedDate< modifiedDate).ToList());
            }
        }

        public static Dictionary<long, string> GetDictionaryByObjCode(long endPointId, string objCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPHANAObjectInfoRepository(dataAccessTransaction).
                    GetByEndpointObjCode(endPointId, CheckNull.AsteriskToString(objCode));
            }
        }


        public static List<string> GetSapFields(long endPointId, string objCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPHANAObjectInfo sapobject = SAPHANAObjectInfo.GetByObjCode(CheckNull.AsteriskToString(objCode), endPointId).FirstOrDefault();
                return SAPHANAObjectInfo.GetSapFields(endPointId, CheckNull.AsteriskToString(objCode), sapobject);                    
            }
        }

        private static List<string> GetSapFields(long EndpointId, string ObjCode, SAPHANAObjectInfo sapObjectInfo)
        {
            List<string> FieldList = new List<string>();

            if (sapObjectInfo != null)
            {
                if (sapObjectInfo.FieldCode0 != null && sapObjectInfo.FieldCode0 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode0);

                if (sapObjectInfo.FieldCode1 != null && sapObjectInfo.FieldCode1 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode1);

                if (sapObjectInfo.FieldCode2 != null && sapObjectInfo.FieldCode2 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode2);

                if (sapObjectInfo.FieldCode3 != null && sapObjectInfo.FieldCode3 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode3);

                if (sapObjectInfo.FieldCode4 != null && sapObjectInfo.FieldCode4 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode4);

                if (sapObjectInfo.FieldCode5 != null && sapObjectInfo.FieldCode5 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode5);

                if (sapObjectInfo.FieldCode6 != null && sapObjectInfo.FieldCode6 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode6);

                if (sapObjectInfo.FieldCode7 != null && sapObjectInfo.FieldCode7 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode7);

                if (sapObjectInfo.FieldCode8 != null && sapObjectInfo.FieldCode8 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode8);

                if (sapObjectInfo.FieldCode9 != null && sapObjectInfo.FieldCode9 != string.Empty)
                    FieldList.Add(sapObjectInfo.FieldCode9);
            }
            return FieldList;
        }


        public static IList<SAPHANAObjectInfo> GetDeletedSAPObject(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPHANAObjectInfo> listDeletedSapTCodeObject = new List<SAPHANAObjectInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endpointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery); 
                    if (process != null)
                    {
                        listDeletedSapTCodeObject = SAPHANAObjectInfo.GetByModifiedDate(endpointSystemId, process.ProcessStartAt);
                    }
                }

                return listDeletedSapTCodeObject;

            }
        }

        
        public static void SaveSapObject(IList<SAPHANAObjectInfo> list, long endpointSystemId,string currentUser, IList<SAPHANAObjectInfo> existingObjects = null)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                IList<SAPHANAObjectInfo> listExistingObjects = new List<SAPHANAObjectInfo>();
                SAPHANAObjectInfo sapExistingObj = null;
                if (existingObjects == null)
                {
                    listExistingObjects = SAPHANAObjectInfo.GetByEndpointId(endpointSystemId);
                }
                else
                {
                    listExistingObjects = existingObjects;
                }
                foreach (SAPHANAObjectInfo obj in list)
                {
                    sapExistingObj = listExistingObjects.Where(t => t.SapObjectCode == obj.SapObjectCode && t.FieldCode1 == obj.FieldCode1
                   && t.EndPointSystemId == endpointSystemId).FirstOrDefault();
                    if (sapExistingObj == null)
                    {
                        obj.Save(currentUser);
                        listExistingObjects.Add(obj);
                    }
                    else
                    {
                        obj.Id = sapExistingObj.Id;
                        obj.Save(currentUser);
                    }
                }

                //dataAccessTransaction.Commit();
            }
        }

        public static void SaveSapObject(IList<SAPHANAObjectInfo> list, string currentUser)
        {
            foreach (SAPHANAObjectInfo obj in list)
            {
                obj.Save(currentUser);
            }
        }

        public static void DeleteSAPObjects(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPHANAObjectInfo>(dataAccessTransaction).Get(sapazObject => sapazObject.EndPointSystemId == endPointSystemId).ToList());
                DeleteList(ItemstobeDeleted);
            }
        }

        public static void DeleteSAPObjectsByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAPHANA_Objects] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQuery(String.Format(query, endpointSystemId));
            }
        }
    }
}
