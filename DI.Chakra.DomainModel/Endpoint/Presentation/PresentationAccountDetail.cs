using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAccountDetail
    {
        public string LoginName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string PhoneNo { get; set; }

        public string Email { get; set; } 

        public AccountType AccountType { get; set; }

        public bool IsMappedAccount { get; set; }

        public Nullable<DateTime> LastLogonDate { get; set; }

        public string Version { get; set; }

        public long? DIUserID { get; set; }

        public string UnMappedMapTo { get; set; }

        public bool IsPrimaryAccount { get; set; }

        public long EndpointUserId { get; set; }

        public long EndpointSystemId { get; set; }

        public string ApplicationType { get; set; }

        public string Application { get; set; }

        public static PresentationAccountDetail ORMap(EntityModel.Presentation.PresentationAccountDetail presentationAccountDetailEntityModel)
        {
            if (presentationAccountDetailEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAccountDetail, PresentationAccountDetail>(presentationAccountDetailEntityModel);
        }

        public static List<PresentationAccountDetail> ORMapList(IList<EntityModel.Presentation.PresentationAccountDetail> presentationAccountDetailEntityModels)
        {
            return presentationAccountDetailEntityModels.Select(ORMap).ToList();
        }

        public static PresentationAccountDetail GetByADEndPointUserId(long endpointUserId)
        {
            PresentationAccountDetail accountDetail = new PresentationAccountDetail();

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                ADSUserInfo adsUserInfo = ADSUserInfo.GetById(endpointUserId);

                if (adsUserInfo == null)
                    return accountDetail;
                accountDetail.LoginName = adsUserInfo.AccountName;
                accountDetail.DisplayName = adsUserInfo.displayName;
                accountDetail.Description = adsUserInfo.description;
                accountDetail.PhoneNo = adsUserInfo.homePhone;
                accountDetail.Email = adsUserInfo.mail;
                accountDetail.LastLogonDate = adsUserInfo.lastLogon;
                accountDetail.EndpointSystemId = adsUserInfo.EndPointSystemId;
                accountDetail.EndpointUserId = endpointUserId;

                return accountDetail;
            }
        }

        public void UpdateADAccountDetail()
        {
            User diUser = null;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                ADSUserInfo adsUserInfo = ADSUserInfo.GetByEndPointSystemId(this.EndpointUserId, this.EndpointSystemId);
                if (adsUserInfo != null)
                {
                    adsUserInfo.LastModifiedDate = DateTime.Today;
                    if (this.AccountType == AccountType.User && diUser != null)
                        diUser.UserName = adsUserInfo.sAMAccountName;
                    adsUserInfo.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
        }

        public static List<PresentationAccountDetail> GetPwdExpiryADUsers()
        {
            List<PresentationAccountDetail> presentationAccountDetailList = new List<PresentationAccountDetail>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<ADSUserInfo> adsUserInfo = ADSUserInfo.GetByPasswordExpirationDate();
                adsUserInfo.ForEach(lq =>
                {
                    presentationAccountDetailList.Add(new PresentationAccountDetail()
                    {
                        LoginName = lq.AccountName,
                        DisplayName = lq.displayName,
                        Description = lq.description,
                        PhoneNo = lq.homePhone,
                        Email = lq.mail,
                        LastLogonDate = lq.lastLogon,
                        EndpointSystemId = lq.EndPointSystemId,
                        EndpointUserId = lq.Id,
                        ApplicationType = Convert.ToString(EndpointType.ActiveDirectory),
                        Application = lq.EndPointSystem.DisplayName
                    });
                });
            }
            return presentationAccountDetailList;
        }

        public static List<PresentationAccountDetail> GetPwdExpirySAPUsers()
        {
            List<PresentationAccountDetail> presentationAccountDetailList = new List<PresentationAccountDetail>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<ADSUserInfo> adsuser = ADSUserInfo.GetByPasswordExpirationDate();
                adsuser.ForEach(lq =>
                {
                    presentationAccountDetailList.Add(new PresentationAccountDetail()
                    {
                        LoginName = lq.AccountName,
                        DisplayName = lq.displayName,
                        Description = lq.description,
                        PhoneNo = lq.homePhone,
                        Email = lq.mail,
                        LastLogonDate = lq.lastLogon,
                        EndpointSystemId = lq.EndPointSystemId,
                        EndpointUserId = lq.Id,
                        ApplicationType = Convert.ToString(EndpointType.SAP),
                        Application = lq.EndPointSystem.DisplayName
                    });
                });
            }
            return presentationAccountDetailList;
        }

        public static PresentationAccountDetail GetByInhouseEndPointUserId(long endpointUserId)
        {
            PresentationAccountDetail presentationAccountDetail = new PresentationAccountDetail();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                InhouseUserInfo inhouseUserInfo = InhouseUserInfo.GetById(endpointUserId);
                if (inhouseUserInfo == null)
                    return presentationAccountDetail;
                presentationAccountDetail.LoginName = inhouseUserInfo.AccountName;
                presentationAccountDetail.DisplayName = inhouseUserInfo.DisplayName;
                presentationAccountDetail.Email = inhouseUserInfo.Email;
                presentationAccountDetail.LastLogonDate = inhouseUserInfo.LastLoginDate;
                presentationAccountDetail.EndpointSystemId = inhouseUserInfo.EndPointSystemId;
                presentationAccountDetail.EndpointUserId = endpointUserId;
            }
            return presentationAccountDetail;
        }

        public void UpdateInhouseAccountDetail()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                InhouseUserInfo inhouseUserInfo = InhouseUserInfo.GetByEndPointSystemId(this.EndpointUserId, this.EndpointSystemId);
                if (inhouseUserInfo != null)
                {
                    inhouseUserInfo.LastModifiedDate = DateTime.Today;
                    inhouseUserInfo.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
        }

        public static PresentationAccountDetail GetBySAPEndPointUserId(long endpointUserId)
        {
            PresentationAccountDetail presentationAccountDetail = new PresentationAccountDetail();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                SAPUserinfo sapUserinfo = SAPUserinfo.GetById(endpointUserId);
                if (sapUserinfo == null)
                    return presentationAccountDetail;
                presentationAccountDetail.LoginName = sapUserinfo.AccountName;
                presentationAccountDetail.DisplayName = sapUserinfo.Firstname + " " + sapUserinfo.Lastname;
                presentationAccountDetail.PhoneNo = sapUserinfo.Pers_No;
                presentationAccountDetail.Email = sapUserinfo.E_Mail;
                presentationAccountDetail.LastLogonDate = sapUserinfo.LastLoginTime;
                presentationAccountDetail.EndpointSystemId = sapUserinfo.EndPointSystemId;
                presentationAccountDetail.EndpointUserId = endpointUserId;
            }
            return presentationAccountDetail;
        }

        public void UpdateSAPAccountDetail()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                SAPUserinfo sapUserinfo = SAPUserinfo.GetUserAccountByEndPointSystemId(this.EndpointUserId, this.EndpointSystemId);
                if (sapUserinfo != null)
                {
                    sapUserinfo.LastModifiedDate = DateTime.Today;
                    sapUserinfo.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
        }

        public static PresentationAccountDetail GetBySAPEPEndPointUserId(long endpointUserId)
        {
            PresentationAccountDetail presentationAccountDetail = new PresentationAccountDetail();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                SAPEPUserInfo sapepUserInfo = SAPEPUserInfo.GetById(endpointUserId);
                if (sapepUserInfo == null)
                    return presentationAccountDetail;
                presentationAccountDetail.LoginName = sapepUserInfo.AccountName;
                presentationAccountDetail.DisplayName = sapepUserInfo.DisplayName;
                presentationAccountDetail.PhoneNo = sapepUserInfo.Cellphone;
                presentationAccountDetail.Email = sapepUserInfo.Email;
                presentationAccountDetail.LastLogonDate = sapepUserInfo.LastModifiedDate;
                presentationAccountDetail.EndpointSystemId = sapepUserInfo.EndPointSystemId;
                presentationAccountDetail.EndpointUserId = endpointUserId;
            }
            return presentationAccountDetail;
        }

        public void UpdateSAPEPAccountDetail()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                SAPEPUserInfo sapepUserInfo = SAPEPUserInfo.GetByIdEndPointSystemId(this.EndpointUserId, this.EndpointSystemId);
                if (sapepUserInfo != null)
                {
                    sapepUserInfo.LastModifiedDate = DateTime.Today;
                    sapepUserInfo.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
        }

        //public static PresentationAccountDetail GetAccountDetailByEndPointUserId(long endpointUserId, EndpointType endpointType)
        //{
        //    PresentationAccountDetail presentationAccountDetail = new PresentationAccountDetail();
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        switch (endpointType)
        //        {
        //            case EndpointType.ActiveDirectory:
        //                presentationAccountDetail = PresentationAccountDetail.GetByADEndPointUserId(endpointUserId);
        //                break;
        //            case EndpointType.SAP:
        //                presentationAccountDetail = PresentationAccountDetail.GetBySAPEndPointUserId(endpointUserId);
        //                break;
        //            case EndpointType.LotusNotes:
        //                presentationAccountDetail = PresentationAccountDetail.GetByLotusNotesEndPointUserId(endpointUserId);
        //                break;
        //            case EndpointType.SAPEnterprisePortal:
        //                presentationAccountDetail = PresentationAccountDetail.GetBySAPEPEndPointUserId(endpointUserId);
        //                break;
        //            case EndpointType.InhouseSystems:
        //                presentationAccountDetail = PresentationAccountDetail.GetByInhouseEndPointUserId(endpointUserId);
        //                break;
        //            case EndpointType.Linux:
        //                presentationAccountDetail = PresentationAccountDetail.GetByLinuxEndPointUserId(endpointUserId);
        //                break;
        //            case EndpointType.OracleEBS:
        //                presentationAccountDetail = PresentationAccountDetail.GetByOEBSEndPointUserId(endpointUserId);
        //                break;

        //            case EndpointType.ORCLUM:
        //                presentationAccountDetail = PresentationAccountDetail.GetByOUMEndPointUserId(endpointUserId);
        //                break;
        //            default:
        //                break;

        //        }
        //        return presentationAccountDetail;
        //    }
        //}

        public static IList<PresentationAccountDetail> GetPwdExpiryUsers()
        {
            List<PresentationAccountDetail> Returnvalue = new List<PresentationAccountDetail>();

            var ADExpiredUser = PresentationAccountDetail.GetPwdExpiryADUsers();
            var SAPExpiredUser = PresentationAccountDetail.GetPwdExpirySAPUsers();

            Returnvalue.AddRange(ADExpiredUser);
            Returnvalue.AddRange(SAPExpiredUser);

            return Returnvalue;
        }
    }
}

