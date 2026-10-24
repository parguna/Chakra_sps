using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure.Enums;
using System.DirectoryServices;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using SapEpOldWebservice;

namespace DI.Chakra.Connector.SAPIP
{
    public class UserInfoOrchestrator
    {
        public long EndpointSystemId { get; set; }
        public string TenantId { get; set; }
        public int HistoryId { get; set; }
        public EndPoint EndPoint { get; set; }

        public IList<SAPEPUserInfo> GetUserInfo(ZZUserInformation[] zUserList) // IList<SAPEPUserInfoHistory> 
        {
            IList<SAPEPUserInfo> list = new List<SAPEPUserInfo>();
            SAPEPUserInfo sapEPUserInfo = null;

            try
            {
                foreach (ZZUserInformation zUser in zUserList)
                {
                    sapEPUserInfo = new SAPEPUserInfo();

                    sapEPUserInfo.TenantId = TenantId;
                    sapEPUserInfo.EndPointSystemId = EndpointSystemId;
                    //sapEPUserInfo.HistoryId = HistoryId;
                    sapEPUserInfo.UserName = zUser.UserName;
                    sapEPUserInfo.UniqueName = zUser.UniqueName;
                    sapEPUserInfo.FirstName = zUser.FirstName;
                    sapEPUserInfo.LastName = zUser.LastName;
                    sapEPUserInfo.Salutation = zUser.Salutation;
                    sapEPUserInfo.DisplayName = zUser.DisplayName;
                    sapEPUserInfo.Email = zUser.Email;
                    sapEPUserInfo.Department = zUser.Department;
                    sapEPUserInfo.JobTitle = zUser.JobTitle;
                    sapEPUserInfo.Title = zUser.Title;
                    sapEPUserInfo.Company = zUser.Company;
                    sapEPUserInfo.Cellphone = zUser.Cellphone;
                    sapEPUserInfo.Telephone = zUser.Telephone;
                    sapEPUserInfo.Fax = zUser.Fax;
                    sapEPUserInfo.Street = zUser.Street;
                    sapEPUserInfo.City = zUser.City;
                    sapEPUserInfo.State = zUser.State;
                    sapEPUserInfo.Country = zUser.Country;
                    sapEPUserInfo.Zip = zUser.Zip;
                    sapEPUserInfo.Currency = zUser.Currency;
                    sapEPUserInfo.Language = zUser.Language;
                    sapEPUserInfo.Password = zUser.Password;

                    if (null == zUser.UserLastLogOnTime)
                        sapEPUserInfo.UserLastLogOnTime = null;
                    else
                        sapEPUserInfo.UserLastLogOnTime = this.getDateFromStr(zUser.UserLastLogOnTime);

                    if (null == zUser.StartDate)
                        sapEPUserInfo.StartDate = null;
                    else
                        sapEPUserInfo.StartDate = this.getDateFromStr(zUser.StartDate);

                    if (null == zUser.EndDate)
                        sapEPUserInfo.EndDate = null;
                    else
                        sapEPUserInfo.EndDate = this.getDateFromStr(zUser.EndDate);


                    sapEPUserInfo.IsCompanyUser = zUser.IsCompanyUser;
                    sapEPUserInfo.UniqueId = zUser.UniqueId;

                    sapEPUserInfo.Extra1 = string.Empty;
                    sapEPUserInfo.Extra2 = string.Empty;
                    sapEPUserInfo.Extra3 = string.Empty;
                    sapEPUserInfo.CreatedBy = "Service";
                    sapEPUserInfo.CreatedDate = DateTime.Now;
                    sapEPUserInfo.LastModifiedBy = "Service";
                    sapEPUserInfo.LastModifiedDate = DateTime.Now;
                    sapEPUserInfo.Version = Convert.ToInt16(VersionNo.Available);
                    list.Add(sapEPUserInfo);
                }
            }

            catch (Exception ex)
            {
                
            }

            return list;
        }

        private DateTime? getDateFromStr(string dateStr)
        {
            if (!(!string.IsNullOrEmpty(dateStr) && dateStr.Length == 28))
            {
                return null;
            }

            int year;
            int month = 0;
            int day;

            int.TryParse(dateStr.Substring(24, 4), out year);
            int.TryParse(dateStr.Substring(8, 2), out day);

            switch (dateStr.Substring(4, 3).ToLower())
            {
                case "jan": month = 1;
                    break;
                case "feb": month = 2;
                    break;
                case "mar": month = 3;
                    break;
                case "apr": month = 4;
                    break;
                case "may": month = 5;
                    break;
                case "jun": month = 6;
                    break;
                case "jul": month = 7;
                    break;
                case "aug": month = 8;
                    break;
                case "sep": month = 9;
                    break;
                case "oct": month = 10;
                    break;
                case "nov": month = 11;
                    break;
                case "dec": month = 12;
                    break;
                default: return null;
            }

            DateTime date = new DateTime(year, month, day);
            return date;
        }
    }
}
