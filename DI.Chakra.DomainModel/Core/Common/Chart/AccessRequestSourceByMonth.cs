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
    public class AccessRequestSourceByMonth : DomainModelBase<AccessRequestSourceByMonth, EntityModel.AccessRequestSourceByMonth>
    {
        public AccessRequestSource Name { get; set; }
        public decimal Y { get; set; }
        public decimal LabelName { get; set; }

        public static DataTable GetAccessRequestSourceForUserByYear(int year, long userId)
        {
            IList<AccessRequestSourceByMonth> chartData = null;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                chartData = GetAccessRequestSourceForUser(year, userId);
                return CreateDataForChart(chartData);
            }
        }

        public static DataTable CreateDataForChart(IList<AccessRequestSourceByMonth> chartData)
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
            RequestTypes.Add("AdhocRequest");
            RequestTypes.Add("OnBoarding");
            RequestTypes.Add("OffBoarding");
            RequestTypes.Add("OUChange");
            RequestTypes.Add("ReOrganisation");
            RequestTypes.Add("EmployeeExtension");
            RequestTypes.Add("AttestationRemediation");
            RequestTypes.Add("EmployeeCreation");
            RequestTypes.Add("Rehire");
            RequestTypes.Add("Promotion");
            RequestTypes.Add("ExtendedRequest");
            RequestTypes.Add("GamingApplicationRemoval");
            RequestTypes.Add("OffBoardingDay2");
            RequestTypes.Add("OffBoardingDay3");
            RequestTypes.Add("Demotion");

            ArrayList objDataColumn = new ArrayList();

            if (chartData.Count() > 0)
            {
                objDataColumn.Add("MonthName");
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
                row["MonthName"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(d[i].LabelName));

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

        public static DataTable GetAccessRequestSourceByYear(int year)
        {
            IList<AccessRequestSourceByMonth> chartData = null;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                chartData = GetAccessRequestSource(year);
                return CreateDataForChart(chartData);
            }
        }
        public static IList<AccessRequestSourceByMonth> GetAccessRequestSource(int year)  
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AccessRequestSourceByMonthRepository(transaction).GetAccessRequestSource(year));
            }
        }

        public static IList<AccessRequestSourceByMonth> GetAccessRequestSourceForUser(int year, long userId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AccessRequestSourceByMonthRepository(transaction).GetAccessRequestSourceForUser(year, userId));
            }
        }
    }
}
