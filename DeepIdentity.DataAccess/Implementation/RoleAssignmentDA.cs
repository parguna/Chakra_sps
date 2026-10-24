using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using System.Data;
using System.Data.SqlClient;
using DeepIdentity.Infrastructure;

namespace DeepIdentity.DataAccess
{
    public class RoleAssignmentDA 
    {
    //    public IACMDbOperator m_dbOperator = null;
    //    private IACMDBContext oIACMDBContext = new IACMDBContext();

    //    public RoleAssignmentDA()
    //    {

    //    }

    //    #region IRoleDA Members

    //    public IList<RoleAssignment> GetAllRoleAssignments()
    //    {
    //        //string query = "DI_spGetAllRoleAssignments";
    //        IList<RoleAssignment> list = new List<RoleAssignment>();
    //        try
    //        {
    //            using (IACMDBContext oIACMDBContext = new IACMDBContext())
    //            {
    //                list = oIACMDBContext.RoleAssignments.ToList();
    //            }
    //            return list;
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getitng all role assignments.", ex);
    //            throw;
    //        }
    //    }

    //    public bool AddRoleAssignmentsForUsers(IList<RoleAssignment> _roleAssignmentList)
    //    {
    //        bool isDataSaved = false;
    //        //string query = "DI_spAddRoleAssignment";
    //        try
    //        {
    //            foreach (RoleAssignment _roleAssignment in _roleAssignmentList)
    //            {
    //                DeleteRoleAssignment(_roleAssignment.RoleId, _roleAssignment.UserId, false, _roleAssignment.LastModifiedBy);
    //            }

    //            using (IACMDBContext oIACMDBContext = new IACMDBContext())
    //            {
    //                foreach (RoleAssignment _roleAssignment in _roleAssignmentList)
    //                {
    //                    IList<RoleAssignment> tmpRoleAssignment = oIACMDBContext.RoleAssignments.Where(ra => ra.RoleId == _roleAssignment.RoleId && (ra.UserId == _roleAssignment.UserId || _roleAssignment.IsGroupAssignment == true) && (ra.GroupId == _roleAssignment.GroupId || _roleAssignment.IsGroupAssignment == false)).ToList();
    //                    if (tmpRoleAssignment.Count == 0)
    //                    {
    //                        oIACMDBContext.RoleAssignments.Add(_roleAssignment);
    //                        oIACMDBContext.SaveChanges();
    //                        isDataSaved = true;
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            isDataSaved = false;
    //            Log.General.WriteMessage(LogLevel.ERROR, "Error occur while adding new role assignment.", ex);
    //            throw;
    //        }
    //        isDataSaved = true;
    //        return isDataSaved;
    //    }

    //    public bool AddRoleAssignmentsForGroup(RoleAssignment _roleAssignment)
    //    {
    //        bool isDataSaved = false;
    //        try
    //        {
    //            if (_roleAssignment != null)
    //            {
    //                DeleteRoleAssignment(_roleAssignment.RoleId, -1, true, _roleAssignment.LastModifiedBy);
    //                using (IACMDBContext oIACMDBContext = new IACMDBContext())
    //                {
    //                    IList<RoleAssignment> tmpRoleAssignment = oIACMDBContext.RoleAssignments.Where(ra => ra.RoleId == _roleAssignment.RoleId && (ra.UserId == _roleAssignment.UserId || _roleAssignment.IsGroupAssignment == true) && (ra.GroupId == _roleAssignment.GroupId || _roleAssignment.IsGroupAssignment == false)).ToList();
    //                    if (tmpRoleAssignment.Count == 0)
    //                    {
    //                        oIACMDBContext.RoleAssignments.Add(_roleAssignment);
    //                        oIACMDBContext.SaveChanges();
    //                        isDataSaved = true;
    //                    }
    //                }
    //            }               
    //        }
    //        catch (Exception ex)
    //        {
    //            isDataSaved = false;
    //            Log.General.WriteMessage(LogLevel.ERROR, "Error occur while adding new role assignment for Group.", ex);
    //            throw;
    //        }
    //        isDataSaved = true;
    //        return isDataSaved;
    //    }

