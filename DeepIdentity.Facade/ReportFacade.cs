using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.ReportData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Facade
{
    public class ReportFacade
    {
        private UserDA UserDA = new UserDA();

        public IList<AllUsersReportData> GetAllUsersReport(string ipSelectType)
        {
            return UserDA.GetAllUsersReport(ipSelectType);
        }
        public IList<UserAccessDetailMatrixGridData> GetUserAccessDetailMatrixGridData(int userId)
        {
            try
            {
                return UserDA.GetUserAccessDetailMatrixGridData(userId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<UsersByGroupReportData> getUsersByGroupReportList()
        {
            try
            {
                return UserDA.getUsersByGroupReportList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<UsersByDepartmentReportData> getUsersByDepartmentReportList()
        {
            try
            {
                return UserDA.getUsersByDepartmentReportList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<UsersByManagerReportData> getUsersByManagerReportList()
        {
            try
            {
                return UserDA.getUsersByManagerReportList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<UsersByManagerReportData> getUsersByWithoutManagerReportList()
        {
            try
            {
                return UserDA.getUsersByWithoutManagerReportList();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<UserPasswordCountGridData> getUserPasswordCountReportData(string userName)
        {
            return UserDA.getUserPasswordCountReportData(userName);
        }

        public IList<UserListGridData> getUsersListByAttestation(int attestationId)
        {
            return UserDA.getUsersListByAttestation(attestationId);
        }
        public List<DeprovisioningUserDetailsGridData> GetDeprovisioningUserDetailsGridDataByUserId(int UserId)
        {
            return UserDA.GetDeprovisioningUserDetailsGridDataByUserId(UserId);

        }

        public IList<UserAccessDetailMatrixGridData> getUserAccessDetailsReport(int _endPointSystemTypeId, int _endPointSystemId, int _managerId)
        {
            try
            {
                return UserDA.getUserAccessDetailsReport(_endPointSystemTypeId, _endPointSystemId, _managerId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<UserAccessDetailMatrixGridData> getUserGroupRoleAccessMatrixDetailsReport(int _endPointSystemTypeId, int _endPointSystemId, int _managerId)
        {
            try
            {
                return UserDA.getUserGroupRoleAccessMatrixDetailsReport(_endPointSystemTypeId, _endPointSystemId, _managerId);
            }
            catch (Exception)
            {

                throw;
            }
        }
         public List<DeptChangedUserGridData> GetDeptChangedUserGridData(List<User> CrntUserList)
        {
            try
            {
                return UserDA.GetDeptChangedUserGridData(CrntUserList);
            }
            catch (Exception)
            {
                throw;
            }
        }
         public List<TaskDelegationReportData> GetTaskDelegationsForReport()
         {
             try
             {
                 return UserDA.GetTaskDelegationsForReport();
             }
             catch (Exception)
             {
                 throw;
             }
         }

         public List<TaskDelegationReportData> GetCurrentTaskDelegationsForReport()
         {
             try
             {
                 return UserDA.GetCurrentTaskDelegationsForReport();
             }
             catch (Exception)
             {
                 throw;
             }
         }
         public IList<MyRequestsGridData> GetAdhocRequestsByProgressStatus(AccessRequestProgress progressStatus)
         {
             try
             {
                 return UserDA.GetAdhocRequestsByProgressStatus(progressStatus);
             }
             catch
             {
                 throw;
             }
         }

         public IList<MyRequestsGridData> GetSystemRequestsByProgressStatus(AccessRequestProgress progressStatus)
         {
             try
             {
                 return UserDA.GetSystemRequestsByProgressStatus(progressStatus);
             }
             catch
             {
                 throw;
             }
         }
    }
}
