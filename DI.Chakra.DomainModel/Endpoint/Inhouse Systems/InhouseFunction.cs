using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class InhouseFunction : DomainModelBase<InhouseFunction, EntityModel.InhouseFunction>, IXLData
    {
        public long EndpointSystemId { get; set; }
        public string FunctionName { get; set; }
        public string PermissionName { get; set; }
        public string InhouseSystemCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public static InhouseFunction GetByObject(InhouseFunction obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction).Get(cnfg => cnfg.FunctionName == obj.FunctionName && cnfg.PermissionName == obj.PermissionName && cnfg.InhouseSystemCode == obj.InhouseSystemCode).FirstOrDefault());
            }
                
        }

        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                var lastModifiedDate = new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
                        //string userDisplayName = User.GetByUserName(lastModifiedDate.CreatedBy)?.DisplayName;
                        //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }

        public static void TruncateDI_INH_Function()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DeleteAll();
            }
        }

        public static List<InhouseFunction> GetInhouseFunctionGridData(string inhouseSystemCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (inhouseSystemCode == "ALL")
                {

                    return ORMapList(new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction)
                           .GetAll().ToList());
                }
                else
                
                    return ORMapList(new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction)
                           .Get(objUser => objUser.InhouseSystemCode == inhouseSystemCode).ToList());
            }
                    
        }

        public static Dictionary<long, string> GetByEndPointSystemId(long endpointId, string permission, string searchString)
        {
                var endpoint = EndPoint.GetById(endpointId);
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                    {
                    return new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction).Get(inh => inh.InhouseSystemCode == endpoint.ApplicationCode && inh.PermissionName == permission && inh.FunctionName.Contains(searchString) && inh.IsActive).
                       OrderBy(inh => inh.FunctionName)
                       .Select(inh => new { Key = inh.Id, Value = inh.FunctionName }).ToDictionary(o => o.Key, o => o.Value);
                    }
                    
                }
                else
                {
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                    {
                        return new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction).Get(inh => inh.InhouseSystemCode == endpoint.ApplicationCode && inh.PermissionName == permission && inh.IsActive).
                          OrderBy(inh => inh.FunctionName)
                          .Select(inh => new { Key = inh.Id, Value = inh.FunctionName }).ToDictionary(o => o.Key, o => o.Value);
                    }
                    
                }
        }

        public static List<InhouseFunction> GetInhouseFunctionActive(long endpointId, string permission, int type, long endpointUserId = 0)
        {
            var functions = new List<InhouseFunction>();
                switch (type)
                {
                    case 1: //New account
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                    {
                        functions = ORMapList(new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction)
                      .Get(obj => obj.EndpointSystemId == endpointId && obj.PermissionName == permission && obj.IsActive).ToList());
                    }
                       
                        break;
                    case 2: //Amendment add
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                    {
                        functions = ORMapList(new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction)
                       .Get(obj => obj.EndpointSystemId == endpointId && obj.PermissionName == permission && obj.IsActive).ToList());
                        var existing = InhUserPermissionFunction.GetByEndpointUserId(endpointUserId, endpointId);
                        foreach (var eFunc in existing)
                            functions.RemoveAll(p => p.Id == eFunc.FunctionId);
                    }
                        
                        break;
                    case 3: //Amendment remove
                            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                            {
                                var allFunctions = ORMapList(new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction)
                               .Get(obj => obj.EndpointSystemId == endpointId && obj.PermissionName == permission && obj.IsActive).ToList());
                                var temp = InhousePermission.GetByName(permission, endpointId);
                                var existing1 = InhUserPermissionFunction.GetByEndpointUserId(endpointUserId, endpointId, temp.Id);
                                foreach (var eFunc in existing1)
                                    functions.Add(new InhouseFunction { Id = eFunc.Id, FunctionName = allFunctions.Where(p => p.Id == eFunc.FunctionId).FirstOrDefault().FunctionName });
                            }
                        
                        break;
                    default:
                        break;
                }
            return functions;
        }

        public static InhouseFunction GetInhouseFunctionByApplicationCode(string function, string inhouseSystemCode, string permission)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction)
                       .Get(obj => obj.InhouseSystemCode == inhouseSystemCode && obj.FunctionName == function.Trim() && obj.PermissionName == permission.Trim()).FirstOrDefault());
            }
                
        }

        public static bool DoesExist(string permission, string function, string applicationCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction).Any(item => item.FunctionName == function && item.PermissionName == permission && item.InhouseSystemCode == applicationCode);
            }
                
        }

        public static List<string> GetByPermissionName(string permission, string applicationCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.InhouseFunction>(dataAccessTransaction).Get(item => item.PermissionName == permission && item.InhouseSystemCode == applicationCode).ToList().Select(x => x.FunctionName).ToList();
            }
               
        }
    }
}
