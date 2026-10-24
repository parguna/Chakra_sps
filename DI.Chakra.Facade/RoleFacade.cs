using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure;
using DI.Chakra.DataAccess;
using DI.Chakra.Model.ReportData;

namespace DI.Chakra.Facade
{
    public class RoleFacade// : RoleFacade
    {

        RoleDA RoleDA = null;

        public RoleFacade()
        {
            RoleDA = new RoleDA();
        }

        #region RoleFacade Members

        public IList<Role> GetAllRole()
        {
            try
            {
                return this.GetAll();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Role GetRole(long _id)
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

        public bool IsRoleExist(string RoleName, int RoleId)
        {
            try
            {
                return RoleDA.IsRoleExist(RoleName, RoleId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public int Add(Role _role)
        //{
        //    try
        //    {
        //        return RoleDA.AddRole(_role);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public void Update(Role _role)
        //{
        //    try
        //    {
        //        RoleDA.updateRole(_role);
        //    }
        //    catch
        //    {

        //        throw;
        //    }
        //}

        public long SaveRole(Role _role)
        {
            try
            {
                return RoleDA.SaveRole(_role);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long CloneRole(Role role, bool cloneMenu, bool cloneQL, bool cloneDB, int cloneRoleID)
        {
            try
            {
                return RoleDA.CloneRole(role, cloneMenu, cloneQL, cloneDB, cloneRoleID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteRole(int RoleId)
        {
            return RoleDA.DeleteRole(RoleId);
        }
        public bool DeleteUserForSelectedRole(int RoleId, User User)
        {
            return RoleDA.DeleteUserForSelectedRole(RoleId, User);
        }
        #endregion

        #region IAssembler<Role,Role> Members

        public Role Assemble(Role _dto)
        {
            Role role = Role.CreateRole();

            role.Id = _dto.Id;
            role.RoleName = _dto.RoleName;
            role.RoleDescription = _dto.RoleDescription;
            role.CreatedBy = _dto.CreatedBy;
            role.CreatedDate = _dto.CreatedDate;
            role.LastModifiedBy = _dto.LastModifiedBy;
            role.LastModifiedDate = _dto.LastModifiedDate;

            return role;
        }

        public Role Dessemble(Role _aggregateRoot)
        {
            Role dto = new Role();
            dto.Id = _aggregateRoot.Id;
            dto.RoleName = _aggregateRoot.RoleName;
            dto.RoleDescription = _aggregateRoot.RoleDescription;
            dto.CreatedBy = _aggregateRoot.CreatedBy;
            dto.CreatedDate = _aggregateRoot.CreatedDate;
            dto.LastModifiedBy = _aggregateRoot.LastModifiedBy;
            dto.LastModifiedDate = _aggregateRoot.LastModifiedDate;

            return dto;

        }

        #endregion
        public Role GetByKey(object _key)
        {
            try
            {
                var _list = GetAll();

                var _role = _list.Where(role => role.Id == Convert.ToInt32(_key)).ToList();

                if (_role.Count() <= 0)
                    return null;

                return (Role)_role.First();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Role> GetAll()
        {
            try
            {
                return RoleDA.GetAllRole();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<Role> GetAllRolesByEndpointID(int EndpointSystemId, int EndPointID)
        {
            try
            {
                return RoleDA.GetAllRolesByEndpointID(EndpointSystemId, EndPointID);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public void SaveGroup(Role _Role)
        //{
        //    try
        //    {
        //        RoleDA.SaveGroup(_Role);

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

    }
}
