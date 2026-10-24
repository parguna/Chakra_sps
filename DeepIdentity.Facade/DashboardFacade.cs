using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DeepIdentity.Facade
{
    public class DashboardFacade
    {
        DashboardDA dashboardDA = new DashboardDA();

        public decimal GetAttestationHealth(int days)
        {
            try
            {
                DateTime fromDate = DateTime.Now.AddDays(-days);
                var tasks = dashboardDA.GetAttestationTasksByDays(fromDate);

                if (tasks.Count == 0)
                {
                    return 100;
                }
                else
                {
                    return this.CalculateAttestationHealth(tasks);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ChartData> GetTaskStatusByUserId(int userId, int days)
        {
            try
            {
                DateTime fromDate = DateTime.Now.AddDays(-days);

                var tasks = dashboardDA.GetTaskStatusByUserId(userId, fromDate);

                return this.PopulateChartDataForTasks(tasks);
            }
            catch
            {
                throw;
            }
        }

        public List<ChartData> GetRequestStatusByUserId(int userId, int days)
        {
            try
            {
                DateTime fromDate = DateTime.Now.AddDays(-days);

                var requests = dashboardDA.GetRequestStatusByUserId(userId, fromDate);

                return this.PopulateChartDataForRequests(requests);
            }
            catch
            {
                throw;
            }
        }

        private List<ChartData> PopulateChartDataForTasks(List<Task> tasks)
        {
            List<ChartData> list = new List<ChartData>();

            int pendingTaskCount = tasks.Count(item => item.IsCompleted == false && item.IsActive == true);

            ChartData chartData1 = new ChartData()
            {
                Color = ColorConstants.YELLOW,
                Name = "Pending",
                Value = pendingTaskCount,
                Explode = true
            };

            int autoCompletedTaskCount = tasks.Count(item => item.IsCompleted == false && item.IsActive == false);
            ChartData chartData2 = new ChartData()
            {
                Color = ColorConstants.BLUE,
                Name = "AutoCompleted",
                Value = autoCompletedTaskCount,
                Explode = true
            };

            int completedTaskCount = tasks.Count(item => item.IsCompleted == true && item.IsActive == true);
            ChartData chartData3 = new ChartData()
            {
                Color = ColorConstants.GREEN,
                Name = "Completed",
                Value = completedTaskCount,
                Explode = true
            };

            list.Add(chartData1);
            list.Add(chartData2);
            list.Add(chartData3);

            return list;
        }

        private List<ChartData> PopulateChartDataForRequests(List<AccessRequest> requests)
        {
            List<ChartData> list = new List<ChartData>();

            int pendingRequestCount = requests.Count(item => item.RequestStatus == AccessRequestStatus.Requested);

            ChartData chartData1 = new ChartData()
            {
                Color = ColorConstants.YELLOW,
                Name = "Pending",
                Value = pendingRequestCount,
                Explode = true
            };

            int rejectedRequestCount = requests.Count(item => item.RequestStatus == AccessRequestStatus.Rejected);
            ChartData chartData2 = new ChartData()
            {
                Color = ColorConstants.RED,
                Name = "Rejected",
                Value = rejectedRequestCount,
                Explode = true
            };

            int approvedRequestCount = requests.Count(item => item.RequestStatus == AccessRequestStatus.Approved  ||
                                                      item.RequestStatus == AccessRequestStatus.Granted);
            ChartData chartData3 = new ChartData()
            {
                Color = ColorConstants.GREEN,
                Name = "Approved",
                Value = approvedRequestCount,
                Explode = true
            };

            list.Add(chartData1);
            list.Add(chartData2);
            list.Add(chartData3);

            return list;
        }

        private decimal CalculateAttestationHealth(List<Task> tasks)
        {
            int completedTaskCount = tasks.Count(item => item.IsCompleted == true || item.IsActive == false);

            decimal health = Math.Round(((completedTaskCount * (decimal)100) / tasks.Count), 2);

            return health;
        }
        public IList<Dashboard> GetDashboardsByUserRole(int RoleId)
        {
            try
            {

                return dashboardDA.GetDashboardsByUserRole(RoleId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DashboardType GetDashboardType(int DashboardId)
        {
            try
            {
                return dashboardDA.GetDashboardType(DashboardId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Dashboard> GetDashboardByUserRole(int RoleId)
        {
            try
            {

                return dashboardDA.GetDashboardByUserRole(RoleId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<DashBoardPreference> GetUserDashboards(int UserId)
        {
            try
            {
                return dashboardDA.GetUserDashboards(UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<AccessRequestSourcePieSeries> GetPieChartData()
        {
            try
            {
                return dashboardDA.GetPieChartData();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<int> GetAllYears()
        {
            try
            {
                return dashboardDA.GetAllYears();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<int> GetUserYears(int UserId)
        {
            try
            {
                return dashboardDA.GetUserYears(UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable GetAccessRequestSourceByYear(int Year)
        {
            try
            {
                return dashboardDA.GetAccessRequestSourceByYear(Year);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable GetAccessRequestSourceByUser(int Year, int UserId)
        {
            try
            {
                return dashboardDA.GetAccessRequestSourceByUser(Year, UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<AccessRequestSourceChartData> GetSeriesDataByAccessRequestSource(AccessRequestSource RequestSource)
        {
            try
            {
                return dashboardDA.GetSeriesDataByAccessRequestSource(RequestSource);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<AccessRequestSourceChartData> GetAccessRequestSourceByMonth(int Year, int Month, AccessRequestSource RequestSource)
        {
            try
            {
                return dashboardDA.GetAccessRequestSourceByMonth(Year, Month, RequestSource);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void SaveUserDashboard(UserDashboardPreference dashboard)
        {
            try
            {
                dashboardDA.SaveUserDashboard(dashboard);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void DeleteUserDashboard(int dashBoardId, int UserId)
        {
            try
            {
                dashboardDA.DeleteUserDashboard(dashBoardId, UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<DashBoardPreference> GetUserPreferenceDashboards(int UserId)
        {
            try
            {

                return dashboardDA.GetUserPreferenceDashboards(UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<DashBoardPreference> GetDefaultDashboards(int RoleId)
        {
            try
            {

                return dashboardDA.GetDefaultDashboards(RoleId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
