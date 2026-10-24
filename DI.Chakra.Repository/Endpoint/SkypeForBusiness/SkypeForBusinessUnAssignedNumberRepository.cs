using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SkypeForBusinessUnAssignedNumberRepository : RepositoryBase<SkypeForBusinessUnAssignedNumberMatrix>
    {
        public SkypeForBusinessUnAssignedNumberRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public int GetUnAssignedRandomNumber(long endpointsystemid)
        {
            var query = $"SELECT TOP 1 Number FROM DI_SFB_UnAssignedNumberMatrix WHERE endpointsystemid={endpointsystemid} AND ASSIGNED=0 AND  RESERVED = 0   ORDER BY  NEWID()";
            var number = CurrentContext.Database.SqlQuery<int>(query);
            return number.FirstOrDefault();
        }
        public void UpdateNumber(long endpointsystemid, int number, bool assigned = false)
        {
            var isAssigned = assigned ? 1 : 0;
            var query = $"UPDATE DI_SFB_UnAssignedNumberMatrix Set ASSIGNED={isAssigned},RESERVED = CASE WHEN {isAssigned}=1 THEN 0 ELSE RESERVED END,LastModifiedDate=GETDATE() WHERE endpointsystemid={endpointsystemid} AND NUMBER={number}";
            CurrentContext.Database.ExecuteSqlCommand(query);
        }
        public void UpdateReservedNumber(long endpointsystemid, int number, bool reserved = false)
        {
            var isReserved = reserved ? 1 : 0;
            var query = $"UPDATE DI_SFB_UnAssignedNumberMatrix Set RESERVED={isReserved}  WHERE endpointsystemid={endpointsystemid} AND NUMBER={number}";
            CurrentContext.Database.ExecuteSqlCommand(query);
        }
        public void DeleteNumberRanges(int priority)
        {
            var query = $"DELETE FROM DI_SFB_UnAssignedNumberMatrix WHERE Priority={priority}";
            CurrentContext.Database.ExecuteSqlCommand(query);
        }
        public Dictionary<long, string> GetAllReservedNumbers(long endpointId, string searchString)
        {
            var returnValue = new Dictionary<long, string>();
            var datas = from matrixData in CurrentContext.SkypeForBusinessUnAssignedNumberMatrix
                        from users in CurrentContext.SkypeForBusinessUserInfo
                        where matrixData.EndPointSystemId == endpointId && matrixData.Reserved && matrixData.Assigned ==false
                              && matrixData.Number != users.LineURIExtension
                        select matrixData;
            var existing = CurrentContext.SkypeForBusinessUserInfo.Where(i => i.LineURIExtension != null).Select(i => i.LineURIExtension);
            datas = datas.Where(i=>!existing.Contains(i.Number));
            int num;
            if (!string.IsNullOrWhiteSpace(searchString) && int.TryParse(searchString,out num))
            {
                datas = datas.Where(i => i.Number.ToString().Contains(num.ToString()));
            }
            if (datas.Any())
            {
                var tempResults = datas.Select((element) => new { Key = element.Number, Value = element.Number.ToString() }).Distinct().ToList();
                foreach (var item in tempResults)
                {
                    returnValue.Add(item.Key, item.Value);
                }
            }
            return returnValue;
        }

        public List<SkypeForBusinessUnAssignedNumberMatrix> GetSFBExtensionMatrixData(long endpointId)
        {
            var script = $@"SELECT	UAN.[Id], UAN.[Number], UAN.[Priority], UAN.[Assigned], 
                            CASE WHEN ISNULL(SFB.ID,'')='' THEN '' ELSE SFB.DisplayName+'('+ SFB.SamAccountName+')' END as CreatedBy
                            , UAN.[CreatedDate], UAN.[LastModifiedBy], UAN.[LastModifiedDate], 
                            UAN.[EndPointSystemId], UAN.[Reserved]
                            FROM	DI_SFB_UnAssignedNumberMatrix AS UAN LEFT JOIN 
                            (
                                SELECT ID,UserType,LineURIExtension,DisplayName,SamAccountName FROM  DI_SFB_UserInfo WHERE EndPointSystemId={endpointId} AND UserType <> {(int)EntitlementType.DCC} 
                            ) AS SFB  ON UAN.Number = SFB.LineURIExtension
                            WHERE  UAN.EndPointSystemId={endpointId}";
            return this.CurrentContext.Database.SqlQuery<SkypeForBusinessUnAssignedNumberMatrix>(script).ToList();
        }
    }
}
