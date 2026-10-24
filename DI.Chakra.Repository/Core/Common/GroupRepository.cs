using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System;

namespace DI.Chakra.Repository
{
    public class GroupRepository : RepositoryBase<Group>
    {
        public GroupRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public Dictionary<long, string> GetByGroupName(string searchString)
        {
            return this.CurrentContext.Group
                .Where(item => item.GroupName.Contains(searchString))
                .ToList()
                .Select(x => new { Key = x.Id, Value = x.GroupName + " - " + x.GroupDescription })
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }

        //public override void Update(Group group) 
        //{
        //    Group dbRole = this.GetById(group.Id, r => r.Users);

        //    List<User> deleteUsers = dbRole.Users.Except(group.Users, new IdBaseEqualityComparer<User>()).ToList();

        //    List<User> addedUsers = group.Users.Except(dbRole.Users, new IdBaseEqualityComparer<User>()).ToList();

        //    deleteUsers.ForEach(user => dbRole.Users.Remove(user));

        //    foreach (User user in addedUsers)
        //    {
        //        if (CurrentContext.Entry(user).State == System.Data.Entity.EntityState.Detached)
        //            CurrentContext.User.Attach(user);

        //        dbRole.Users.Add(user);
        //    }

        //    base.Update(group);
        //}



        public override long Insert(Group group)
        {
            if (group.Users.Count > 0)
            {
                var selectedUsers = group.Users.ToList();
                group.Users.Clear();
                foreach (var usr in selectedUsers)
                {
                    User user = this.CurrentContext.User.Find(usr.Id);
                    if (user != null)
                        group.Users.Add(user);
                }
            }
            return base.Insert(group);

        }

        public override void Update(Group group)
        {
            Group dbGroup = this.GetById(group.Id, r => r.Users);
            dbGroup.Users.Clear();
            foreach (var usr in group.Users)
            {
                User user = this.CurrentContext.User.Find(usr.Id);
                if (user != null)
                    dbGroup.Users.Add(user);
            }

            base.Update(group);
        }

        public string DeleteGroup (long groupID)
        {
            try
            {
                Group group = this.CurrentContext.Group.Where(x => x.Id == groupID).FirstOrDefault();
                if (group != null)
                {
                    var attestations = this.CurrentContext.Attestations.Count(x => x.IsGroupTask && x.TaskActionUserId == groupID);
                    if (attestations > 0)
                    {
                        return string.Format("{0} Group is attached to Attestations, cannot be deleted.", group.GroupName);
                    }

                    var workflowDetails = this.CurrentContext.WorkflowConfigurationDetails.Count(x => x.ApprovalUserType == Infrastructure.Enums.ApprovalUserType.Group && x.Approver == groupID);
                    if ( workflowDetails > 0)
                    {
                        return string.Format("{0} Group is attached to Workflow, cannot be deleted.", group.GroupName);
                    }

                    var tasks = this.CurrentContext.Tasks.Count(x => x.IsGroupTask && x.ActionGroupId == groupID && x.IsActive && !x.IsDelegated && (x.Action == Infrastructure.Enums.StatusType.None || x.Action == Infrastructure.Enums.StatusType.NotInitiated || x.Action == Infrastructure.Enums.StatusType.Pending));
                    if (tasks > 0)
                    {
                        return string.Format("{0} Group is attached to Tasks, cannot be deleted.", group.GroupName);
                    }

                    this.ExecuteQuery(string.Format("Delete from UserGroup where Group_GroupId = {0}", groupID));
                    this.Delete(group);
                    return "Group Deleted Successfully";
                }
            }
            catch (System.Exception ex)
            {
                return string.Format("Exception while deleting Group: {0}", ex.Message);
                //   throw;
            }
            return "";
        }

        public long? GetByIdAndCheckOnlyOneUsers(long groupId)
        {
            long? retVal = null;
            var query = $@" SELECT U.ID FROM DI_M_Group G 
                            INNER JOIN UserGroup UG on G.Id = UG.Group_GroupId
                            INNER JOIN DI_M_User U on UG.User_UserId = U.Id
                            WHERE G.Id = {groupId}";
            var results = CurrentContext.Database.SqlQuery<long>(query).ToList();
            if (results.Count == 1)
                retVal = results.FirstOrDefault();

            return retVal;

        }
    }
}
