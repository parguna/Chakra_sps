using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.Model.SAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DataAccess.Implementation
{
    public class UserAccountDA
    {
        
        public IList<UserAccount> GetUserAccountDetail(int UserID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            try
            {
                


                return Listtobereturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<UserEntitlement> GetEntitlementDetail(long EndpointUserId, EndpointType ApplicationType)
        {
            IList<UserEntitlement> Listtobereturn = new List<UserEntitlement>();
            try
            {
                switch(ApplicationType)
                {
                    case EndpointType.ActiveDirectory:
                        new ActiveDirectoryDA().GetEntitlementDetail(EndpointUserId).ToList().ForEach(lq =>
                        {
                            Listtobereturn.Add(lq);
                        });
                        break;
                    case EndpointType.SAP:
                        new SAPConnectorDA().GetEntitlementDetail(EndpointUserId).ToList().ForEach(lq =>
                        {
                            Listtobereturn.Add(lq);
                        });
                        break;
                    case EndpointType.LotusNotes:
                        new LotusNotesDA().GetEntitlementDetail(EndpointUserId).ToList().ForEach(lq =>
                        {
                            Listtobereturn.Add(lq);
                        });
                        break;
                    case EndpointType.SAPEnterprisePortal:
                        new SAPEPDA().GetEntitlementDetail(EndpointUserId).ToList().ForEach(lq =>
                        {
                            Listtobereturn.Add(lq);
                        });
                        break;
                    case EndpointType.InhouseSystems:
                        //new InhouseSystemDA().GetEntitlementDetail(EndpointUserId).ToList().ForEach(lq =>
                        //{
                        //    Listtobereturn.Add(lq);
                        //});
                        break;

                    case EndpointType.Linux:
                        new LinuxDA().GetEntitlementDetail(EndpointUserId).ToList().ForEach(lq =>
                            {
                                Listtobereturn.Add(lq);
                            });
                        break;


                    case EndpointType.OracleEBS:
                        new OracleEBSDA().GetEntitlementDetail(EndpointUserId).ToList().ForEach(lq =>
                        {
                            Listtobereturn.Add(lq);
                        });
                        break;

                    case EndpointType.ORCLUM:
                        new ORCLUMDA().GetEntitlementDetail(EndpointUserId).ToList().ForEach(lq =>
                        {
                            Listtobereturn.Add(lq);
                        });
                        break;

                    
                    default:
                        break;
                }
                return Listtobereturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<UserAccount> GetUserAccountDetailByApplication(int ApplicationID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            try
            {
                new ActiveDirectoryDA().GetUserAccountDetailByApplication(ApplicationID).ToList().ForEach(lq =>
                {
                    Listtobereturn.Add(lq);
                });
                new SAPConnectorDA().GetUserAccountDetailByApplication(ApplicationID).ToList().ForEach(lq =>
                {
                    Listtobereturn.Add(lq);
                });
                new LotusNotesDA().GetUserAccountDetailByApplication(ApplicationID).ToList().ForEach(lq =>
                {
                    Listtobereturn.Add(lq);
                });
                new SAPEPDA().GetUserAccountDetailByApplication(ApplicationID).ToList().ForEach(lq =>
                {
                    Listtobereturn.Add(lq);
                });
                new InhouseSystemDA().GetUserAccountDetailByApplication(ApplicationID).ToList().ForEach(lq =>
                {
                    Listtobereturn.Add(lq);
                });

                new LinuxDA().GetUserAccountDetailByApplication(ApplicationID).ToList().ForEach(lq =>
                    {
                        Listtobereturn.Add(lq);
                    });

                new OracleEBSDA().GetUserAccountDetailByApplication(ApplicationID).ToList().ForEach(lq =>
                {
                    Listtobereturn.Add(lq);
                });


                new ORCLUMDA().GetUserAccountDetailByApplication(ApplicationID).ToList().ForEach(lq =>
                  {
                      Listtobereturn.Add(lq);

                     });
                return Listtobereturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AccountDetail GetAccountDetailByEndPointUserID(int EndpointUserId, EndpointType ApplicationType)
        {
            AccountDetail AccountDetail = new AccountDetail();
            try
            {
                switch (ApplicationType)
                {
                    case EndpointType.ActiveDirectory:
                        AccountDetail = new ActiveDirectoryDA().GetAccountDetailByEndPointUserID(EndpointUserId);
                        break;
                    case EndpointType.SAP:
                        AccountDetail = new SAPConnectorDA().GetAccountDetailByEndPointUserID(EndpointUserId);
                        break;
                    case EndpointType.LotusNotes:
                        AccountDetail = new LotusNotesDA().GetAccountDetailByEndPointUserID(EndpointUserId);
                        break;
                    case EndpointType.SAPEnterprisePortal:
                        AccountDetail = new SAPEPDA().GetAccountDetailByEndPointUserID(EndpointUserId);
                        break;
                    case EndpointType.InhouseSystems:
                        AccountDetail = new InhouseSystemDA().GetAccountDetailByEndPointUserID(EndpointUserId);
                        break;
                    case EndpointType.Linux:
                        AccountDetail = new LinuxDA().GetAccountDetailByEndPointUserID(EndpointUserId);
                        break;
                    case EndpointType.OracleEBS:
                           AccountDetail = new OracleEBSDA().GetAccountDetailByEndPointUserID(EndpointUserId);
                        break;

                    case EndpointType.ORCLUM:
                        AccountDetail = new ORCLUMDA().GetAccountDetailByEndPointUserID(EndpointUserId);
                        break;
                    default:
                        break;
                    
                }
                return AccountDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public IList<AccountDetail> GetPwdExpiryUsers()
        {
            List<AccountDetail> Returnvalue = new List<AccountDetail>();
            var ADExpiredUser = new ActiveDirectoryDA().GetPwdExpiryUsers();
            var SAPExpiredUser = new SAPConnectorDA().GetPwdExpiryUsers();
            Returnvalue.AddRange(ADExpiredUser);
            Returnvalue.AddRange(SAPExpiredUser);
            return Returnvalue;
        }
    }
}
