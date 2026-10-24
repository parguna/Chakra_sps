using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class CompliancePolicy : DomainModelBase<CompliancePolicy, EntityModel.CompliancePolicy>
    {
        public string CompliancePolicyCode { get; set; }

        public string CompliancePolicyName { get; set; }

        public string CompliancePolicyDescription { get; set; }

        public string CompliancePolicyType { get; set; }

        public string Status { get; set; }

        public Nullable<long> OwnerId { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public User Owner { get; set; }

        public List<SODConflictFunctions> Conflicts { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static bool DoesExists(string compliancePolicyCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {

                    return new RepositoryBase<EntityModel.CompliancePolicy>(dataAccessTransaction)
                    .Any(policycode => policycode.CompliancePolicyCode == compliancePolicyCode);
            }
        }

        public static bool HasConflictFunction(long conflictFunctionId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.CompliancePolicy>(dataAccessTransaction)
                    .Any(policy => policy.Conflicts.Any(con => con.Id == conflictFunctionId));
            }
        }

        //public override void Save()
        //{
        //    bool isNew = this.IsNew;

        //    using (DataAccessTransaction tranaction = new DataAccessTransaction())
        //    {
        //        base.Save(tranaction);

        //        if (!isNew && this.Conflicts != null)
        //        {
        //            List<SODConflictFunctions> dbconflicts = ORMap(new RepositoryBase<EntityModel.CompliancePolicy>(tranaction)
        //                .Get(g => g.Id == this.Id,g => g.Conflicts).Single()).Conflicts;

        //            List<SODConflictFunctions> newfunction = dbconflicts
        //                .Except(this.Conflicts, new IdBaseEqualityComparer<SODConflictFunctions>()).ToList();

        //            newfunction.ForEach(conflicts => conflicts.Delete(tranaction));

        //            this.Conflicts.ForEach(conflicts => conflicts.Save(tranaction));
        //        }

        //        tranaction.Commit();
        //    }
        //}
        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new CompliancePolicyRepository(transaction).Insert(ORUnMap());
                }
                else
                {
                    new CompliancePolicyRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }
        public static CompliancePolicy GetByCompliancePolicyCode(string compliancePolicyCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.CompliancePolicy>(tranaction)
                    .Get(compolicy => compolicy.CompliancePolicyCode == compliancePolicyCode, 
                      compolicy => compolicy.Owner,
                      compolicy => compolicy.Conflicts).FirstOrDefault());
            }
        }

        public static CompliancePolicy GetByIdWithNavigation(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.CompliancePolicy>(tranaction)
                    .GetById(id,
                      compolicy => compolicy.Owner,
                      compolicy => compolicy.Conflicts));
            }
        }

        public static Dictionary<long, string> GetBySearchString(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new CompliancePolicyRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(searchString));
            }
        }
        static List<string> GetAllProblemRolesInsideFunction(SODFunction function, List<EntitlementReconcilation> userEntitlement)
        {
            List<string> RolesInFunction = new List<string>();
            List<string> currentUserEntitlement = userEntitlement.Select(a => a.Entitlement).ToList();
            foreach (SODFunctionItem functionitem in function.SodFunctionItems)
            {
                List<string> currentUserEntitlementForFunctionEndpoint = userEntitlement.Where(a => a.EndpointSystemId == functionitem.Endpointsystemid).Select(a => a.Entitlement).ToList();
                if (functionitem.SodFunctionType == "TCodes")
                {
                    var tCodeRoles = SAPRoleToAuthMap.GetRolesByTCode(functionitem.SodRoleDesc, (long)functionitem.Endpointsystemid);
                    foreach(SAPRoleToAuthMap tCodeRole in tCodeRoles)
                        RolesInFunction.Add(tCodeRole.RoleName);
    }
                else
                {
                    RolesInFunction.Add(functionitem.SodRoleDesc);
}
                RolesInFunction = RolesInFunction.Intersect(currentUserEntitlementForFunctionEndpoint).ToList();
            }

            return RolesInFunction.Distinct().ToList();
        }

        public static List<UserSODComplianceReport> CheckUserConflictBetweenFunctions(List<EntitlementReconcilation> userEntitlement, SODConflict conflict)
        {
            try
            {
                bool isUserHasFunction1Roles = true; bool isUserHasFunction2Roles = true;
                List<UserSODComplianceReport> lstUserProblemRoleReport = new List<UserSODComplianceReport>();
                List<string> lstUserProblemRoleFromFunction1 = new List<string>();
                List<string> lstUserProblemRoleFromFunction2 = new List<string>();

                var risk = SODRiskCode.GetById(conflict.RiskCodeId);
                var riskLevel = risk != null ? SODRiskLevel.GetById((long)risk.RiskLevelId).RiskLevelCode : "";
                var riskScore = risk != null ? SODRiskScore.GetById((long)risk.RiskScoreId).RiskScore : 0;

                //validate user role against the roles available in the function1
                lstUserProblemRoleFromFunction1 = GetAllProblemRolesInsideFunction(conflict.function1, userEntitlement);
                if(lstUserProblemRoleFromFunction1.Count > 0) isUserHasFunction1Roles = true;
                
                //if user doesnt have function1 roles, then he doesnt have any conflict at all, all good.
                if (lstUserProblemRoleFromFunction1.Count == 0 && !isUserHasFunction1Roles) return lstUserProblemRoleReport;

                //validate user role against the roles available in the function2
                lstUserProblemRoleFromFunction2 = GetAllProblemRolesInsideFunction(conflict.function2, userEntitlement);
                if (lstUserProblemRoleFromFunction2.Count > 0) isUserHasFunction2Roles = true;

                //if user has roles from both function1 roles and function2 roles then only we are having potential conflicts.
                if (isUserHasFunction1Roles && isUserHasFunction2Roles)
                {
                    foreach (string entitlement in lstUserProblemRoleFromFunction1)
                    {
                        foreach(string conflictEntitlement in lstUserProblemRoleFromFunction2)
                        {
                            try
                            {
                                var conflictType = "Existing";

                                if (userEntitlement.Where(a => a.Entitlement == entitlement && a.EndpointUserId == 0).Any())
                                {
                                    conflictType = "New";
                                }

                                if (userEntitlement.Where(a => a.Entitlement == conflictEntitlement && a.EndpointUserId == 0).Any())
                                {
                                    conflictType = "New";
                                }

                                var endpoint1 = conflict.function1.SodFunctionItems.Where(a => a.SodRoleDesc == entitlement);
                                var conflict1Endpoint = endpoint1.Any() ? endpoint1.FirstOrDefault().endpoint.DisplayName : "";
                                var endpoint2 = conflict.function2.SodFunctionItems.Where(a => a.SodRoleDesc == conflictEntitlement);
                                var conflict2Endpoint = endpoint2.Any() ? endpoint2.FirstOrDefault().endpoint.DisplayName : "";

                                lstUserProblemRoleReport.Add(new UserSODComplianceReport {
                                    ConflictEntitlement1 = entitlement,
                                    ConflictEntitlement1Endpoint = conflict1Endpoint,
                                    ConflictEntitlement2 = conflictEntitlement,
                                    ConflictEntitlement2Endpoint = conflict2Endpoint,
                                    ConflictName = conflict.SODConflictCode,
                                    ConflictDescription = conflict.SODConflictDescription,
                                    Function1Name = conflict.function1.SodFunctionCode,
                                    Function1Description = conflict.function1.SodFunctionDescription,
                                    Function2Name = conflict.function2.SodFunctionCode,
                                    Function2Description = conflict.function2.SodFunctionDescription,
                                    Risk = risk.RiskCode,
                                    RiskDescription = risk.RiskDescription,
                                    RiskLevel = riskLevel,
                                    RiskScore = riskScore,
                                    Endpoint = userEntitlement.Where(a => a.Entitlement == entitlement).FirstOrDefault().EndpointType.ToString(),
                                    EndpointSystemId=userEntitlement.Where(a=>a.Entitlement==entitlement).FirstOrDefault().EndpointSystemId,
                                    Type = conflictType,
                                });
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    return lstUserProblemRoleReport;
                }
                return new List<UserSODComplianceReport>();
            }
            catch (Exception ex)
            {
                return new List<UserSODComplianceReport>();
            }
        }

        static List<UserSODComplianceReport> CheckUserHasSensitiveFunctions(List<EntitlementReconcilation> userEntitlement, SODFunction function, long riskCodeId, string sodConflictCode, string sodConflictDescription)
        {
            try
            {
                bool isUserHasFunction1Roles = true;
                List<UserSODComplianceReport> lstUserProblemRoleReport = new List<UserSODComplianceReport>();
                List<string> lstUserProblemRoleFromFunction1 = new List<string>();

                var risk = SODRiskCode.GetById(riskCodeId);
                var riskLevel = risk != null ? SODRiskLevel.GetById((long)risk.RiskLevelId).RiskLevelCode : "";
                var riskScore = risk != null ? SODRiskScore.GetById((long)risk.RiskScoreId).RiskScore : 0;

                //validate user role against the roles available in the function1
                lstUserProblemRoleFromFunction1 = GetAllProblemRolesInsideFunction(function, userEntitlement);
                if (lstUserProblemRoleFromFunction1.Count > 0)
                {
                    foreach (string entitlement in lstUserProblemRoleFromFunction1)
                    {
                        try
                        {
                            var conflictType = "Existing";

                            if (userEntitlement.Where(a => a.Entitlement == entitlement && a.EndpointUserId == 0).Any())
                            {
                                conflictType = "New";
                            }

                            var endpoint = function.SodFunctionItems.Where(a => a.SodRoleDesc == entitlement);
                            var conflictEndpoint = endpoint.Any() ? endpoint.FirstOrDefault().endpoint.DisplayName : "";

                            lstUserProblemRoleReport.Add(new UserSODComplianceReport
                            {
                                ConflictEntitlement1 = entitlement,
                                ConflictEntitlement1Endpoint = conflictEndpoint,
                                ConflictEntitlement2 = "-",
                                ConflictEntitlement2Endpoint = "-",
                                ConflictName = sodConflictCode,
                                ConflictDescription = sodConflictDescription,
                                Function1Name = function.SodFunctionCode,
                                Function1Description = function.SodFunctionDescription,
                                Function2Name = "-",
                                Function2Description = "-",
                                Risk = risk.RiskCode,
                                RiskDescription = risk.RiskDescription,
                                RiskLevel = riskLevel,
                                RiskScore = riskScore,
                                Endpoint = userEntitlement.Where(a => a.Entitlement == entitlement).FirstOrDefault().EndpointType.ToString(),
                                EndpointSystemId = userEntitlement.Where(a => a.Entitlement == entitlement).FirstOrDefault().EndpointSystemId,
                                Type = conflictType,
                                IsSensitiveRole = true,
                            });
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    return lstUserProblemRoleReport;
                }
                                
                return new List<UserSODComplianceReport>();
            }
            catch (Exception ex)
            {
                return new List<UserSODComplianceReport>();
            }
        }

        public static IList<UserSODComplianceReport> CheckUserSensitiveComplianceReport(UserSODComplianceRequest request)
        {
            ComplianceProcess complianceProcess = null;
            List<UserSODComplianceReport> complianceRoleDetails = new List<UserSODComplianceReport>();
            try
            {
                var _processId = request.PolicyId;
                //try to get the available policy to validate.
                //understanding is that company will have only one policy.
                //if we have many policy, we are taking the first policy for the SensitiveAccess.
                if (_processId <= 0)
                {
                    // var defaultPolicy = CompliancePolicy.GetById(2,true);
                    var defaultPolicy = CompliancePolicy.GetAll().Where(a => a.CompliancePolicyType != "SOD" && a.IsActive == true).FirstOrDefault();
                    if (defaultPolicy == null) return complianceRoleDetails;
                    var sODPolicyExecutionDefinition = SODPolicyExecutionDefinition.GetAll().Where(pe => pe.CompliancePolicyId.HasValue && pe.CompliancePolicyId.Value == defaultPolicy.Id).FirstOrDefault();
                    if (sODPolicyExecutionDefinition == null) return complianceRoleDetails;
                    _processId = sODPolicyExecutionDefinition.Id;
                }

                complianceProcess = ComplianceProcess.GetComplianceProcessByPolicyExecutionId(_processId);
                var policyExecutionDefinition = complianceProcess != null ? complianceProcess.PolicyExecutionDefinition : null;
                if (policyExecutionDefinition == null)
                {
                    return complianceRoleDetails;
                }
                var policy = policyExecutionDefinition.CompliancePolicy;
                if (policy == null)
                {
                    return complianceRoleDetails;
                }

                if (policy.CompliancePolicyType != "SOD")
                {
                    //get all user roles.
                    List<EntitlementReconcilation> userEntitlement = EntitlementReconcilation.GetByUser(request.UserId);
                    if (!string.IsNullOrEmpty(request.EntitlementDetails))
                    {
                        var entitlementDetails = request.EntitlementDetails.Trim();
                        if (!string.IsNullOrEmpty(entitlementDetails))
                        {
                            foreach (string requestedRole in entitlementDetails.Split(','))
                            {

                                userEntitlement.Add(new EntitlementReconcilation
                                {
                                    Entitlement = requestedRole,
                                    EndpointSystemId = request.EndpointSystemId,
                                    EndpointType = request.EndpointTypeValue,
                                });
                            }
                        }
                    }

                    //if its from Access Request get all the entitlements from cart as well.
                    if (request.IsFromAccessRequest)
                    {
                        var interimRequest = InterimStore.GetByUserId(request.UserId, true, Infrastructure.Enums.InterimStoreItemType.AccessRequest);

                        if (interimRequest != null)
                        {
                            for (int i = 0; i < interimRequest.Count; i++)
                            {
                                var AdhocaccessRequest = !String.IsNullOrEmpty(interimRequest[i].XmlData) ?
                                    UtilityMethods.Deserialize<Dto.AdHocAccessRequest>(interimRequest[i].XmlData) : null;
                                if (AdhocaccessRequest != null)
                                {
                                    if (request.RequesteeUserId == AdhocaccessRequest.RequesteeUserId || request.UserId == AdhocaccessRequest.RequesterUserId)
                                    {
                                        userEntitlement.Add(new EntitlementReconcilation
                                        {
                                            Entitlement = AdhocaccessRequest.Entitlement,
                                            EndpointSystemId = (long)AdhocaccessRequest.EndpointSystemId,
                                            EndpointType = AdhocaccessRequest.EndpointSystemType,
                                        });
                                    }
                                }
                            }
                        }
                    }

                    foreach (SODConflictFunctions conflictfunction in policy.Conflicts)
                    {
                        SODConflictFunctions conflictFunctions = SODConflictFunctions.GetSodFunctionFromConflictFunction(conflictfunction.SodConflictCode);
                        List<SODFunction> saFunctions = conflictFunctions.Functions;
                        if (saFunctions.Count > 0)
                        {
                            foreach (SODFunction function in saFunctions)
                            {
                                complianceRoleDetails.AddRange(CheckUserHasSensitiveFunctions(userEntitlement, function, conflictfunction.RiskCodeId, conflictfunction.SodConflictCode, conflictfunction.SodConflictDescription));
                            }
                        }
                    }
                }

                foreach (UserSODComplianceReport userSOD in complianceRoleDetails)
                {
                    userSOD.CompliancePolicyCode = policy.CompliancePolicyCode;
                    userSOD.CompliancePolicyName = policy.CompliancePolicyName;
                    userSOD.CompliancePolicyType = policy.CompliancePolicyType;
                }

                //foreach (SODException exception in policyExecutionDefinition.Exceptions)
                //{
                //    var sodException = SODException.GetById(exception.Id, true);
                //    ComplianceReport = ApplyException(ComplianceReport, sodException);
                //}

                return complianceRoleDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IList<UserSODComplianceReport> CheckUserCompliance(UserSODComplianceRequest request)
        {
            ComplianceProcess complianceProcess = null;
            AccessRequest accessRequest = null;
            List<UserSODComplianceReport> complianceRoleDetails = new List<UserSODComplianceReport>();
            try
            {
                if(request.AccessRequestId > 0) accessRequest = AccessRequest.GetByIdIncludeAdHocAccessRequest(request.AccessRequestId);
                if (request.UserId <= 0)
                {                    
                    if (accessRequest != null)
                    {
                        request.UserId = accessRequest.UserId;                            
                    }
                    else
                        return complianceRoleDetails;
                    
                }

                if(string.IsNullOrEmpty(request.EntitlementDetails) && accessRequest != null)
                {
                    if (accessRequest.AdHocAccessRequest != null)
                        request.EntitlementDetails = accessRequest.AdHocAccessRequest.AccountType != AccountType.User ? accessRequest.AdHocAccessRequest.SharedAccountName : accessRequest.AdHocAccessRequest.Entitlement;
                }

                if((request.EndpointSystemId <= 0) && accessRequest != null)
                {
                    request.EndpointSystemId = (long)accessRequest.EndpointSystemId;
                    if(accessRequest.AdHocAccessRequest != null)
                        request.EndpointTypeValue = accessRequest.AdHocAccessRequest.EndpointSystemType;
                }

                var _processId = request.PolicyId;
                //try to get the available policy to validate.
                //understanding is that company will have only one policy.
                //if we have many policy, we are taking the first policy for the SOD.
                if (_processId <= 0)
                {
                   // var defaultPolicy = CompliancePolicy.GetById(2,true);
                    var defaultPolicy = CompliancePolicy.GetAll().Where(a => a.CompliancePolicyType == "SOD" && a.IsActive == true).FirstOrDefault();
                    if (defaultPolicy == null) return complianceRoleDetails;
                    var sODPolicyExecutionDefinition = SODPolicyExecutionDefinition.GetAll().Where(pe => pe.CompliancePolicyId.HasValue && pe.CompliancePolicyId.Value == defaultPolicy.Id).FirstOrDefault();
                    if(sODPolicyExecutionDefinition == null) return complianceRoleDetails;
                    _processId = sODPolicyExecutionDefinition.Id;
                }

                complianceProcess = ComplianceProcess.GetComplianceProcessByPolicyExecutionId(_processId);
                var policyExecutionDefinition = complianceProcess != null?complianceProcess.PolicyExecutionDefinition:null;
                if (policyExecutionDefinition == null)
                {
                    complianceRoleDetails.AddRange(CheckUserSensitiveComplianceReport(request));
                    return complianceRoleDetails;
                }
                var policy = policyExecutionDefinition.CompliancePolicy;
                if (policy == null)
                {
                    complianceRoleDetails.AddRange(CheckUserSensitiveComplianceReport(request));
                    return complianceRoleDetails;
                } 
                
                if (policy.CompliancePolicyType == "SOD")
                {
                    //get all user roles.
                    List<EntitlementReconcilation> userEntitlement = EntitlementReconcilation.GetByUser(request.UserId);
                    if (!string.IsNullOrEmpty(request.EntitlementDetails))
                    {
                        var entitlementDetails = request.EntitlementDetails.Trim();
                        if (!string.IsNullOrEmpty(entitlementDetails))
                        {
                            foreach (string requestedRole in entitlementDetails.Split(','))
                            {
                                userEntitlement.Add(new EntitlementReconcilation
                                {
                                    Entitlement = requestedRole,
                                    EndpointSystemId = request.EndpointSystemId,
                                    EndpointType = request.EndpointTypeValue,
                                });
                            }
                        }
                    }

                    //if its from Access Request get all the entitlements from cart as well.
                    if(request.IsFromAccessRequest)
                    {
                        IList<InterimStore> interimRequest;
                             interimRequest = InterimStore.GetByUserId(request.UserId, true, Infrastructure.Enums.InterimStoreItemType.AccessRequest);
                        if (interimRequest != null)
                        {
                            for (int i = 0; i < interimRequest.Count; i++)
                            {
                                var AdhocaccessRequest = !String.IsNullOrEmpty(interimRequest[i].XmlData) ?
                                    UtilityMethods.Deserialize<Dto.AdHocAccessRequest>(interimRequest[i].XmlData) : null;
                                
                                if (AdhocaccessRequest != null)
                                {
                                    if (request.RequesteeUserId == AdhocaccessRequest.RequesteeUserId || request.UserId==AdhocaccessRequest.RequesterUserId)
                                    {
                                        userEntitlement.Add(new EntitlementReconcilation
                                        {
                                            Entitlement = AdhocaccessRequest.Entitlement,
                                            EndpointSystemId = (long)AdhocaccessRequest.EndpointSystemId,
                                            EndpointType = AdhocaccessRequest.EndpointSystemType,
                                        });
                                    }
                                }
                            }
                        }
                    }

                    foreach (SODConflictFunctions conflictfunction in policy.Conflicts)
                    {
                        List<SODConflict> lstConflicts = SODConflict.GetBySodConflictCodeIncludingSODRiskCode(conflictfunction.SodConflictCode);
                        if (lstConflicts.Count > 0)
                        {
                            foreach (SODConflict conflict in lstConflicts)
                            {
                                complianceRoleDetails.AddRange(CheckUserConflictBetweenFunctions(userEntitlement, conflict));                                
                            }
                        }
                    }
                }

                foreach(UserSODComplianceReport userSOD in complianceRoleDetails)
                {
                    userSOD.CompliancePolicyCode = policy.CompliancePolicyCode;
                    userSOD.CompliancePolicyName = policy.CompliancePolicyName;
                    userSOD.CompliancePolicyType = policy.CompliancePolicyType;
                }

                //foreach (SODException exception in policyExecutionDefinition.Exceptions)
                //{
                //    var sodException = SODException.GetById(exception.Id, true);
                //    ComplianceReport = ApplyException(ComplianceReport, sodException);
                //}

                complianceRoleDetails.AddRange(CheckUserSensitiveComplianceReport(request));

                return complianceRoleDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
