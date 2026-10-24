using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;
using DI.Chakra.Infrastructure;
using System.Data.Entity;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
namespace DI.Chakra.DataAccess
{
    public class GroupDA //: IGroupDA
    {
        private IACMDBContext oIACMDBContext = new IACMDBContext();

        #region IGroupDA Members

        private IACMDbOperator m_dbOperator = null;
        private IACMDbOperator iacmOperator = null;
        QueryExecutor executor = null;

        public IList<Group> GetAllGroup()
        {
            IList<Group> list = new List<Group>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    list = oIACMDBContext.Groups.Include(lq => lq.Owner).Include(lq => lq.Users).OrderBy(x => x.GroupName).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }
        public Dictionary<long, string> GetAllGroups(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.Groups
                        .Where(item => item.GroupName.Contains(searchString))
                        .ToList()
                        .Select(x => new { Key = x.Id, Value = x.GroupName + " - " + x.GroupDescription })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public Group GetGroupById(long groupId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.Groups
                              .Include("Users")
                            where item.Id == groupId
                            select item).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public string GetGroupNameById(int groupId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Groups.FirstOrDefault(item => item.Id == groupId).GroupName;
                }
            }
            catch
            {

                throw;
            }
        }

       

        public IList<Group> GetAllGroupsByEndpointID(int EndpointSystemId, int EndPointID, string searchString)
        {
            //string query = "DI_spGetAllGroupsByEndpointID";
            Group group;
            IList<Group> list = new List<Group>();

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    EndpointType _endPintSystemTyptName = oIACMDBContext.EndpointSystemTypes.Where(est => est.Id == EndpointSystemId).Select(est => est.EndpointSystemTypeName).First();
                    switch (_endPintSystemTyptName)
                    {
                        case EndpointType.ActiveDirectory:
                            var tmpList1 = (from gi in oIACMDBContext.ADSGroupInfos
                                            where gi.EndPointSystemId == EndPointID && gi.cn.Contains(searchString)
                                            orderby gi.cn
                                            select new { GroupId = gi.Id, GroupName = gi.cn, Description = gi.description }).Distinct();

                            if (tmpList1.Count() > 0)
                            {
                                foreach (var tLst in tmpList1)
                                {
                                    group = new Group();
                                    group.Id = CheckNull.AgainstInt(tLst.GroupId);
                                    group.GroupName = CheckNull.AgainstString(tLst.GroupName);
                                    group.GroupDescription = CheckNull.AgainstString(tLst.Description);
                                    list.Add(group);
                                }
                            }
                            break;
                        case EndpointType.SAP:
                            var tmpList2 = (from gi in oIACMDBContext.SAPGroupInfos
                                            where gi.EndPointSystemId == EndPointID && gi.GROUP_CODE.Contains(searchString)
                                            orderby gi.GROUP_CODE
                                            select new { GroupId = gi.Id, GroupName = gi.GROUP_CODE, Description = gi.GROUP_DESC }).Distinct();

                            if (tmpList2.Count() > 0)
                            {
                                foreach (var tLst in tmpList2)
                                {
                                    group = new Group();
                                    group.Id = CheckNull.AgainstInt(tLst.GroupId);
                                    group.GroupName = CheckNull.AgainstString(tLst.GroupName);
                                    group.GroupDescription = CheckNull.AgainstString(tLst.Description);
                                    list.Add(group);
                                }
                            }
                            break;
                        case EndpointType.LotusNotes:
                            var tmpList3 = (from gi in oIACMDBContext.LotusGroupInfos
                                            where gi.EndPointSystemId == EndPointID && gi.Group.Contains(searchString)
                                            orderby gi.Group
                                            select new { GroupId = gi.Id, GroupName = gi.Group, gi.Description }).Distinct();

                            if (tmpList3.Count() > 0)
                            {
                                foreach (var tLst in tmpList3)
                                {
                                    group = new Group();
                                    group.Id = CheckNull.AgainstInt(tLst.GroupId);
                                    group.GroupName = CheckNull.AgainstString(tLst.GroupName);
                                    group.GroupDescription = CheckNull.AgainstString(tLst.Description);
                                    list.Add(group);
                                }
                            }
                            break;
                        case EndpointType.SAPEnterprisePortal:
                            var tmpList4 = (from gi in oIACMDBContext.SAPEPGroupInfos
                                            where gi.EndPointSystemId == EndPointID && gi.GroupName.Contains(searchString)
                                            orderby gi.GroupName
                                            select new { GroupId = gi.Id, gi.GroupName, Description = gi.GroupDescription }).Distinct();

                            if (tmpList4.Count() > 0)
                            {
                                foreach (var tLst in tmpList4)
                                {
                                    group = new Group();
                                    group.Id = CheckNull.AgainstInt(tLst.GroupId);
                                    group.GroupName = CheckNull.AgainstString(tLst.GroupName);
                                    group.GroupDescription = CheckNull.AgainstString(tLst.Description);
                                    list.Add(group);
                                }
                            }
                            break;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public IList<Group> GetGroupsByUser(long _userId)
        {
            try
            {
                User user = oIACMDBContext.Users.First(usr => usr.Id == _userId);
                List<Group> groups = null;
                if (user != null)
                    groups = user.Groups;
                return groups;
            }
            catch (Exception)
            {
                throw;
            }
        }
        

        public Group GetGroup(int _id)
        {
            Group group = new Group();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    group = oIACMDBContext.Groups.Include(lq => lq.Users).Where(lq => lq.Id == _id).FirstOrDefault();
                }
                return group;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while getting all group.", ex);
                throw;
            }
        }

        public Group GetGroup(string _name)
        {
            throw new NotImplementedException();
        }

