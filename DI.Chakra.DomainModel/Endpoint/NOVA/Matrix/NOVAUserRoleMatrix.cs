using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class NOVAUserRoleMatrix : DomainModelBase<NOVAUserRoleMatrix, EntityModel.NOVAUserRoleMatrix>, IXLData, IApprover
    {
        public string JobDescription { get; set; }
        public long EndPointSystemId { get; set; }
        public string CompanyCode { get; set; }
        public string SectionCode { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string FLINSRoles { get; set; }
        public string BWAccess { get; set; }
        public string ADGroups { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string BusinessRole
        {
            get
            {
                return this.Position;
            }
        }
        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.EAM);
            }
        }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.NOVAUserRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_NOVA_UserRoleMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }

        public static NOVAUserRoleMatrix GetNOVAUserRoleMatrixbyObject(NOVAUserRoleMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.NOVAUserRoleMatrix>(dataAccessTransaction)
                        .Get(nova => nova.CompanyCode == obj.CompanyCode && nova.SectionCode == obj.SectionCode && nova.Position == obj.Position).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.NOVAUserRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.NOVAUserRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    else
                        return string.Empty;
                }
            }
        }

        public static List<NOVAUserRoleMatrix> GetNOVAUserRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.NOVAUserRoleMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.NOVAUserRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.NOVAUserRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }

        public static NOVAUserRoleMatrix GetNovaMatrix(AdHocNovaRequestDetails detail)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            { 
                return  ORMap(new RepositoryBase<EntityModel.NOVAUserRoleMatrix>(dataAccessTransaction)
                        .Get(m => m.CompanyCode == detail.CompanyCode &&
                                  m.SectionCode == detail.SectionCode &&
                                  m.Position == detail.RoleName && (m.ValidTo == null || m.ValidTo >= DateTime.Now)).FirstOrDefault()); 
            }
        }

        public static NovaMatrixResponse GetNovaMatrix(NovaMatrixParmeter novaMatrixParmeter)
        {
            NovaMatrixResponse novaMatrixResponse = new NovaMatrixResponse();

            if (novaMatrixParmeter == null) return novaMatrixResponse;

            string company = novaMatrixParmeter.Company;
            string section = novaMatrixParmeter.Section;


            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    List<EntityModel.NOVACompositeRoleMatrix> compositeRoleMatrices = new RepositoryBase<EntityModel.NOVACompositeRoleMatrix>(dataAccessTransaction).GetAll().ToList();

                    if (novaMatrixParmeter.novaMatrixType == NovaMatrixType.All || novaMatrixParmeter.novaMatrixType == NovaMatrixType.User)
                    {
                        var matrix = new RepositoryBase<EntityModel.NOVAUserRoleMatrix>(dataAccessTransaction).GetAll();
                        ProcessMatrix(ref novaMatrixResponse, compositeRoleMatrices, NovaMatrixType.User, matrix.ToList<EntityModel.INovaMatrix>());
                    }
                    if (novaMatrixParmeter.novaMatrixType == NovaMatrixType.All || novaMatrixParmeter.novaMatrixType == NovaMatrixType.Management)
                    {
                        var matrix = new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(dataAccessTransaction).GetAll();
                        ProcessMatrix(ref novaMatrixResponse, compositeRoleMatrices, NovaMatrixType.Management, matrix.ToList<EntityModel.INovaMatrix>());
                    }
                    if (novaMatrixParmeter.novaMatrixType == NovaMatrixType.All || novaMatrixParmeter.novaMatrixType == NovaMatrixType.Exceptional)
                    {
                        List<EntityModel.NOVAExceptionalRoleMatrix> matrix = new RepositoryBase<EntityModel.NOVAExceptionalRoleMatrix>(dataAccessTransaction).GetAll().ToList();
                        matrix.ForEach(mat =>
                        {
                            //if (mat.IsExclusiveForUserAccount)
                            //{
                                mat.BWAccess = string.Empty;
                                mat.Position = mat.FLINSRoles;
                                mat.FLINSRoles = mat.FLINSRoles;
                                mat.ADGroups = string.Empty;

                            //}
                            //else
                            //{
                            //    mat.Position = mat.JobDescription;
                            //    mat.BWAccess = string.Empty; 
                            //}
                           
                        });
                        ProcessMatrix(ref novaMatrixResponse, compositeRoleMatrices, NovaMatrixType.Exceptional, matrix.ToList<EntityModel.INovaMatrix>());
                    }
                }
            }
            catch(Exception e) { throw e; }
            return novaMatrixResponse;
        }

        static void ProcessMatrix(ref NovaMatrixResponse novaMatrixResponse, List<EntityModel.NOVACompositeRoleMatrix> compositeRoleMatrices, NovaMatrixType type, List<EntityModel.INovaMatrix> novaMatrices)
        {
            var details = new List<NovaMatrixResponseDetail>();
            if (novaMatrices == null || novaMatrices.Count == 0) return;
            int indexer = novaMatrixResponse.NovaMatrixResponseDetails.Count();
            novaMatrices.ForEach(m =>
            {
                if (m.ValidTo != null && m.ValidTo < DateTime.Now) return;
                indexer = indexer + 1;
                var detail = new NovaMatrixResponseDetail()
                {
                    index = indexer,
                    novaMatrixType = type,
                    Department = m.Department,
                    Company = m.CompanyCode,
                    Section = m.SectionCode,
                    Position =  m.Position ,
                    ADGroup = splitRoles(m.ADGroups).Select(ad=>ad.Trim()).ToList(),
                    FlinsBiTCode = getRoles(compositeRoleMatrices, m.BWAccess).Select(f=>f.Trim()).ToList(),
                    FlinsTCode = getRoles(compositeRoleMatrices, m.FLINSRoles).Select(fb=>fb.Trim()).ToList(),
                    IsExclusiveForUserAccount = m.IsExclusiveForUserAccount,
                    JobDescription = m.JobDescription
                };
                details.Add(detail);
            });
            novaMatrixResponse.NovaMatrixResponseDetails.AddRange(details);
        }

        static List<string> splitRoles(string role)
        {
            return string.IsNullOrWhiteSpace(role) ? new List<string>() : role.Split(new char[] { ',' }).ToList();
        }

        static List<string> getRoles(List<EntityModel.NOVACompositeRoleMatrix> compositeRoleMatrices, string role)
        {
            var returnRoles = new List<string>();
            if (string.IsNullOrWhiteSpace(role)) return returnRoles;
            if (compositeRoleMatrices == null || compositeRoleMatrices.Count == 0)
            {
                returnRoles.AddRange(splitRoles(role));
                return returnRoles;
            }
            List<NOVACompositeRoleMatrix> compositeRoles = null;// compositeRoleMatrices.Where(c => c.CompositeRoleName == role).Select(c => c).ToList();
            if (compositeRoles != null && compositeRoles.Count > 0)
            {
                // isComposite = true;
                compositeRoles.ForEach(c =>
                {
                    returnRoles.AddRange(splitRoles(c.ModuleWiseRoles));
                });
            }

            if (returnRoles.Count == 0)
                returnRoles.AddRange(splitRoles(role));

            return returnRoles;
        }
        static List<SAPNovaTCodes> getRolesFromCompositeRoles(List<EntityModel.NOVACompositeRoleMatrix> compositeRoleMatrices, string role)
        {
            SAPNovaTCodes t;
            var returnRoles = new List<SAPNovaTCodes>();
            if (string.IsNullOrWhiteSpace(role)) return returnRoles;
            List<string> roles = splitRoles(role);
            roles.ForEach(s =>
            {
                var composite = compositeRoleMatrices.FirstOrDefault(c => c.CompositeRoleName == s && (!c.ValidTo.HasValue || c.ValidTo.Value >= DateTime.Now));
                if (composite == null)
                {
                    t = new SAPNovaTCodes(s);
                }
                else
                {
                    t = new SAPNovaTCodes(s) { IsCompositeRole = true, CompositeRoles = splitRoles(composite.ModuleWiseRoles) };
                }
                returnRoles.Add(t);
            });
            return returnRoles;
        }
        public static List<NOVAUserRoleMatrix> GetExisitingRequestAndRole(AdHocActionType adhocActionType, long userId, long endpointId, string SAPSection)
        {
            using (var trans = new DataAccessTransaction())
            {
                return ORMapList(new NOVAUserRoleMatrixRepository(trans).GetExisitingRequestAndRole(endpointId, userId, adhocActionType,SAPSection).ToList());
            }
        }

        public static List<string> GetSAPSection(AdHocActionType adhocActionType, long userId, long endpointId)
        {
            using (var trans = new DataAccessTransaction())
            {
                return new NOVAUserRoleMatrixRepository(trans).GetSAPSection(userId, adhocActionType, endpointId).ToList() ;
            }

        }


    }
}
