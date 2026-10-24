using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using DeepIdentity.DataAccess;

namespace DeepIdentity.Facade
{
    public class GroupFacade// : GroupFacade
    {
        GroupDA GroupDA = null;

        #region GroupFacade Members

        public GroupFacade()
        {
            GroupDA = new GroupDA();
        }
        public IList<Group> GetAllGroup()
        {
            try
            {
                return GroupDA.GetAllGroup();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Dictionary<int, string> GetAllGroups(string searchString)
        {
            try
            {
                return GroupDA.GetAllGroups(searchString);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Group GetGroup(int _id)
        {
            try
            {
               return this.GetByKey(_id);
               
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
                return GroupDA.GetGroupById(groupId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetGroupNameById(int groupId)
        {
            try
            {
                return GroupDA.GetGroupNameById(groupId);
            }
            catch
            {

                throw;
            }
        }

        public IList<Group> GetGroupsByUser(int userID)
        {
            try
            {
                return GroupDA.GetGroupsByUser(userID);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Group GetGroupByName(string groupName)
        {
            try
            {
                return GroupDA.GetGroupByName(groupName);
            }
            catch
            {
                throw;
            }
        }

        public Group GetGroup(string _name)
        {
            try
            {
               
                var _list = this.GetAll();

                var _grp = _list.Where(grp => grp.GroupName == _name).ToList();

                if (_grp.Count() <= 0)
                    return null;

                return (Group)_grp.First();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public int AddGroup(Group _group)
        //{
        //    try
        //    {
        //        return GroupDA.AddGroup(_group);

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public void SaveGroup(Group _group)
        {
            try
            {
                GroupDA.SaveGroup(_group);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeleteGroup(int GroupId)
        {
            try
            {
                return GroupDA.DeleteGroup(GroupId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeleteUserGroup(int GroupId, IList<User>_delUsers)
        {
            try
            {
                return GroupDA.DeleteUserGroup(GroupId, _delUsers);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void AddUserGroup(IList<User> _Usergroup, int _GroupId)
        {
            try
            {
                GroupDA.AddUserGroup(_Usergroup, _GroupId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Add(Group _organisation)
        {
            throw new NotImplementedException();
        }

        public void Update(Group _group)
        {
            throw new NotImplementedException();
        }

        public IList<Group> GetAllGroupsByEndpointID(int EndpointSystemId, int EndPointID, string searchString)
        {
            try
            {
                return GroupDA.GetAllGroupsByEndpointID(EndpointSystemId, EndPointID, searchString);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Group GetByKey(object _key)
        {
            try
            {


                var _list = GetAll();

                var _grp = _list.Where(grp => grp.GroupId == Convert.ToInt32(_key)).ToList();

                if (_grp.Count() <= 0)
                    return null;

                return (Group)_grp.First();


            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Group> GetAll()
        {
            try
            {
                return GroupDA.GetAllGroup();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        //public void SaveGroup(Group group)
        //{
        //    throw new NotImplementedException();
        //}

        public bool IsGroupExist(string GroupName, int GroupId)
        {
            return GroupDA.IsGroupExist(GroupName, GroupId);
        }
    }
}
