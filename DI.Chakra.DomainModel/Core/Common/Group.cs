using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class Group : DomainModelBase<Group, EntityModel.Group>
    {
        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        public string EmailId { get; set; }

        public long? GroupOwner { get; set; }

        public bool IsDefaultGroup { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public List<User> Users { get; set; }

        public User Owner { get; set; }

        public static Group GetByGroupName(string groupName)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Group>(tranaction).
                    Get(grp => grp.GroupName.Equals(groupName, StringComparison.CurrentCultureIgnoreCase), grp => grp.Users).FirstOrDefault());
            }
        }

        public static Group GetByGroupNameWithoutUsers(string groupName)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Group>(tranaction).
                    Get(grp => grp.GroupName.Equals(groupName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());
            }
        }

        public static Group GetByIdIncludingUser(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Group>(tranaction).GetById(id, grp => grp.Users, grp => grp.Owner));
            }
                
        }

        public static Group GetByIdIncludingOnlyUser(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Group>(tranaction).GetById(id, grp => grp.Users));
            }
        }
        public static string DeleteGroupById(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                var result = new GroupRepository(transaction).DeleteGroup(id);
                transaction.Commit();
                return result;
            }
        }
        //public static Group GetByIdIncludingOwner(long id)
        //{
        //    Group grp = null;
        //    List<Group> groupinput = new List<Group>();
        //    List<User> groupList = new List<User>();
        //    groupinput.Add(Group.GetById(id));

        //    groupList = User.GetByGroup(groupinput);

        //    using (DataAccessTransaction tranaction = new DataAccessTransaction())
        //    {
        //        grp = ORMap(new RepositoryBase<EntityModel.Group>(tranaction).GetById(id, user => user.Users, user => user.Owner));
        //    }
        //    grp.Users = groupList;
        //    return grp;
        //}


        //public override void Save()
        //{
        //    using (DataAccessTransaction tranaction = new DataAccessTransaction())
        //    {
        //        base.Save(tranaction);
        //        if (this.Users != null)
        //        {
        //            List<User> dbUsers = ORMap(new RepositoryBase<EntityModel.Group>(tranaction).
        //                Get(g => g.Id == this.Id, g => g.Users).Single()).Users;

        //            List<User> removedUser = dbUsers.Except(this.Users, new IdBaseEqualityComparer<User>()).ToList();
        //            removedUser.ForEach(user => user.Delete(tranaction));
        //            this.Users.ForEach(user => user.Save());
        //        }
        //        tranaction.Commit();
        //    }
        //}


        //public override void Save()
        //{
        //    using (DataAccessTransaction transaction = new DataAccessTransaction())
        //    {
        //        if (this.Id == 0)
        //        {
        //            this.Id = new GroupRepository(transaction).Insert(ORUnMap());
        //        }
        //        else
        //        {
        //            new GroupRepository(transaction).Update(ORUnMap());
        //        }

        //        transaction.Commit();
        //    }
        //}


        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new GroupRepository(transaction).Insert(ORUnMap());
                    //base.Save(transaction);
                }
                else
                {
                    new GroupRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }
        public static void AddUserToGroup(List<User> users, long groupId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                User userToAdd;
                if (users != null && users.Count > 0 && groupId > -1)
                {
                    Group targetGroup = Group.GetById(groupId);
                    if (targetGroup != null)
                    {
                        foreach (User usr in users)
                        {
                            userToAdd = User.GetById(usr.Id);
                            if (userToAdd != null)
                                targetGroup.Users.Add(userToAdd);
                        }
                    }
                    targetGroup.Save(tranaction);
                    tranaction.Commit();
                }
            }
        }

        public static void DeleteUserFromGroup(long groupId, User userToRemove)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                Group targetGroup = Group.GetById(groupId, true);
                targetGroup.Users.Remove(userToRemove);
                targetGroup.Save(tranaction);
                tranaction.Commit();
            }
        }

        public static void DeleteUsersFromGroup(long groupId, IList<User> userListToRemove)
        {
            User userToRemove;
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                if (userListToRemove != null && userListToRemove.Count > 0 && groupId > -1)
                {
                    Group targetGroup = Group.GetById(groupId);
                    if (targetGroup != null)
                    {
                        foreach (User usr in userListToRemove)
                        {
                            userToRemove = User.GetById(usr.Id);
                            if (userToRemove != null)
                                targetGroup.Users.Remove(userToRemove);
                        }
                    }
                    targetGroup.Save(tranaction);
                    tranaction.Commit();
                }
            }
        }

        public static List<Group> GetByUser(long userId)
        {
            User user = User.GetById(userId);
            List<Group> groups = null;
            if (user != null)
                groups = user.Groups;
            return groups;
        }
        public static List<Group> GetByUserIncludeGroups(long userId)
        {
            User user = User.GetByIdIncludeGroup(userId);
            List<Group> groups = null;
            if (user != null)
                groups = user.Groups;
            return groups;
        }
        public static List<Group> GetAllGroupsByEndpointID(long endpointSystemTypeId, long endPointID, string searchString)
        {
            //using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                EndpointType endPointSystemTypeName = EndPointSystemType.GetById(endpointSystemTypeId).EndpointSystemTypeName;
                return EndPointFactory.GetEndPoint(endPointSystemTypeName).GetAllGroupsByEndpointID(endPointID, searchString);
                //switch (endPointSystemTypeName)
                //{
                //    case EndpointType.ActiveDirectory:
                //        List<ADSGroupInfo> GrpList = ADSGroupInfo.GetAllADSGroups(endPointID, CheckNull.AsteriskToString(searchString));
                //        var adsGrpList = (from tmp in GrpList select new { GroupId = tmp.Id, GroupName = tmp.cn, Description = tmp.description }).Distinct();

                //        if (adsGrpList.Count() > 0)
                //        {
                //            foreach (var adsGrp in adsGrpList)
                //            {
                //                group = new Group();
                //                group.Id = CheckNull.AgainstInt(adsGrp.GroupId);
                //                group.GroupName = CheckNull.AgainstString(adsGrp.GroupName);
                //                group.GroupDescription = CheckNull.AgainstString(adsGrp.Description);
                //                grpList.Add(group);
                //            }
                //        }
                //        break;

                //    case EndpointType.SAP:
                //        List<SAPGroupInfo> sapList = SAPGroupInfo.GetAllSAPGroups(endPointID, CheckNull.AsteriskToString(searchString));
                //        var sapGrpList = (from tmp in sapList select new { GroupId = tmp.Id, GroupName = tmp.GROUP_CODE, Description = tmp.GROUP_DESC }).Distinct();

                //        if (sapGrpList.Count() > 0)
                //        {
                //            foreach (var tLst in sapGrpList)
                //            {
                //                group = new Group();
                //                group.Id = CheckNull.AgainstInt(tLst.GroupId);
                //                group.GroupName = CheckNull.AgainstString(tLst.GroupName);
                //                group.GroupDescription = CheckNull.AgainstString(tLst.Description);
                //                grpList.Add(group);
                //            }
                //        }
                //        break;

                //    case EndpointType.LotusNotes:
                //        List<LotusGroupInfo> lnsList = LotusGroupInfo.GetAllLNSGroups(endPointID, CheckNull.AsteriskToString(searchString));
                //        var lnsGrpList = (from tmp in lnsList select new { GroupId = tmp.Id, GroupName = tmp.Group, Description = tmp.Description }).Distinct();

                //        if (lnsGrpList.Count() > 0)
                //        {
                //            foreach (var tLst in lnsGrpList)
                //            {
                //                group = new Group();
                //                group.Id = CheckNull.AgainstInt(tLst.GroupId);
                //                group.GroupName = CheckNull.AgainstString(tLst.GroupName);
                //                group.GroupDescription = CheckNull.AgainstString(tLst.Description);
                //                grpList.Add(group);
                //            }
                //        }
                //        break;

                //    case EndpointType.SAPEnterprisePortal:
                //        List<SAPEPGroupInfo> sapepList = SAPEPGroupInfo.GetAllSAPEPGroups(endPointID, CheckNull.AsteriskToString(searchString));
                //        var sapepGrpList = (from tmp in sapepList select new { GroupId = tmp.Id, GroupName = tmp.GroupName, Description = tmp.GroupDescription }).Distinct();

                //        if (sapepGrpList.Count() > 0)
                //        {
                //            foreach (var tLst in sapepGrpList)
                //            {
                //                group = new Group();
                //                group.Id = CheckNull.AgainstInt(tLst.GroupId);
                //                group.GroupName = CheckNull.AgainstString(tLst.GroupName);
                //                group.GroupDescription = CheckNull.AgainstString(tLst.Description);
                //                grpList.Add(group);
                //            }
                //        }
                //        break;
                //}
            }
        }

        public static List<Group> GetByGroupName()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Group>(transaction).
                    Get(grp => grp.GroupName == "AdminGroup" || grp.GroupName == "ImplementerGroup").Distinct().ToList());
            }
        }

        public static bool DoesExist(string groupName, long groupID)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                var dbGroup = ORMap(new RepositoryBase<EntityModel.Group>(transaction).
                 Get(c => c.GroupName == groupName).FirstOrDefault());

                return dbGroup != null ? true : false;
            }
        }

        public static Dictionary<long, string> GetAllGroups(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new GroupRepository(transaction).GetByGroupName(CheckNull.AsteriskToString(searchString));
            }
        }

        public static Group GetByIdIncludingUsers(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Group>(dataAccessTransaction).
                       Get(grp => grp.Id == id, grp => grp.Users).FirstOrDefault());
            }
        }
        public static Nullable<long> GetByIdAndCheckOnlyOneUsers(long groupId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new GroupRepository(dataAccessTransaction).GetByIdAndCheckOnlyOneUsers(groupId);
            }
        }
        public static List<Group> GetByIsActive()
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Group>(dataAccessTransaction).
                       Get(grp => grp.Id > 0, grp => grp.Users).ToList());
            }
        }

        public static List<string> GetUserEmailIds()
        {
            //using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<User> user = new List<User>();
                List<string> emailIds = new List<string>();
                user = Group.GetByGroupName("ITCCTeam").Users;
                foreach (User usr in user)
                {
                    emailIds.Add(usr.EmailId);
                }
                return emailIds;
            }
        }

        public static Dictionary<long, string> GetGroupsByUserId(long userid)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.User>(dataAccessTransaction).Get(user => user.Id == userid, user => user.Groups).FirstOrDefault().Groups.ToDictionary(x => x.Id, x => x.GroupName);
            }
        }
    }
}