    //    public int DeleteRoleAssignment(Nullable<int> _roleID, Nullable<int> _userId, bool _isGroupDelete, string userName) //(int _roleID, int _userId, // TODO: C //
    //    {
    //        try
    //        {
    //            IList<RoleAssignment> delRoleAssignment = new List<RoleAssignment>();
    //            using (IACMDBContext oIACMDBContext = new IACMDBContext())
    //            {
    //                if (_isGroupDelete == true)
    //                    delRoleAssignment = oIACMDBContext.RoleAssignments.Where(ra => ra.RoleId == _roleID && ra.GroupId != null).ToList();
    //                else
    //                    delRoleAssignment = oIACMDBContext.RoleAssignments.Where(ra => ra.RoleId == _roleID && ra.UserId == _userId && ra.GroupId == null).ToList();

    //                if (delRoleAssignment.Count > 0)
    //                {
    //                    foreach (RoleAssignment roleAssignment in delRoleAssignment)
    //                    {
    //                       roleAssignment.LastModifiedBy = userName;
    //                        roleAssignment.LastModifiedDate = DateTime.Now;
    //                        oIACMDBContext.RoleAssignments.Remove(roleAssignment);
    //                    }
    //                    oIACMDBContext.SaveChanges();
    //                }
    //                int tmpRowCnt = oIACMDBContext.RoleAssignments.Count();
    //                return tmpRowCnt;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting role assignment.", ex);
    //            throw;
    //        }
    //    }

    //    public IList<User> GetUsersByDIRoleId(int _roleId)
    //    {
    //        //string query = "DI_spGetUsersByDIRoleId";
    //        IList<User> list = new List<User>();
    //        try
    //        {
    //            using (IACMDBContext oIACMDBContext = new IACMDBContext())
    //            {
    //                var query = from usr in oIACMDBContext.Users
    //                            join ra in oIACMDBContext.RoleAssignments on usr.UserId equals ra.UserId
    //                            where ra.RoleId == _roleId && ra.GroupId == null
    //                            select usr;

    //                if (query != null)
    //                {
    //                    if (query.Count() > 0) list = query.ToList();
    //                }
    //            }
    //            return list;
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting users by DIRoleID.", ex);
    //            throw;
    //        }
    //    }

    //    public bool CheckUserExistInAdminOrManagerRole(int userid, int grpId)
    //    {
    //        IList<RoleAssignment> list = new List<RoleAssignment>();
    //        bool isExist = false;
    //        try
    //        {
    //            using (IACMDBContext oIACMDBContext = new IACMDBContext())
    //            {
    //                list = oIACMDBContext.RoleAssignments.Where(ra=>(ra.RoleId == 1 || ra.RoleId == 2) && (ra.UserId == userid || ra.GroupId == grpId)).ToList();
    //            }
    //            if (list.Count > 0)
    //                isExist = true;
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getitng role assignments.", ex);
    //            throw;
    //        }
    //        return isExist;
    //    }

    //    public void SaveGroup(RoleAssignment _RoleAssignment)
    //    {
    //        try
    //        {                
    //            DeleteRoleAssignment(_RoleAssignment.RoleId, _RoleAssignment.UserId, false, _RoleAssignment.LastModifiedBy);

    //            using (IACMDBContext oIACMDBContext = new IACMDBContext())
    //            {                   
    //                IList<RoleAssignment> tmpRoleAssignment = oIACMDBContext.RoleAssignments.Where(ra => ra.RoleId == _RoleAssignment.RoleId && (ra.UserId == _RoleAssignment.UserId || _RoleAssignment.IsGroupAssignment == true) && (ra.GroupId == _RoleAssignment.GroupId || _RoleAssignment.IsGroupAssignment == false)).ToList();
    //                if (tmpRoleAssignment.Count == 0)
    //                {
    //                    oIACMDBContext.RoleAssignments.Add(_RoleAssignment);
    //                    oIACMDBContext.SaveChanges();                            
    //                }                  
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Assigning the Role", ex);
    //            throw ex;
    //        }
    //    }

    //    #endregion

    }
}
