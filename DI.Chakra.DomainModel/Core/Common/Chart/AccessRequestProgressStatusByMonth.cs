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
    public class AccessRequestProgressStatusByMonth : DomainModelBase<AccessRequestProgressStatusByMonth, EntityModel.AccessRequestProgressStatusByMonth>
    {
        public AccessRequestProgress Name { get; set; }
        public decimal Y { get; set; }
        public AccessRequestProgress LabelName { get; set; }

        public static DataTable CreateDataForChart(IList<AccessRequestProgressStatusByMonth> chartData)
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
            RequestTypes.Add("None");
            RequestTypes.Add("WaitingforApproval");
            RequestTypes.Add("RequestRejected");
            RequestTypes.Add("ReadytoExecute");
            RequestTypes.Add("Executing");
            RequestTypes.Add("Completed");
            RequestTypes.Add("ExecutionFailed");
            RequestTypes.Add("ValidationFailed");
            RequestTypes.Add("Withdrawn");
            RequestTypes.Add("ADIDConstructionFailed");
            RequestTypes.Add("WaitingforImplementation");

            ArrayList objDataColumn = new ArrayList();

            if (chartData.Count() > 0)
            {
                for (int i = 0; i < RequestTypes.Count; i++)
                {
                    objDataColumn.Add(RequestTypes[i]);
                }
            }

            for (int i = 0; i < objDataColumn.Count; i++)
            {
                dt.Columns.Add(objDataColumn[i].ToString());
            }
            var row = dt.NewRow();
            for (int i = 0; i < d.Count; i++)
            {
                var res = d[i].Name.ToList();
                for (int j = 0; j < res.Count; j++)
                {
                    var col = res[j].Sub.ToString();
                    if (dt.Columns.IndexOf(col) >= 0)
                    {
                        row[col] = res[j].Count.ToString();
                    }
                }                
            }

            if(d.Count > 0)
                dt.Rows.Add(row);

            return dt;
        }

        public static DataTable GetAccessRequestProgressStatusByUser(int Year, int Month, long UserId)
        {
            IList<AccessRequestProgressStatusByMonth> query;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                query = ORMapList(new AccessRequestProgressStatusByMonthRepository(dataAccessTransaction).GetAccessRequestProgressStatusByUser(Year, Month, UserId).ToList());

                return CreateDataForChart(query);
            }
        }
        public static DataTable GetAccessRequestProgressStatusByAdmin(int Year, int Month)
        {
            IList<AccessRequestProgressStatusByMonth> query;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                query = ORMapList(new AccessRequestProgressStatusByMonthRepository(dataAccessTransaction).GetAccessRequestProgressStatus(Year, Month).ToList());

                return CreateDataForChart(query);
            }
        }
    }
}
