using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class AdhocRequestAccountTyebyMonth : DomainModelBase<AdhocRequestAccountTyebyMonth, EntityModel.AdhocRequestAccountTyebyMonth>
    {
        public AdHocActionType Name { get; set; }
        public decimal Y { get; set; }
        public AccountType LabelName { get; set; }

        public static DataTable GetAccessRequestAccountType(int Year, int Month)
        {
            IList<AdhocRequestAccountTyebyMonth> query;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                query = ORMapList(new AdhocRequestAccountTyebyMonthRepository(dataAccessTransaction).GetAccessRequestAccountType(Year, Month).ToList());

                return CreateDataForChart(query);
            }
        }

        public static DataTable GetUserAccessRequestByAccountType(int Year, int Month, long UserId)
        {        
            IList<AdhocRequestAccountTyebyMonth> query;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                query = ORMapList(new AdhocRequestAccountTyebyMonthRepository(dataAccessTransaction).GetUserAccessRequestByAccountType(Year, Month, UserId).ToList());
              
                return CreateDataForChart(query);
            }            
        }

        public static DataTable CreateDataForChart(IList<AdhocRequestAccountTyebyMonth> chartData)
        {
            DataTable dt = new DataTable();
            var d = (from f in chartData
                     group f by new { f.LabelName }
                             into myGroup
                     where myGroup.Count() > 0
                     select new
                     {
                         myGroup.Key.LabelName,
                         Name = myGroup.GroupBy(f => f.Name).Select
                         (m => new { Sub = m.Key, Count = m.Sum(c => c.Y) })
                     }).ToList();

            List<string> RequestTypes = new List<string>();
            RequestTypes.Add("NewAccount");
            RequestTypes.Add("AmendmentAdd");
            RequestTypes.Add("AmendmentRemove");
            RequestTypes.Add("DeleteAccount");

            ArrayList objDataColumn = new ArrayList();
            if (chartData.Count() > 0)
            {
                objDataColumn.Add("AccountType");
                for (int i = 0; i < RequestTypes.Count; i++)
                {
                    objDataColumn.Add(RequestTypes[i]);
                }
            }
            for (int i = 0; i < objDataColumn.Count; i++)
            {
                dt.Columns.Add(objDataColumn[i].ToString());
            }
            for (int i = 0; i < d.Count; i++)
            {
                var row = dt.NewRow();
                row["AccountType"] = (d[i].LabelName.ToString());

                var res = d[i].Name.ToList();
                for (int j = 0; j < res.Count; j++)
                {
                    var col = res[j].Sub.ToString();
                    if (dt.Columns.IndexOf(col) >= 0)
                    {
                        row[col] = res[j].Count.ToString();
                    }
                }

                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}
