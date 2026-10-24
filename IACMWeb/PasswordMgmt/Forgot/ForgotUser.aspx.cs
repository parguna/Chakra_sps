using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Master;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Logger;
using System.Configuration;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.OpenIdConnect;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;

namespace DI.Chakra.Web.PasswordMgmt.Forgot
{
    public partial class ForgotUser : FrontUIPage
    {
        SessionStatusImpl session = new SessionStatusImpl();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (session.ReturnErrorMessage != string.Empty)
            {
                MasterPage.ShowAlert(AlertType.Error, session.ReturnErrorMessage);
                session.ReturnErrorMessage = string.Empty;
            }

            /*if (Request.QueryString["type"].ToString() == "unlock")
            {
                btnForgot.Text = "UnLock Account";
                new SessionStatusImpl().ForgotPwdType = ForgotPwdType.Unlock;
            }
            else*/
            {
                btnForgot.Text = "Forgot Password";
                new SessionStatusImpl().ForgotPwdType = ForgotPwdType.ForgotPassword;

            }
        }

        protected void btnForgot_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("name", txtUserName.Text);
            User usr = DomainApiClient<User>.GetByAction<User>(DomainApiAction.User.ForgotUser, parameter);

            if (usr == null && false)
            {
                MasterPage.ShowAlert(AlertType.Error, "User Does Not Exist");

            }
            else
            {

                //Dictionary<string, string> param = new Dictionary<string, string>();
                //param.Add("userName", txtUserName.Text);
                //// string responseMessage = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.GetUserIsActive, parameter);
                //string responseMessage = DomainApiClient<Dto.User>.GetByAction<string>("GetUserIsActive", param);


                //LoginHelper loginHelper = new LoginHelper();
                //string exceptionMessage = loginHelper.IsADActiveAccount(txtUserName.Text,responseMessage);

                //if (!string.IsNullOrEmpty(responseMessage))
                //{
                //    MasterPage.ShowAlert(AlertType.Error, responseMessage);
                //    //DILog.ProvisioningLogger.WriteLog(LogLevelType.Error, string.Format("Authenticated failed for user {0}", txtUserName.Text), ex);
                //    return;
                //}
                //else if (!string.IsNullOrEmpty(exceptionMessage))
                //{
                //    MasterPage.ShowAlert(AlertType.Error, exceptionMessage);
                //    DILog.ProvisioningLogger.WriteLog(LogLevelType.Error, string.Format(exceptionMessage, txtUserName.Text), null);
                //    return;
                //}
                //else
                {
                    session.User = usr;
                    if (MasterConfigHelper.PasswordResetOption == "OTP")
                    {
                        if (MasterConfigHelper.OTPOption == "SMS")
                        {
                            if (string.IsNullOrEmpty(usr.MobileNumber))
                            {
                                MasterPage.ShowAlert(AlertType.Error, "User Mobile Number Not Found");
                                return;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(usr.EmailId))
                            {
                                MasterPage.ShowAlert(AlertType.Error, "User EmailId Not Found");
                                return;
                            }
                        }
                        Response.Redirect("OTP.aspx");
                    }
                    else if (MasterConfigHelper.PasswordResetOption?.ToUpper() == "MFA")
                    {
                        
                        //var con = ConfigurationManager.AppSettings["ida:RedirectUri"];
                        //var t = new AuthenticationProperties { RedirectUri = con, };
                        //t.Dictionary.Add("login_hint", ($"{txtUserName.Text}@outlook.com"));
                        //HttpContext.Current.GetOwinContext().Authentication.Challenge(t, OpenIdConnectAuthenticationDefaults.AuthenticationType);
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Please contact Administrator.");
                        //Dictionary<string, string> gparameter = new Dictionary<string, string>();
                        //gparameter.Add("userId", session.User.Id.ToString());

                        //List<GlobalAnswer> globalAnswers = DomainApiClient<Dto.GlobalAnswer>.GetByAction<List<Dto.GlobalAnswer>>(DomainApiAction.SecurityQA.GlobalAnswer.GetByUser, gparameter);


                        //if (globalAnswers.Count > 0)
                        //{
                        //    Response.Redirect("SecurityQuestion.aspx");
                        //}
                        //else
                        //{
                        //    MasterPage.ShowAlert(AlertType.Error, "User Does Not Have Any Security Questions configured to get the Password. Please contact Administrator.");
                        //}
                    }
                }
               
            }
        }
        protected void RedirectPost(string url, IEnumerable<KeyValuePair<string, string>> fields)
        {
            Response.Clear();

            const string template =
            @"<html>
            <body onload='document.forms[""form""].submit()'>
            <form name='form' action='{0}' method='post'>
            {1}
            </form>
            </body>
            </html>";

            var fieldsSection = string.Join(
                    Environment.NewLine,
                    fields.Select(x => $"<input type='hidden' name='{HttpUtility.UrlEncode(x.Key)}' value='{HttpUtility.UrlEncode(x.Value)}'>")
                );

            var html = string.Format(template, HttpUtility.UrlEncode(url), fieldsSection);

            Response.Write(html);

            Response.End();
        }

        private const string JsonDataFieldName = "_jsonData";

        protected void RedirectPost<T>(string url, T bodyPayload)
        {
            var json = JsonConvert.SerializeObject(bodyPayload, Formatting.Indented);
            //explicit type declaration to prevent recursion
            IEnumerable<KeyValuePair<string, string>> postFields = new List<KeyValuePair<string, string>>()
                {new KeyValuePair<string, string>(JsonDataFieldName, json)};

            RedirectPost(url, postFields);

        }

        protected T GetPostData<T>() where T : class
        {
            var urlEncodedFieldData = Request.Params[JsonDataFieldName];
            if (string.IsNullOrEmpty(urlEncodedFieldData))
            {
                return null;// default(T);
            }

            var fieldData = HttpUtility.UrlDecode(urlEncodedFieldData);

            var result = JsonConvert.DeserializeObject<T>(fieldData);
            return result;
        }

    }
}
