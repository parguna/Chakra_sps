using DI.Chakra.DataAccess;
using DI.Chakra.Model;
using System;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Collections;

namespace DI.Chakra.Facade
{
    public class InhouseSystemFacade
    {
        public InhouseSystemDA _inhouseSystemDA { get; set; }

        public InhouseSystemFacade()
        {
            _inhouseSystemDA = new InhouseSystemDA();
        }

        public void AddInhouseSystem(InhouseSystemConfiguration inhouseSystem)
        {
            try
            {
                _inhouseSystemDA.AddInhouseSystem(inhouseSystem);
            }
            catch
            {
                throw;
            }
        }

        public InhouseSystemConfiguration GetInhouseSystemById(int EndpointSystemID)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseSystemById(EndpointSystemID);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateInhouseSystem(InhouseSystemConfiguration inhouseSystem)
        {
            try
            {
                _inhouseSystemDA.UpdateInhouseSystem(inhouseSystem);
            }
            catch
            {
                throw;
            }
        }
        
       
   
        public IList<InhouseSystemsGridData> GetInhouseSystemsGridData()
        {
            try
            {
                return _inhouseSystemDA.GetInhouseSystemsGridData();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<InhouseSystemEntitlementsGridData> GetInhouseSystemEntitlementsGridData(int inhouseSystemId)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseSystemEntitlementsGridData(inhouseSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<InhouseSystemConfiguration> GetInhouseSystemsByEndpointType(EndpointType endpointSystemType)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseSystemsByEndpointType(endpointSystemType);
            }
            catch
            {
                throw;
            }
        }
        public List<InhousePermission> GetAllPermissions()
        {
            try
            {
                return _inhouseSystemDA.GetAllPermissions();
            }
            catch
            {
                throw;
            }
        }
        public Dictionary<long, string> GetPermissionsByEndpointSystemId(int EndpointSysteId, string searchString)
        {
            try
            {
                return _inhouseSystemDA.GetPermissionsByEndpointSystemId(EndpointSysteId, searchString);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<int, string> GetEntitlementsByInhouseSystemId(int inhouseSystemId, string searchString)
        {
            //try
            //{
            //    return _inhouseSystemDA.GetEntitlementsByInhouseSystemId(inhouseSystemId, searchString);
            //}
            //catch
            //{
            //    throw;
            //}

            return null;
        }

        public InhouseSystemConfiguration GetInhouseSystemBySystemCode(string SystemCode)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseSystemBySystemCode(SystemCode);
            }
            catch
            {
                throw;
            }
        }

        public void AddInhouseSystemUserInfo(InhouseUserInfo inhouseSystem)
        {
            try
            {
                _inhouseSystemDA.AddInhouseSystemUserInfo(inhouseSystem);
            }
            catch
            {
                throw;
            }
        }

        public InhousePermission GetInhouseSystemPermissionById(int ID)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseSystemPermissionById(ID);
            }
            catch
            {
                throw;
            }
        }

