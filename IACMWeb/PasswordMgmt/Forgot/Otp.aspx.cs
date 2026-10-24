using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using DI.Chakra.Infrastructure;
using Application;
using System.Text;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System.Threading;

namespace DI.Chakra.Web.PasswordMgmt.Forgot
{
    public partial class Otp : FrontUIPage
    {
        long userID;
        TimeSpan sendtime;
        TimeSpan entertime;
        private SessionStatusImpl sessionStatus;

        protected void Page_Load(object sender, EventArgs e)
        {
            sessionStatus = new SessionStatusImpl();
            User user = sessionStatus.User;

            if (Session["MaxAttReach"] != null && Session["MaxAttReach"].ToString() == "MaxAttReach")
            {
                Response.Redirect("~/Login.aspx");
            }
            try
            {
                string UserName = string.Empty;
                if (sessionStatus.User != null)
                    UserName = sessionStatus.User.UserName;
                if (Request.QueryString["UserName"] != null)
                {

                    UserName = Request.QueryString["UserName"].ToString();
                    if (UserName.ToUpper() == "IACMADMIN")
                    {
                        Response.Redirect("~/Login.aspx");
                    }
                    if (sessionStatus.User != null)
                        sessionStatus.User.UserName = Request.QueryString["UserName"].ToString();
                }

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("name", UserName);

                user = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.ForgotUser, parameter);
                sessionStatus.User = user;
                user.Password = RSAHelper.EncryptData(user.Password);

                if (user != null)
                    sessionStatus.User.Id = user.Id;
                userID = sessionStatus.UserId;

                if (!IsPostBack)
                {
                    //Generate Random 6 digit otp and send to User email /Mobile
                    if (sessionStatus.NoofAttempts == -1)
                    {
                        string OTP = this.GenerateRandomNumber();

                        user.Reserved1 = OTP;
                        DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.SaveUser,null,user);

                        if (MasterConfigHelper.OTPOption == OTPOption.SMS.ToString())
                        {
                            SendOTPtoMobile(OTP);
                        }
                        else
                        {
                            SendOTPbyEmail(OTP);
                            Dictionary<string, string> param = new Dictionary<string, string>();
                            DomainApiClient<EmailInfo>.GetByAction<bool>(DomainApiAction.Email.SendOTP.SendOTPEmail, param);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var ans = txtOTP.Text;
            bool correct = false;

            sendtime = sessionStatus.User.LastModifiedDate.Value.TimeOfDay;
            entertime = DateTime.Now.TimeOfDay;
            var diff = entertime-sendtime;

            if (diff.TotalMinutes > 1)
            {
                MasterPage.ShowAlert(AlertType.Warning, "OTP Expired");
                Thread.Sleep(5000);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "PageScript", "Redirect();",true);
                return;
            }

            if (ans == sessionStatus.User.Reserved1)
            {
                if (DateTime.Now <= sessionStatus.User.LastModifiedDate.Value.AddMinutes(5))
                {
                    correct = true;
                    if (correct)
                    {
                        if (new SessionStatusImpl().ForgotPwdType == ForgotPwdType.ForgotPassword)
                        {
                            Server.Transfer("ResetPassword.aspx");
                        }
                        else
                        {
                            Dictionary<string, string> parameter = new Dictionary<string, string>();
                            parameter.Add("userName", sessionStatus.User.UserName);
                            bool bReturn = DomainServiceClient.GetByAction<bool>
                                 (DomainServiceAction.ActiveDirectoryService.RootName, DomainServiceAction.ActiveDirectoryService.UnlockAccount, parameter);

                            if (bReturn == true)
                            {
                                MasterPage.ShowAlert(AlertType.Success, "Account Unlocked successfully");
                                Thread.Sleep(5000);

                                Response.Redirect("~/Login.aspx");
                            }


                        }
                    }
                }
            }
            else
            {
                sessionStatus.NoofAttempts = Convert.ToInt32(sessionStatus.NoofAttempts) + 1;
                if (Convert.ToInt32(sessionStatus.NoofAttempts) >= 3)
                {
                    Session["MaxAttReach"] = "MaxAttReach";
                    Response.Redirect("~/Login.aspx");
                }

                MasterPage.ShowAlert(AlertType.Error, "You entered wrong OTP");
                // pnlInvalid.Visible = true;
            }
        }


        #region  Private Methods

        private string GenerateRandomNumber()
        {
            Random rnd = new Random();
            int myRandomNo = rnd.Next(100000, 999999);
            return Convert.ToString(myRandomNo);
        }

        public void SendOTPbyEmail(string OTP)
        {
            string strMailTo = string.Empty;
            try
            {
                Dto.EmailInfo emailInfo = new Dto.EmailInfo();

                List<Dto.MailServerConfiguration> mailServerConfigurationList = DomainApiClient<Dto.MailServerConfiguration>.GetByAction<List<Dto.MailServerConfiguration>>(DomainApiAction.Email.MailServerConfiguration.GetAllConfig,null);
                if (mailServerConfigurationList.Count <= 0)
                {
                    sessionStatus.ReturnErrorMessage = "Failed to get E-Mail server configuration. Please contact IACM Administrator.";
                    Response.Redirect("ForgotUser.aspx");
                }

                var listmailconfig = mailServerConfigurationList.First();
                string mailContent = string.Empty;
                string mailSubject = string.Empty;
                Dictionary<string, string> Para = new Dictionary<string, string>();
                Para.Add("userId", sessionStatus.UserId.ToString());
                User user = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByUsrId,Para);
                strMailTo = user.EmailId;

                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("userId", user.Id.ToString());
                DomainApiClient<EmailInfo>.GetByAction<bool>(DomainApiAction.Email.SendOTP.EmailCompose, param);

            }
            catch (Exception ex)
            {
                //  Log.Provisioning.WriteMessage(LogLevel.ERROR, string.Format("Notification of OTP  failed"), ex);
                throw;
            }

        }

       

        public void SendOTPtoMobile(string OTP)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("userId", sessionStatus.UserId.ToString());
                var responseMessage = DomainApiClient<Dto.User>.GetByAction<string>("SendSMS", param);

                if (!string.IsNullOrEmpty(responseMessage))
                {
                    sessionStatus.ReturnErrorMessage = responseMessage;
                    Response.Redirect("ForgotUser.aspx");
                }
                //SMSService.Send(user.MobileNumber, message);

            }
            catch (Exception ex)
            {
                // Log.Provisioning.WriteMessage(LogLevel.ERROR, string.Format("Notification of OTP  failed"), ex);
                throw;
            }

        }


        #endregion 
    }
}