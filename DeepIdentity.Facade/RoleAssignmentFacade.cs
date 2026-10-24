using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using DeepIdentity.Infrastructure;
using DeepIdentity.DataAccess;

namespace DeepIdentity.Facade
{
    public class RoleAssignmentFacade
    {
        //RoleAssignmentDA roleAssignmentDA = null;

        //public RoleAssignmentFacade()
        //{
        //    roleAssignmentDA = new RoleAssignmentDA();
        //}

        //#region RoleFacade Members

        //public IList<RoleAssignment> GetAllRoleAssignment()
        //{
        //    try
        //    {
        //        return roleAssignmentDA.GetAllRoleAssignments();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public RoleAssignment GetRoleAssignment(int _id)
        //{
        //    try
        //    {
        //        return this.GetByKey(_id);
        //    }
        //    catch 
        //    {

        //        throw;
        //    }
        //}

        //public bool AddRoleAssignmentsForUsers(IList<RoleAssignment> _roleAssignmentList)
        //{
        //    return roleAssignmentDA.AddRoleAssignmentsForUsers(_roleAssignmentList);
        //}
       
        //public bool AddRoleAssignmentsForGroup(RoleAssignment _roleAssignment)
        //{
        //    return roleAssignmentDA.AddRoleAssignmentsForGroup(_roleAssignment);
        //}

        //public int DeleteRoleAssignment(Nullable<int> _roleID, Nullable<int> _userId, bool _isGroupDelete, string userName) //(int _roleID, int _userId, // TODO: C //
        //{
        //    return roleAssignmentDA.DeleteRoleAssignment(_roleID, _userId, _isGroupDelete, userName);
        //}

        //public IList<User> GetUsersByDIRoleId(int _roleId)
        //{
        //    return roleAssignmentDA.GetUsersByDIRoleId(_roleId);
        //}

      
        //#endregion
        //public RoleAssignment GetGroupByRole(int roleId)
        //{
        //    try
        //    {
        //        var _list = GetAllRoleAssignment();
        //        var _role = _list.Where(role => role.RoleId == roleId && role.GroupId!=null).ToList();

        //        if (_role.Count() <= 0)
        //            return null;

        //        return (RoleAssignment)_role.First();

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public RoleAssignment GetByKey(object _key)
        //{
        //    try
        //    {
        //        int _id = Convert.ToInt32(_key);

        //        var _list = GetAll();

        //        var _roleAssignment = _list.Where(roleAssignment => roleAssignment.Id == _id).ToList();

        //        if (_roleAssignment.Count() <= 0)
        //            return null;

        //        return (RoleAssignment)_roleAssignment.First();

        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public IList<RoleAssignment> GetAll()
        //{
        //    try
        //    {
        //        return roleAssignmentDA.GetAllRoleAssignments();
        //    }
        //    catch
        //    {

        //        throw;
        //    }
        //}

        //public bool CheckUserExistInAdminOrManagerRole(int userid, int grpId)
        //{
        //    try
        //    {
        //        return roleAssignmentDA.CheckUserExistInAdminOrManagerRole(userid, grpId);
        //    }
        //    catch
        //    {

        //        throw;
        //    }
        //}
        
        //public void SaveGroup(RoleAssignment _RoleAssignment)
        //{
        //    try
        //    {
        //        roleAssignmentDA.SaveGroup(_RoleAssignment);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