        public void AddInhouseSystemPermission(InhousePermission inhousepermission)
        {
            try
            {
                _inhouseSystemDA.AddInhouseSystemPermission(inhousepermission);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateInhousePermission(InhousePermission inhousepermission)
        {
            try
            {
                _inhouseSystemDA.UpdateInhousePermission(inhousepermission);
            }
            catch
            {
                throw;
            }
        }

        public IList<InhouseSystemEntitlementsGridData> GetInhousePermissionGridData(string inhouseSystemCode)
        {
            try
            {
                return _inhouseSystemDA.GetInhousePermissionGridData(inhouseSystemCode);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<InhouseSystemEntitlementsGridData> GetInhousePermissionByInhouseID(int EndpointSystemID)
        {
            try
            {
                return _inhouseSystemDA.GetInhousePermissionByInhouseID(EndpointSystemID);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<InhouseUserInfo> GetInhouseUserInfoByUserName(string UserName, string inhousesystemcode)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseUserInfoByUserName(UserName, inhousesystemcode);
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseUserInfo> GetInhouseUserInfoByID(long ID)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseUserInfoByID(ID);
            }
            catch
            {
                throw;
            }
        }

        public InhousePermission GetInhousePermissionByID(int ID)
        {
            try
            {
                return _inhouseSystemDA.GetInhousePermissionByID(ID);
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseUserInfo> GetPrimaryInhouseUserInfoByuserId(long ID)
        {
            try
            {
                return _inhouseSystemDA.GetPrimaryInhouseUserInfoByuserId(ID);
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseUserInfo> GetPrimaryInhouseUserInfoByuserId(long ID, long endpointSystemId)
        {
            try
            {
                return _inhouseSystemDA.GetPrimaryInhouseUserInfoByuserId(ID, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseSystemConfiguration> GetInhouseSystemBySystemName(string SystemName)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseSystemBySystemName(SystemName);
            }
            catch
            {
                throw;
            }
        }

        public List<InhousePermission> GetInhousePermissionByName(string PermissionName, string SystemCode)
        {
            try
            {
                return _inhouseSystemDA.GetInhousePermissionByName(PermissionName, SystemCode);
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseSystemsGridData> GetInhouseUserInfoBySystemCode(string inhousesystemcode)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseUserInfoBySystemCode(inhousesystemcode);
            }
            catch
            {
                throw;
            }
        }

        public IList<InhouseUserInfo> GetInhouseUsers(int DIUserId)
        {
            return _inhouseSystemDA.GetInhouseUsers(DIUserId);
        }

        public List<InhousePermission> GetAllPermissionsByUserid(long UserId)
        {
            return _inhouseSystemDA.GetAllPermissionsByUserid(UserId);
        }


        public InhouseUserInfo GetInhouseUserInfoByUserId(int UserId)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseUserInfoByUserId(UserId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public InhouseUserInfo GetInhouseUserInfoByKey(long UserId)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseUserInfoByKey(UserId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void UpdateInhouseUserInfo(InhouseUserInfo inhouseuser)
        {
            try
            {
                _inhouseSystemDA.UpdateInhouseUserInfo(inhouseuser);
            }
            catch
            {
                throw;
            }
        }

        public List<AttributeMappingColumns> GetAttributeMappingColumns(int SystemId)
        {
            try
            {
                return _inhouseSystemDA.GetAttributeMappingColumns(SystemId);
            }
            catch
            {
                throw;
            }
        }

       
     

        public IList<InhouseSystemEntitlementsGridData> GetInhousePermissionBySystemCode(string SystemCode)
        {
            try
            {
                return _inhouseSystemDA.GetInhousePermissionBySystemCode(SystemCode);
            }
            catch
            {
                throw;
            }
        }

         public Dictionary<long, string> GetINHUsersByDIUserId(long DIUserID, int endpointsystemid, AccountType accountType)
        {
            try
            {
                return _inhouseSystemDA.GetINHUsersByDIUserId(DIUserID, endpointsystemid, accountType);
            }
            catch
            {
                throw;
            }
        }

         public IList<InhouseSystemsGridData> GetInhouseInfoByDIUserIDSystemCode(int DIUserID)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseInfoByDIUserIDSystemCode(DIUserID);
            }
            catch
            {
                throw;
            }
        }

         public IList<TableProperties> GetColumnsList(string TableName)
         {
             return _inhouseSystemDA.GetColumnsList(TableName);
         }
		 
		 public InhouseUserInfo GetInhouseInfoByUserNameAndSystemCode(string userName, string systemCode)
        {
            try
            {
                return _inhouseSystemDA.GetInhouseInfoByUserNameAndSystemCode(userName,  systemCode);
            }
            catch
            {
                throw;
            }
        }

         public List<InhouseUserInfo> GetInhouseUserInfoByEndpointSystemId(int EndpointSystemId)
         {
             try
             {
                 return _inhouseSystemDA.GetInhouseUserInfoByEndpointSystemId(EndpointSystemId);
             }
             catch
             {
                 throw;
             }
         }

         public void DeleteInhouseUserInfo(long userId)
         {
             try
             {
                 _inhouseSystemDA.DeleteInhouseUserInfo(userId);
             }
             catch
             {
                 throw;
             }
         }

         public void DeleteInhousePermission(int Id)
         {
             try
             {
                 _inhouseSystemDA.DeleteInhousePermission(Id);
             }
             catch
             {
                 throw;
             }
         }

         public void SaveInhouseUserInfo(InhouseUserInfo inhouseuser)
         {
             try
             {
                 _inhouseSystemDA.SaveInhouseUserInfo(inhouseuser);
             }
             catch
             {
                 throw;
             }
         }

         public void SaveInhousePermission(InhousePermission inhousepermission)
         {
             try
             {
                 _inhouseSystemDA.SaveInhousePermission(inhousepermission);
             }
             catch
             {
                 throw;
             }
         }

         public void MapUser(EndPoint EndPoint)
         {
             _inhouseSystemDA.MapUser(EndPoint);
         }
    }
}