        public Group GetGroupByName(string groupName)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Groups.FirstOrDefault(item => item.GroupName.Equals(groupName, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            catch
            {
                throw;
            }
        }
        public void SaveGroup(Group _group)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (_group.Id == 0)
                    {
                        oIACMDBContext.Groups.Attach(_group);
                        oIACMDBContext.Entry(_group).State = EntityState.Added;
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        oIACMDBContext.Database.ExecuteSqlCommand(string.Format("DELETE FROM UserGroup where Group_GroupId={0}", _group.Id));
                        var dbGroup = oIACMDBContext.Groups
                                              .Include(x => x.Users)
                                              .Single(c => c.Id == _group.Id);

                        oIACMDBContext.Entry(dbGroup).CurrentValues.SetValues(_group);
                        foreach (var newUser in _group.Users)
                        {
                            var dbUser = oIACMDBContext.Users.Where(t => t.Id == newUser.Id).FirstOrDefault();
                            if (dbUser != null)
                                dbGroup.Users.Add(dbUser);
                        }
                    }
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
              DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Saving the Group", ex);
                throw ex;
            }
        }
        //public int AddGroup(Group _group)
        //{

        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {

        //            oIACMDBContext.Groups.Attach(g);
        //            oIACMDBContext.Entry(g).State = EntityState.Added;
        //            oIACMDBContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }
        //    return -1;

        //}

        public void AddUserGroup(IList<User> _Usergroup, int _GroupId)
        {
            //string query = "DI_spAddUserGroup";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int noofAffected = -1;
                    User newUser;
                    if (_Usergroup != null && _Usergroup.Count > 0 && _GroupId > -1)
                    {
                        Group targetGroup = oIACMDBContext.Groups.FirstOrDefault(grp => grp.Id == _GroupId);
                        if (targetGroup != null)
                        {
                            foreach (User item in _Usergroup)
                            {
                                newUser = oIACMDBContext.Users.FirstOrDefault(usr => usr.Id == item.Id);
                                if (newUser != null)
                                    targetGroup.Users.Add(newUser);
                            }

                            noofAffected = oIACMDBContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
              DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Inserting the UserGroup", ex);

                throw ex;
            }
        }
        public bool DeleteUserForSelectedRole(int GroupId, User delUser)
        {
            bool isDelete = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    Group SelGroup = oIACMDBContext.Groups.Include("Users").Where(ra => ra.Id == GroupId).FirstOrDefault();

                    SelGroup.Users.Remove(delUser);
                
                    oIACMDBContext.Entry(SelGroup).State = EntityState.Modified;

                    oIACMDBContext.SaveChanges();
                    isDelete = true;
                }
                return isDelete;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while deleting user for selected role.", ex);
                isDelete = false;
                throw;
            }
        }
        public bool DeleteUserGroup(int GroupId, IList<User> _delUsers)
        {
            //string query = "DI_spDeleteUserGroup";
            User removeUser;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    if (_delUsers != null && _delUsers.Count != 0 && GroupId > -1)
                    {
                        Group targetGroup = oIACMDBContext.Groups.FirstOrDefault(grp => grp.Id == GroupId);
                        if (targetGroup != null)
                        {
                            foreach (User usr in _delUsers)
                            {
                                removeUser = oIACMDBContext.Users.FirstOrDefault(aUser => aUser.Id == usr.Id);
                                if (removeUser != null)
                                    targetGroup.Users.Remove(removeUser);
                            }
                            recordsAffected = oIACMDBContext.SaveChanges();
                        }
                    }

                    if (recordsAffected != 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
              DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the UserGroup", ex);
                throw ex;
            }
        }

        public bool DeleteGroup(int GroupId)
        {
            //    string query = "DI_spDeleteGroup";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    Group delGroup = oIACMDBContext.Groups.FirstOrDefault(grp => grp.Id == GroupId);
                    if (delGroup != null)
                    {
                        oIACMDBContext.Groups.Remove(delGroup);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
              DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the Group", ex);
                throw ex;
            }
        }

        //public int UpdateGroup(Group _group)
        //{
        //    using (IACMDBContext _context = new IACMDBContext())
        //    {
        //        var dbGroup = _context.Groups
        //               .Include(x => x.Users)
        //               .Single(c => c.Id == group.Id);

        //        _context.Entry(dbGroup).CurrentValues.SetValues(g);

        //        var v = (from x in dbGroup.Users
        //                 where !_group.Users.Contains(x)
        //                 select x).ToList();

        //        foreach (var item in v)
        //        {
        //            dbGroup.Users.Remove(item);
        //        }


        //        foreach (var newUser in group.Users)
        //        {
        //            var item = dbGroup.Users.SingleOrDefault(s => s.Id == newUser.Id);
        //            if (item != null)
        //                _context.Entry(item).CurrentValues.SetValues(newUser);
        //            else
        //            {
        //                var dbUser = _context.Users.Where(t => t.Id == newUser.Id).FirstOrDefault();

        //                if (dbUser != null)
        //                    dbGroup.Users.Add(dbUser);
        //            }
        //        }

        //        _context.SaveChanges();
        //    }
        //}

        #endregion

        public bool IsGroupExist(string GroupName, int GroupID)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                bool flag = true;
                if (GroupID > 0)
                {
                    var ExistGroupName = GetGroup(GroupID).GroupName;
                    flag = (ExistGroupName != GroupName) ? true : false;
                }
                if (flag)
                {
                    var dbGroup = oIACMDBContext.Groups.Where(c => c.GroupName == GroupName).FirstOrDefault();
                    return dbGroup != null ? true : false;
                }
                return false;
            }
        }
    }
}
