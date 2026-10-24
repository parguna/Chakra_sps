using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace DeepIdentity.DataAccess
{
    public class DashboardDA
    {

        public List<Task> GetAttestationTasksByDays(DateTime fromDate)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Tasks.Where(item => item.AttestationId.HasValue && item.CreatedDate >= fromDate).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Task> GetTaskStatusByUserId(int userId, DateTime fromDate)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (userId == 0)
                    {
                        return _context.Tasks.Where(item => item.CreatedDate >= fromDate).ToList();
                    }
                    return _context.Tasks.Where(item => item.ActionUserId == userId && item.CreatedDate >= fromDate).ToList();
                }
            }
            catch
            {
                throw;
            }            
        }

        public List<AccessRequest> GetRequestStatusByUserId(int userId, DateTime fromDate)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (userId == 0)
                    {
                        return _context.AccessRequests.Where(item => item.CreatedDate >= fromDate).ToList();
                    }
                    return _context.AccessRequests.Where(item => item.UserId == userId && item.CreatedDate >= fromDate).ToList();
                }
            }
            catch
            {
                throw;
            } 
        }
        public IList<Dashboard> GetDashboardsByUserRole(int RoleId)
        {
            IList<Dashboard> menuList = new List<Dashboard>();
            try
            {
                if (RoleId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        var query = from DR in oIACMDBContext.DashboardAccessRights
                                    join r in oIACMDBContext.Roles
                                on DR.RoleId equals r.RoleId
                                    join D in oIACMDBContext.Dashboards
                                on DR.DashboardId equals D.Id
                                    where r.RoleId == RoleId && D.TargetUrl != null
                                    select D;

                        menuList = query.ToList();
                    }
                }
                return menuList;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetDashboardsByUserRole");
                throw;
            }
        }
        public IList<Dashboard> GetDashboardByUserRole(int RoleId)
        {
            IList<Dashboard> dashboardList = new List<Dashboard>();
            try
            {
                if (RoleId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        var query = from DR in oIACMDBContext.DashboardAccessRights
                                    join r in oIACMDBContext.Roles
                                on DR.RoleId equals r.RoleId
                                    join d in oIACMDBContext.Dashboards
                                on DR.DashboardId equals d.Id
                                    where r.RoleId == RoleId && d.TargetUrl != null
                                    select d;

                        dashboardList = query.ToList();
                    }
                }
                return dashboardList;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetDashboardByUserRole");
                throw;
            }
        }
        public IList<DashBoardPreference> GetUserDashboards(int UserId)
        {
            IList<DashBoardPreference> dashboardList = new List<DashBoardPreference>();
            try
            {
                if (UserId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        var query = from DR in oIACMDBContext.UserDashboardPreferences.Include("Dashboard")
                                    join D in oIACMDBContext.Dashboards
                                    on DR.DashboardId equals D.Id
                                    where DR.UserId == UserId
                                    select new DashBoardPreference
                                    {
                                        DashboardId = DR.Id,
                                        //Title = DR.Dashboard.Title,
                                        DashboardName = DR.Name,
                                        DashboardCategory = D.DashboardCategory,
                                        TargetUrl = D.TargetUrl,
                                        NavigateUrl = D.NavigateUrl,
                                        Position = DR.Position
                                    };
                        dashboardList = query.OrderBy(p => p.Position).ToList();
                    }
                }
                return dashboardList;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetUserDashboards");
                throw;
            }
        }
        public void SaveUserDashboard(UserDashboardPreference dashboard)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (dashboard.Id == 0)
                    {
                        oIACMDBContext.UserDashboardPreferences.Add(dashboard);
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        UserDashboardPreference objDashboard = new UserDashboardPreference();
                        objDashboard = oIACMDBContext.UserDashboardPreferences.Find(dashboard.Id);
                        oIACMDBContext.Entry(objDashboard).CurrentValues.SetValues(dashboard);
                        oIACMDBContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
        }
        public IList<DashBoardPreference> GetUserPreferenceDashboards(int UserId)
        {

            IList<DashBoardPreference> menuList = new List<DashBoardPreference>();
            try
            {
                if (UserId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        var query = from DP in oIACMDBContext.UserDashboardPreferences
                                    join D in oIACMDBContext.Dashboards
                                on DP.DashboardId equals D.Id
                                    where DP.UserId == UserId
                                    orderby DP.Position
                                    select new DashBoardPreference
                                    {
                                        DashboardName = D.DashboardName,
                                        DashboardId = D.Id,
                                        TargetUrl = D.TargetUrl,
                                        Position = DP.Position
                                    };

                        menuList = query.ToList();
                    }
                }
                return menuList;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetUserPreferenceMenus");
                throw;
            }
        }
        public IList<int> GetAllYears()
        {
            IList<int> years = new List<int>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    years = oIACMDBContext.AccessRequests.Where(m => m.ActivationDate != null).Select(m => m.ActivationDate.Value.Year).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return years;
        }
        public IList<int> GetUserYears(int UserId)
        {
            IList<int> years = new List<int>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    years = oIACMDBContext.AccessRequests.Where(m => m.ActivationDate != null && m.UserId == UserId).Select(m => m.ActivationDate.Value.Year).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return years;
        }
        public IList<AccessRequestSourcePieSeries> GetPieChartData()
        {

            IList<AccessRequestSourcePieSeries> chartData = new List<AccessRequestSourcePieSeries>();
            try
            {
                //if (UserId != -1)
                //{
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var query = from UQ in oIACMDBContext.AccessRequests
                                where UQ.RequestSource != null
                                group UQ by UQ.RequestSource into g
                                let count = g.Count()
                                select new AccessRequestSourcePieSeries
                                {
                                    Y = count,
                                    Name = g.Key
                                };

                    chartData = query.ToList();
                    //}
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetPieChartData");
                throw;
            }
        }
        public DataTable GetAccessRequestSourceByYear(int Year)
        {
            IList<AccessRequestSourceByMonth> chartData = new List<AccessRequestSourceByMonth>();
            DataTable dt = new DataTable();
            try
            {
                //if (UserId != -1)
                //{
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var query = from UQ in oIACMDBContext.AccessRequests
                                where UQ.ActivationDate.Value != null && UQ.ActivationDate.Value.Year == Year
                                && UQ.RequestSource != null
                                group UQ by new { UQ.ActivationDate.Value.Month, UQ.RequestSource } into g
                                let count = g.Count()
                                select new AccessRequestSourceByMonth
                                {
                                    Y = count,
                                    Name = g.Key.RequestSource,
                                    LabelName = g.Key.Month
                                };

                    chartData = query.ToList();
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
                    RequestTypes.Add("Propagation");

                    ArrayList objDataColumn = new ArrayList();

                    if (chartData.Count() > 0)
                    {
                        objDataColumn.Add("MonthName");
                        for (int i = 0; i < RequestTypes.Count; i++)
                        {
                            objDataColumn.Add(RequestTypes[i]);
                        }
                    }

                    //Add dynamic columns name to datatable dt
                    for (int i = 0; i < objDataColumn.Count; i++)
                    {
                        dt.Columns.Add(objDataColumn[i].ToString());
                    }
                    //Add data into datatable with respect to dynamic columns and dynamic data
                    for (int i = 0; i < d.Count; i++)
                    {
                        List<string> tempList = new List<string>();
                        tempList.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(d[i].LabelName)));

                        var res = d[i].Name.ToList();
                        for (int j = 0; j < res.Count; j++)
                        {
                            tempList.Add(res[j].Count.ToString());
                        }

                        dt.Rows.Add(tempList.ToArray<string>());
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetPieChartData");
                throw;
            }
        }
        public DataTable GetAccessRequestSourceByUser(int Year, int UserId)
        {
            IList<AccessRequestSourceByMonth> chartData = new List<AccessRequestSourceByMonth>();
            DataTable dt = new DataTable();
            try
            {
                //if (UserId != -1)
                //{
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var query = from UQ in oIACMDBContext.AccessRequests
                                where UQ.ActivationDate.Value != null && UQ.ActivationDate.Value.Year == Year
                                && UQ.RequestSource != null && UQ.UserId == UserId
                                group UQ by new { UQ.ActivationDate.Value.Month, UQ.RequestSource } into g
                                let count = g.Count()
                                select new AccessRequestSourceByMonth
                                {
                                    Y = count,
                                    Name = g.Key.RequestSource,
                                    LabelName = g.Key.Month
                                };

                    chartData = query.ToList();
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
                    RequestTypes.Add("Propagation");

                    ArrayList objDataColumn = new ArrayList();

                    if (chartData.Count() > 0)
                    {
                        objDataColumn.Add("MonthName");
                        for (int i = 0; i < RequestTypes.Count; i++)
                        {
                            objDataColumn.Add(RequestTypes[i]);
                        }
                    }

                    //Add dynamic columns name to datatable dt
                    for (int i = 0; i < objDataColumn.Count; i++)
                    {
                        dt.Columns.Add(objDataColumn[i].ToString());
                    }
                    //Add data into datatable with respect to dynamic columns and dynamic data
                    for (int i = 0; i < d.Count; i++)
                    {
                        List<string> tempList = new List<string>();
                        tempList.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(d[i].LabelName)));

                        var res = d[i].Name.ToList();
                        for (int j = 0; j < res.Count; j++)
                        {
                            tempList.Add(res[j].Count.ToString());
                        }

                        dt.Rows.Add(tempList.ToArray<string>());
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetPieChartData");
                throw;
            }
        }
        public IList<AccessRequestSourceChartData> GetSeriesDataByAccessRequestSource(AccessRequestSource RequestSource)
        {

            IList<AccessRequestSourceChartData> chartData = new List<AccessRequestSourceChartData>();
            try
            {
                //if (UserId != -1)
                //{
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var query = from AR in oIACMDBContext.AccessRequests
                                join U in oIACMDBContext.Users
                                on AR.UserId equals U.UserId
                                where AR.RequestSource == RequestSource
                                select new AccessRequestSourceChartData
                                {
                                    Description = AR.Description,
                                    UserName = U.UserName,
                                    RequestStatus = AR.RequestStatus,
                                    ProgressStatus = AR.ProgressStatus
                                };

                    chartData = query.ToList();
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetSeriesDataByAccessRequestSource");
                throw;
            }
        }
        public IList<AccessRequestSourceChartData> GetAccessRequestSourceByMonth(int Year, int Month, AccessRequestSource RequestSource)
        {

            IList<AccessRequestSourceChartData> chartData = new List<AccessRequestSourceChartData>();
            try
            {
                //if (UserId != -1)
                //{
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var query = from AR in oIACMDBContext.AccessRequests
                                join U in oIACMDBContext.Users
                                on AR.UserId equals U.UserId
                                where AR.RequestSource == RequestSource
                                && AR.ActivationDate != null && AR.ActivationDate.Value.Year == Year
                                && AR.ActivationDate.Value.Month == Month
                                select new AccessRequestSourceChartData
                                {
                                    Description = AR.Description,
                                    UserName = U.UserName,
                                    RequestStatus = AR.RequestStatus,
                                    ProgressStatus = AR.ProgressStatus
                                };

                    chartData = query.ToList();
                }
                return chartData;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetAccessRequestSourceByMonth");
                throw;
            }
        }
        public DashboardType GetDashboardType(int DashboardId)
        {
            DashboardType type = 0;
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                Dashboard board = oIACMDBContext.Dashboards.Where(m => m.Id == DashboardId).FirstOrDefault();
                if (board != null)
                {
                    type = board.DashBoardType;
                }
            }
            return type;
        }
        public void DeleteUserDashboard(int dashBoardId, int UserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (UserId != 0)
                    {
                        UserDashboardPreference link = oIACMDBContext.UserDashboardPreferences.Where(UQ => UQ.DashboardId == dashBoardId && UQ.UserId == UserId).FirstOrDefault();
                        oIACMDBContext.UserDashboardPreferences.Remove(link);
                        oIACMDBContext.SaveChanges();
                    }

                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
        }
        public IList<DashBoardPreference> GetDefaultDashboards(int RoleId)
        {
            IList<DashBoardPreference> dashboardList = new List<DashBoardPreference>();
            try
            {
                if (RoleId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        var query = from drp in oIACMDBContext.DefaultRoleDashboardPreferences
                                    join r in oIACMDBContext.Roles
                                on drp.RoleId equals r.RoleId
                                    join D in oIACMDBContext.Dashboards
                                    on drp.DashBoardId equals D.Id
                                    where drp.RoleId == RoleId
                                    orderby drp.Position
                                    select new DashBoardPreference
                                    {
                                        DashboardName = D.DashboardName,
                                        //Title = D.Title,
                                        DashboardId = D.Id,
                                        TargetUrl = D.TargetUrl,
                                        NavigateUrl = D.NavigateUrl,
                                        DashboardCategory = D.DashboardCategory,
                                        Position = drp.Position
                                    };

                        dashboardList = query.ToList();
                    }
                }
                return dashboardList;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetUserDefaultPreferenceDashboard");
                throw;
            }
        }
       
    }
}
