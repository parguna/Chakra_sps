using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANATCodeObjectMap : DomainModelBase<SAPHANATCodeObjectMap, EntityModel.SAPHANATCodeObjectMap>
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


        public static SAPHANATCodeObjectMap GetByCodes(string objectCode, string fieldCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANATCodeObjectMap>(dataAccessTransaction).Get(codeInfo => codeInfo.ObjectCode == objectCode && codeInfo.FieldCode == fieldCode && codeInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static List<SAPHANATCodeObjectMap> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANATCodeObjectMap>(dataAccessTransaction).Get(codeInfo => codeInfo.EndPointSystemId == endPointSystemId && codeInfo.LastModifiedDate < modifiedDate).ToList());
            }
        }

        
        public void RemoveSAPTCodeObjectMap(SAPHANATCodeObjectMap mapInfo)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPHANATCodeObjectMap sapObject = new SAPHANATCodeObjectMap();
                sapObject = GetByCodes(mapInfo.ObjectCode, mapInfo.FieldCode, mapInfo.EndPointSystemId);
                sapObject.Delete(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }


        public static void DeleteSAPTCodeObjectMap(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPHANATCodeObjectMap>(dataAccessTransaction).Get(saptcode => saptcode.EndPointSystemId == endPointSystemId).ToList());
                 DeleteList(ItemstobeDeleted);
             }
        }

        

        public static void SaveSAPTCodeObjectMap(IList<SAPHANATCodeObjectMap> list, string currentUser)
        {
            if (list != null && list.Count > 0)
            {
                try
                {
                    SQLBulkUploader<SAPHANATCodeObjectMap> sqlSAPTCodeObjectMap = null;
                    try
                    {

                        list = list.Distinct().ToList();
                        string tableName = "DI_SAPHANA_TCodeObject";

                        sqlSAPTCodeObjectMap = new SQLBulkUploader<SAPHANATCodeObjectMap>(tableName);

                        sqlSAPTCodeObjectMap.Flush(list);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlSAPTCodeObjectMap?.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //foreach (SAPHANASingleRoleInfo singleRole in list)
                //{
                //    singleRole.Save(currentUser);
                //}
            }
            //foreach (SAPHANATCodeObjectMap tcObj in list)
            //{
            //    if(tcObj!=null)
            //    tcObj.Save(currentUser);
            //}
        }

        public static List<SAPHANATCodeObjectMap> GetByEndpointId(long endPointsystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANATCodeObjectMap>(dataAccessTransaction).
                    Get(codeInfo => codeInfo.EndPointSystemId == endPointsystemId).ToList());
            }
        }

        public static void DeleteTCodesByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAPHANA_TCodeObject] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQuery(String.Format(query, endpointSystemId));
            }
        }
    }
}
