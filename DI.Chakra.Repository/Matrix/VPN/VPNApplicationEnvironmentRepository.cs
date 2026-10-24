using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class VPNApplicationEnvironmentRepository : RepositoryBase<EntityModel.VPNApplicationEnvironment>
    {
        public VPNApplicationEnvironmentRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }

        public Dictionary<long,string> GetApplication(long endPointUserId, long endpointId, bool isFulltimeEmployee, string searchString, AdHocActionType adHocActionType,VPNType vpnType)
        {
            var returnValue = new Dictionary<long, string>();
            if(adHocActionType == AdHocActionType.NewAccount)
            {


                var result = from application in CurrentContext.VPNApplicationEnvironment
                             select new { Id = application.MatrixId, Name = application.Name, VPNMatrixType = application.MatrixType };
                if (vpnType == VPNType.SSLVPNOnly)
                {
                    result = result.Where(i=>i.VPNMatrixType == VPNMatrixType.Application);
                }
                else
                {
                    result = result.Where(i => i.VPNMatrixType == VPNMatrixType.Project);
                }
                                
                
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    result = result.Where(i=>i.Name.Contains(searchString));
                }
                if (result.Any())
                {
                    result = result.Distinct();
                    foreach (var item in result)
                    {
                        var subString = string.Empty;
                        if (vpnType == VPNType.SSLVPNOnly)
                        {
                            subString = item.Name.Substring(0, item.Name.LastIndexOf("-"));
                        }
                        else
                        {
                            subString = item.Name;
                        }
                        
                        if (!returnValue.ContainsKey(item.Id))
                            returnValue.Add(item.Id, subString);
                    }
                }
            }
            else if (adHocActionType == AdHocActionType.AmendmentAdd)
            {
                var existingData = from vpnUser in CurrentContext.VPNUserApplicationEnviornment
                                   where vpnUser.EndpointUserId == endPointUserId
                                   select vpnUser;
                var existing = from vpnAppln in CurrentContext.VPNApplicationEnvironment
                               join vpnUser in existingData on new { vpnAppln.MatrixId, vpnAppln.EnviornmentType, vpnAppln.MatrixType } equals
                               new { vpnUser.MatrixId, vpnUser.EnviornmentType, vpnUser.MatrixType } into vpnUserMatrix
                               from subSet in vpnUserMatrix.DefaultIfEmpty()
                               where (subSet == null)
                               select new { Id = vpnAppln.MatrixId, Name = vpnAppln.Name, VPNMatrixType = vpnAppln.MatrixType };


                if (vpnType == VPNType.SSLVPNOnly)
                {
                    existing = existing.Where(i => i.VPNMatrixType == VPNMatrixType.Application);
                }
                else
                {
                    existing = existing.Where(i => i.VPNMatrixType == VPNMatrixType.Project);
                }
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    existing = existing.Where(i => i.Name.Contains(searchString));
                }
                if (existing.Any())
                {

                    var result = existing.Distinct().ToList();
                    foreach (var item in result)
                    {
                        var subString = string.Empty;
                        if (vpnType == VPNType.SSLVPNOnly)
                        {
                            subString = item.Name.Substring(0, item.Name.LastIndexOf("-"));
                        }
                        else
                        {
                            subString = item.Name;
                        }
                        if(!returnValue.ContainsKey(item.Id))
                            returnValue.Add(item.Id, subString);
                    }
                }
            }
            else if (adHocActionType == AdHocActionType.AmendmentRemove)
            {
                var existingData = from vpnUser in CurrentContext.VPNUserApplicationEnviornment
                                   where vpnUser.EndpointUserId == endPointUserId
                                   select vpnUser;
                var existing = from vpnAppln in CurrentContext.VPNApplicationEnvironment
                               join vpnUser in existingData on new { vpnAppln.MatrixId, vpnAppln.EnviornmentType, vpnAppln.MatrixType } equals
                               new { vpnUser.MatrixId, vpnUser.EnviornmentType, vpnUser.MatrixType }
                               select new { Id = vpnAppln.MatrixId, Name = vpnAppln.Name, VPNMatrixType = vpnAppln.MatrixType };

                if (vpnType == VPNType.SSLVPNOnly)
                {
                    existing = existing.Where(i => i.VPNMatrixType == VPNMatrixType.Application);
                }
                else
                {
                    existing = existing.Where(i => i.VPNMatrixType == VPNMatrixType.Project);
                }
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    existing = existing.Where(i => i.Name.Contains(searchString));
                }
                if (existing.Any())
                {
                    foreach (var item in existing)
                    {
                        var subString = string.Empty;
                        if (vpnType == VPNType.SSLVPNOnly)
                        {
                            subString = item.Name.Substring(0, item.Name.LastIndexOf("-"));
                        }
                        else
                        {
                            subString = item.Name;
                        }
                        if (!returnValue.ContainsKey(item.Id))
                            returnValue.Add(item.Id, subString);
                    }
                }
            }
            
            return returnValue;
        }

        public Dictionary<long, string> GetVPNMatrixByType(string searchString, VPNType vpnType)
        {
            var returnValue = new Dictionary<long, string>();
            var existingData = from vpnUser in CurrentContext.VPNApplicationEnvironment
                               select vpnUser;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                existingData = existingData.Where(i => i.Name.Contains(searchString));
            }
            if (vpnType == VPNType.SSLVPNOnly)
            {
                existingData = existingData.Where(i=>i.MatrixType == VPNMatrixType.Application);
            }
            else
            {
                existingData = existingData.Where(i => i.MatrixType == VPNMatrixType.Project);
            }
            var results = existingData.Select(i => new { Id = i.MatrixId, i.Name }).ToList();
            if (results.Count > 0)
            {
                foreach (var item in results)
                {
                    var subString = string.Empty;
                    if (vpnType == VPNType.SSLVPNOnly)
                    {
                        subString = item.Name.Substring(0, item.Name.LastIndexOf("-"));
                    }
                    else
                    {
                        subString = item.Name;
                    }
                    if (!returnValue.ContainsKey(item.Id))
                        returnValue.Add(item.Id, subString);
                }
            }
            return returnValue;
        }

        public Dictionary<VPNMatrixEnviornmentType, bool> GetApplicationEnviornment(long endPointUserId, bool excludeApplication, long matrixId)
        {
            var returnValue = new  Dictionary<VPNMatrixEnviornmentType, bool>();
            if (excludeApplication)
            {
                var existingData = (from vpnUser in CurrentContext.VPNUserApplicationEnviornment
                                   where vpnUser.EndpointUserId == endPointUserId && vpnUser.MatrixId == matrixId
                                   select vpnUser);


                var existing = from vpnAppln in CurrentContext.VPNApplicationEnvironment
                               join vpnUser in existingData on new { vpnAppln.MatrixId,vpnAppln.EnviornmentType } equals new { vpnUser.MatrixId, vpnUser.EnviornmentType }  into tempU
                               from subSet in tempU.DefaultIfEmpty()
                               where vpnAppln.MatrixId== matrixId && (subSet == null)
                               select new { EnviornmentType = vpnAppln.EnviornmentType, isNotExists = true };

                if (existing.Any())
                {

                    var result = existing.Where(i=>i.isNotExists).Distinct().ToList();
                    foreach (var item in result)
                    {
                        if (!returnValue.ContainsKey(item.EnviornmentType))
                            returnValue.Add(item.EnviornmentType, item.isNotExists);
                    }
                }
            }
            else 
            {
                var existing = from vpnUser in CurrentContext.VPNUserApplicationEnviornment
                                   where vpnUser.EndpointUserId == endPointUserId && vpnUser.MatrixId == matrixId
                                   select new { EnviornmentType = vpnUser.EnviornmentType, isExists = true };

                if (existing.Any())
                {

                    var result = existing.Distinct().ToList();
                    foreach (var item in result)
                    {
                        if (!returnValue.ContainsKey(item.EnviornmentType))
                            returnValue.Add(item.EnviornmentType, item.isExists);
                    }
                }
            }

            return returnValue;
        }
    }
}
