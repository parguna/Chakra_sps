using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using System.Data;
using System.Data.SqlClient;
using DeepIdentity.Infrastructure;
using System.Data.Entity;
using DeepIdentity.Infrastructure.Enums;
namespace DeepIdentity.DataAccess
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
        public Dictionary<int, string> GetAllGroups(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.Groups
                        .Where(item => item.GroupName.Contains(searchString))
                        .ToList()
                        .Select(x => new { Key = x.GroupId, Value = x.GroupName + " - " + x.GroupDescription })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public Group GetGroupById(int groupId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.Groups
                              .Include("Users")
                            where item.GroupId == groupId
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
                    return _context.Groups.FirstOrDefault(item => item.GroupId == groupId).GroupName;
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
                    EndpointType _endPintSystemTyptName = oIACMDBContext.EndpointSystemTypes.Where(est => est.EndpointSystemTypeId == EndpointSystemId).Select(est => est.EndpointSystemTypeName).First();
                    switch (_endPintSystemTyptName)
                    {
                        case EndpointType.ActiveDirectory:
                            var tmpList1 = (from gi in oIACMDBContext.ADSGroupInfos
                                            where gi.EndPointSystemId == EndPointID && gi.cn.Contains(searchString)
                                            orderby gi.cn
                                            select new { gi.GroupId, GroupName = gi.cn, Description = gi.description }).Distinct();

                            if (tmpList1.Count() > 0)
                            {
                                foreach (var tLst in tmpList1)
                                {
                                    group = new Group();
                                    group.GroupId = CheckNull.AgainstInt(tLst.GroupId);
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
                                            select new { GroupId = gi.SAP_GROUPID, GroupName = gi.GROUP_CODE, Description = gi.GROUP_DESC }).Distinct();

                            if (tmpList2.Count() > 0)
                            {
                                foreach (var tLst in tmpList2)
                                {
                                    group = new Group();
                                    group.GroupId = CheckNull.AgainstInt(tLst.GroupId);
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
                                    group.GroupId = CheckNull.AgainstInt(tLst.GroupId);
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
                                    group.GroupId = CheckNull.AgainstInt(tLst.GroupId);
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


        public IList<Group> GetGroupsByUser(int _userId)
        {
            try
            {
                User user = oIACMDBContext.Users.First(usr => usr.UserId == _userId);
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
                    group = oIACMDBContext.Groups.Include(lq => lq.Users).Where(lq => lq.GroupId == _id).FirstOrDefault();
                }
                return group;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting all group.", ex);
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
                    if (_group.GroupId == 0)
                    {
                        oIACMDBContext.Groups.Attach(_group);
                        oIACMDBContext.Entry(_group).State = EntityState.Added;
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        var dbGroup = oIACMDBContext.Groups
                                              .Include(x => x.Users)
                                              .Single(c => c.GroupId == _group.GroupId);

                        oIACMDBContext.Entry(dbGroup).CurrentValues.SetValues(_group);

                        var v = (from x in dbGroup.Users
                                 where !_group.Users.Contains(x)
                                 select x).ToList();

                        foreach (var item in v)
                        {
                            dbGroup.Users.Remove(item);
                        }


                        foreach (var newUser in _group.Users)
                        {
                            var item = dbGroup.Users.SingleOrDefault(s => s.UserId == newUser.UserId);
                            if (item != null)
                                oIACMDBContext.Entry(item).CurrentValues.SetValues(newUser);
                            else
                            {
                                var dbUser = oIACMDBContext.Users.Where(t => t.UserId == newUser.UserId).FirstOrDefault();

                                if (dbUser != null)
                                    dbGroup.Users.Add(dbUser);
                            }
                        }
                    }
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Saving the Group", ex);
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
                        Group targetGroup = oIACMDBContext.Groups.FirstOrDefault(grp => grp.GroupId == _GroupId);
                        if (targetGroup != null)
                        {
                            foreach (User item in _Usergroup)
                            {
                                newUser = oIACMDBContext.Users.FirstOrDefault(usr => usr.UserId == item.UserId);
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
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Inserting the UserGroup", ex);

                throw ex;
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
                        Group targetGroup = oIACMDBContext.Groups.FirstOrDefault(grp => grp.GroupId == GroupId);
                        if (targetGroup != null)
                        {
                            foreach (User usr in _delUsers)
                            {
                                removeUser = oIACMDBContext.Users.FirstOrDefault(aUser => aUser.UserId == usr.UserId);
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
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the UserGroup", ex);
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
                    Group delGroup = oIACMDBContext.Groups.FirstOrDefault(grp => grp.GroupId == GroupId);
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
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the Group", ex);
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
