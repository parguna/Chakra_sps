using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationSAPUser
    {
        public long UserId { get; set; }
        public string UserLogonName { get; set; }
        public string DisplayName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Department { get; set; }
        public string Pers_No { get; set; }
        public string Function0 { get; set; }
        public Nullable<DateTime> lastLogon { get; set; }
        public string IsActive { get; set; }
        public string PasswordExpirationDate { get; set; }
        public string E_Mail { get; set; }
        public string Role { get; set; }
        public string HostName { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string UserType { get; set; }
        public bool IsUserLocked { get; set; }
        public string IsMapped { get; set; }
        public string Owner { get; set; }
        public string costcenter { get; set; }
        public string IsLocked { get; set; }
        public LockScenario LockScenario { get; set; }

        public static PresentationSAPUser ORMap(EntityModel.Presentation.PresentationSAPUser presentationADSOUsReportDataEntityModel)
        {
            if (presentationADSOUsReportDataEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPUser, PresentationSAPUser>(presentationADSOUsReportDataEntityModel);
        }

        public static List<PresentationSAPUser> ORMapList(IList<EntityModel.Presentation.PresentationSAPUser> presentationADSOUsReportDataEntityModels)
        {
            return presentationADSOUsReportDataEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationSAPUser> GetAllSAPUsersReportData(long EndpointSystemId)
        {
            IList<PresentationSAPUser> sapUserReportData = new List<PresentationSAPUser>();
            PresentationSAPUser sapUserReportItem = new PresentationSAPUser();
            IList<SAPUserinfo> sapUsers = new List<SAPUserinfo>();

            sapUsers = SAPUserinfo.GetAllSAPUserInfo(EndpointSystemId, "*");
            var ep = EndPoint.GetById(EndpointSystemId);

            foreach (SAPUserinfo usr in sapUsers)
            {
                User user = new User();
                string displayname = "";
                if (user != null)
                    displayname = user.DisplayName;
                sapUserReportItem = new PresentationSAPUser();
                sapUserReportItem.UserId = usr.Id;
                sapUserReportItem.UserLogonName = usr.UserLogonName;
                sapUserReportItem.DisplayName = string.Format("{0} {1}", usr.Firstname, usr.Lastname);
                sapUserReportItem.Firstname = usr.Firstname;
                sapUserReportItem.Lastname = usr.Lastname;
                sapUserReportItem.Department = usr.Department;
                sapUserReportItem.costcenter = usr.CostCenter;
                sapUserReportItem.costcenter = usr.CostCenter;
                sapUserReportItem.Pers_No = usr.Pers_No;
                sapUserReportItem.PasswordExpirationDate = usr.PASSWORDEXPTIME.HasValue ? usr.PASSWORDEXPTIME.Value.ToString() : string.Empty;
                sapUserReportItem.Function0 = usr.Function0;
                if (usr.LastLoginTime != null)
                {
                    sapUserReportItem.lastLogon = usr.LastLoginTime.Value;
                }
                sapUserReportItem.IsActive = usr.IsActive ? "Yes" : "No";
                sapUserReportItem.E_Mail = usr.E_Mail;
                sapUserReportItem.HostName = ep.DisplayName;
                sapUserReportItem.Role = "";
                sapUserReportItem.IsUserLocked = usr.IsUserLocked;
                sapUserReportItem.UserType = usr.SapUserType;
                if (usr.validFromDate != null)
                    sapUserReportItem.StartDate = usr.validFromDate.Value;
                if (usr.validToDate != null)
                    sapUserReportItem.EndDate = usr.validToDate.Value;
                sapUserReportData.Add(sapUserReportItem);
            }
            return sapUserReportData;
        }

    }
}
