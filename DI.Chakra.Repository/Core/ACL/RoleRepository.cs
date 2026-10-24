using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace DI.Chakra.Repository
{
    public class RoleRepository : RepositoryBase<Role>
    {
        public RoleRepository(DataAccessTransaction transaction) : base(transaction) { }

        //public override void Update(Role role)
        //{
        //    //Role dbRole = this.GetById(role.Id, r => r.Users);

        //    //List<User> deleteUsers = dbRole.Users.Except(role.Users, new IdBaseEqualityComparer<User>()).ToList();

        //    //List<User> addedUsers = role.Users.Except(dbRole.Users, new IdBaseEqualityComparer<User>()).ToList();

        //    //deleteUsers.ForEach(user => dbRole.Users.Remove(user));

        //    //foreach (User user in addedUsers)
        //    //{
        //    //    if (CurrentContext.Entry(user).State == System.Data.Entity.EntityState.Detached)
        //    //        CurrentContext.User.Attach(user);

        //    //    dbRole.Users.Add(user);
        //    //}

        //    //base.Update(role);
        //}
        public override long Insert(Role role)
        {
            if (role.Users.Count > 0)
            {
                var selectedUsers = role.Users.ToList();
                role.Users.Clear();
                foreach (var usr in selectedUsers)
                {
                    User user = this.CurrentContext.User.Find(usr.Id);
                    if (user != null)
                        role.Users.Add(user);
                }
            }
            return base.Insert(role);

        }

        public override void Update(Role role)
        {
            Role dbRole = this.GetById(role.Id, r => r.Users);
            dbRole.Users.Clear();
            foreach (var usr in role.Users)
            {
                User user = this.CurrentContext.User.Find(usr.Id);
                if (user != null)
                    dbRole.Users.Add(user);
            }

            base.Update(role);
        }
    }
}
