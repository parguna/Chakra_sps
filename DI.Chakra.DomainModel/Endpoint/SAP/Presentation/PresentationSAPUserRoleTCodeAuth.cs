using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationSAPUserRoleTCodeAuth
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Department { get; set; }

        public bool? IsActive { get; set; }

        public string RoleCode { get; set; }

        public string TCode { get; set; }

        public string AuthorizationCode { get; set; }

        public string ProfileCode { get; set; }



        public static PresentationSAPUserRoleTCodeAuth ORMap(EntityModel.Presentation.PresentationSAPUserRoleTCodeAuth presentationSAPUserRoleTCodeAuth)
        {
            if (presentationSAPUserRoleTCodeAuth == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationSAPUserRoleTCodeAuth, PresentationSAPUserRoleTCodeAuth>(presentationSAPUserRoleTCodeAuth);
        }

        public static List<PresentationSAPUserRoleTCodeAuth> ORMapList(IList<EntityModel.Presentation.PresentationSAPUserRoleTCodeAuth> presentationSAPUserRoleTCodeAuths)
        {
            return presentationSAPUserRoleTCodeAuths.Select(ORMap).ToList();
        }

        public static List<string> GetListSAPTCode(string roleCode, string objectCode, string fieldCode)
        {
            using (DataAccessTransaction dataAccesstransaction = new DataAccessTransaction())
            {
                return new SAPRoleToAuthMapRepository(dataAccesstransaction).GetListSAPTCode(roleCode, objectCode, fieldCode);
            }
        }
        public static List<string> LstRoleToAuth(string roleCode, string objectCode)
        {
            using (DataAccessTransaction dataAccesstransaction = new DataAccessTransaction())
            {
                return new SAPRoleToAuthMapRepository(dataAccesstransaction).LstRoleToAuth(roleCode, objectCode);
            }
        }


        public IList<PresentationSAPUserRoleTCodeAuth> GetSapUserRoleTCodeAuthReport(long endpointSystemId, int userId)
        {
            
                using ( DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {

                    IList<PresentationSAPUsersToRolesReport> lstUsersRoles = PresentationSAPUsersToRolesReport.GetSAPUserToRolesReportData(UserId);

                    List<PresentationSAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<PresentationSAPUserRoleTCodeAuth>();


                    foreach (PresentationSAPUsersToRolesReport SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = PresentationSAPUserRoleTCodeAuth.GetListSAPTCode(SapUserRole.RoleCode, "S_TCODE", "TCD");
                        foreach (string SapRoleAuth in lstSapRoleTCode)
                        {
                            List<string> lstRoleToAuth = PresentationSAPUserRoleTCodeAuth.LstRoleToAuth(SapUserRole.RoleCode, "S_TCODE");
                            foreach (string SapRoleTCodeAuth in lstRoleToAuth)
                            {

                                PresentationSAPUserRoleTCodeAuth SapUserRoleTCodeAuth = new PresentationSAPUserRoleTCodeAuth();
                                SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                                SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                                SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                                SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                                SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                                SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                                SapUserRoleTCodeAuth.TCode = SapRoleAuth;
                                SapUserRoleTCodeAuth.AuthorizationCode = SapRoleTCodeAuth;

                                if (SapUserRoleTCodeAuth.ProfileCode == string.Empty)
                                {
                                    //SAPAuthorizationProfile 
                                }

                                lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);
                            }


                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
           

        }
        public IList<PresentationSAPUserRoleTCodeAuth> GetSapUserRoleTCodeAuthReport(int userId, long endpointSystemId, bool withoutAuthrization)
        {
            
                PresentationSAPUserRoleTCodeAuth SapUserRoleTCodeAuth = null;
               using ( DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
               {

                   IList<PresentationSAPUsersToRolesReport> lstUsersRoles = PresentationSAPUsersToRolesReport.GetSAPUserToRolesReportData(userId, endpointSystemId);

                    List<PresentationSAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<PresentationSAPUserRoleTCodeAuth>();


                    foreach (PresentationSAPUsersToRolesReport SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = PresentationSAPUserRoleTCodeAuth.GetListSAPTCode(SapUserRole.RoleCode, "S_TCODE", "TCD");
                        foreach (string tCode in lstSapRoleTCode)
                        {

                            SapUserRoleTCodeAuth = new PresentationSAPUserRoleTCodeAuth();
                            SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                            SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                            SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                            SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                            SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                            SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                            SapUserRoleTCodeAuth.TCode = tCode;
                            lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);



                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
          
        }
        public IList<PresentationSAPUserRoleTCodeAuth> GetSapUserTCodeReport(int userId, long endpointSystemId)
        {
          
                PresentationSAPUserRoleTCodeAuth SapUserRoleTCodeAuth = null;
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {

                    IList<PresentationSAPUsersToRolesReport> lstUsersRoles = PresentationSAPUsersToRolesReport.GetSAPUserToRolesReportData(userId);

                    List<PresentationSAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<PresentationSAPUserRoleTCodeAuth>();


                    foreach (PresentationSAPUsersToRolesReport SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = PresentationSAPUserRoleTCodeAuth.GetListSAPTCode(SapUserRole.RoleCode, "S_TCODE", "TCD");
                           

                        foreach (string SapRoleAuth in lstSapRoleTCode)
                        {
                            List<string> lstRoleToAuth = PresentationSAPUserRoleTCodeAuth.LstRoleToAuth(SapUserRole.RoleCode, "S_TCODE");


                            SapUserRoleTCodeAuth = new PresentationSAPUserRoleTCodeAuth();
                            SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                            SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                            SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                            SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                            SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                            SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                            SapUserRoleTCodeAuth.TCode = SapRoleAuth;
                            lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);



                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
          

        }
        public IList<PresentationSAPUserRoleTCodeAuth> GetSapUserRoleTCodeAuthReport(long endpointSystemId)
        {
           using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {

                    IList<PresentationSAPUsersToRolesReport> lstUsersRoles = PresentationSAPUsersToRolesReport.GetSAPUserToRolesReportData(endpointSystemId);

                    List<PresentationSAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<PresentationSAPUserRoleTCodeAuth>();


                    foreach (PresentationSAPUsersToRolesReport SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = PresentationSAPUserRoleTCodeAuth.GetListSAPTCode(SapUserRole.RoleCode, "S_TCODE", "TCD");
                        foreach (string SapRoleAuth in lstSapRoleTCode)
                        {
                            List<string> lstRoleToAuth = PresentationSAPUserRoleTCodeAuth.LstRoleToAuth(SapUserRole.RoleCode, "S_TCODE");
                                

                            foreach (string SapRoleTCodeAuth in lstRoleToAuth)
                            {

                                PresentationSAPUserRoleTCodeAuth SapUserRoleTCodeAuth = new PresentationSAPUserRoleTCodeAuth();
                                SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                                SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                                SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                                SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                                SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                                SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                                SapUserRoleTCodeAuth.TCode = SapRoleAuth;
                                SapUserRoleTCodeAuth.AuthorizationCode = SapRoleTCodeAuth;

                                if (SapUserRoleTCodeAuth.ProfileCode == string.Empty)
                                {
                                    //SAPAuthorizationProfile 
                                }

                                lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);
                            }


                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
           
        }
        public IList<PresentationSAPUserRoleTCodeAuth> GetSapUserTCodeReport(long endpointSystemId)
        {
            PresentationSAPUserRoleTCodeAuth SapUserRoleTCodeAuth = null;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {

                    IList<PresentationSAPUsersToRolesReport> lstUsersRoles = PresentationSAPUsersToRolesReport.GetSAPUserToRolesReportData(endpointSystemId);

                    List<PresentationSAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<PresentationSAPUserRoleTCodeAuth>();


                    foreach (PresentationSAPUsersToRolesReport SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = PresentationSAPUserRoleTCodeAuth.GetListSAPTCode(SapUserRole.RoleCode, "S_TCODE", "TCD");
                            

                        foreach (string SapRoleAuth in lstSapRoleTCode)
                        {

                            SapUserRoleTCodeAuth = new PresentationSAPUserRoleTCodeAuth();
                            SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                            SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                            SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                            SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                            SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                            SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                            SapUserRoleTCodeAuth.TCode = SapRoleAuth;

                            lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);

                            if (lstSapUserRoleTCodeAuth.Count() % 100 == 0)
                                break;

                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
         

        }

        public static IList<PresentationSapUserRoleTCodeAuthorization> GetSapUserRoleTCodeAuthorization(long taskId)
        {
            var task = DomainModel.Task.GetAttestationTask(taskId, true);
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AttestationRepository(dataAccessTransaction).GetSapUserRoleTCodeAuthorization(ORMapTask(task)));
            }
        }

        public static List<PresentationSapUserRoleTCodeAuthorization> ORMapList(IList<EntityModel.Presentation.PresentationSapUserRoleTCodeAuthorization> presentationSapUserRoleTCodeAuthorizations)
        {
            return presentationSapUserRoleTCodeAuthorizations.Select(ORMap).ToList();
        }
        public static PresentationSapUserRoleTCodeAuthorization ORMap(EntityModel.Presentation.PresentationSapUserRoleTCodeAuthorization presentationSapUserRoleTCodeAuthorization)
        {
            if (presentationSapUserRoleTCodeAuthorization == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSapUserRoleTCodeAuthorization, PresentationSapUserRoleTCodeAuthorization>(presentationSapUserRoleTCodeAuthorization);
        }
        public static EntityModel.Task ORMapTask(DomainModel.Task task)
        {
            if (task == null)
                return null;
            return Mapper.Map<Task, EntityModel.Task>(task);
        }

    }
}
