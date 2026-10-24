using System.Collections;
using DI.Chakra.Logger;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Security.Claims;

[assembly: OwinStartup(typeof(DI.Chakra.Web.Startup))]
namespace DI.Chakra.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls;
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                CookieManager = new Microsoft.Owin.Host.SystemWeb.SystemWebChunkingCookieManager(),
                AuthenticationMode = AuthenticationMode.Active
            });
            app.UseKentorOwinCookieSaver();
           // app.UseCookieAuthentication(new CookieAuthenticationOptions() {​​CookieManager = new Microsoft.Owin.Host.SystemWeb.SystemWebChunkingCookieManager(), AuthenticationMode = AuthenticationMode.Active });
            string clientId = System.Configuration.ConfigurationManager.AppSettings["ida:ClientId"];

            // RedirectUri is the URL where the user will be redirected to after they sign in.
            string redirectUri = System.Configuration.ConfigurationManager.AppSettings["ida:RedirectUri"];

            // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
            string tenant = System.Configuration.ConfigurationManager.AppSettings["ida:TenantId"];
            IdentityModelEventSource.ShowPII = true;
            // Authority is the URL for authority, composed by Microsoft identity platform endpoint and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
            string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/v2.0", tenant);
            app.UseOpenIdConnectAuthentication( new OpenIdConnectAuthenticationOptions
            {
                
                // Sets the ClientId, authority, RedirectUri as obtained from web.config
                ClientId = clientId,
                Authority = authority,
                RedirectUri = redirectUri,
                // PostLogoutRedirectUri is the page that users will be redirected to after sign-out. In this case, it is using the home page
                PostLogoutRedirectUri = redirectUri,
                Scope = OpenIdConnectScope.OpenId,
                // ResponseType is set to request the id_token - which contains basic information about the signed-in user
                ResponseType = OpenIdConnectResponseType.IdToken,
                // ValidateIssuer set to false to allow personal and work accounts from any organization to sign in to your application
                // To only allow users from a single organizations, set ValidateIssuer to true and 'tenant' setting in web.config to the tenant name
                // To allow users from only a list of specific organizations, set ValidateIssuer to true and use ValidIssuers parameter 
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                },
                ProtocolValidator = new OpenIdConnectProtocolValidator()
                {
                    RequireNonce = true,
                    RequireStateValidation = false
                },
                // OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = OnAuthenticationFailed,
                    RedirectToIdentityProvider = (context) =>
                    {
                        string login_hint = context.OwinContext.Authentication.AuthenticationResponseChallenge.Properties.Dictionary["login_hint"];
                        if (context.OwinContext.Response.StatusCode == 401 &&
                        context.OwinContext.Authentication.User.Identity.IsAuthenticated
                        )
                        {
                            DILog.UILogger.WriteLog(null, login_hint, LogLevelType.Info, "User is authenticated but does not have access", null);
                            context.OwinContext.Response.StatusCode = 403;
                            context.HandleResponse();
                        }
                        DILog.UILogger.WriteLog(null, login_hint, LogLevelType.Info, "AD Forgot Password: RedirectToIdentityProvider", null);
                        context.ProtocolMessage.SetParameter("acr_values", "mfa");
                        context.ProtocolMessage.LoginHint = login_hint;
                        return System.Threading.Tasks.Task.FromResult(0);
                    },
                    SecurityTokenReceived = (context1) =>
                    {
                        DILog.UILogger.WriteLog(null, "", LogLevelType.Info, "AD Forgot Password: SecurityTokenReceived", null);
                        return System.Threading.Tasks.Task.FromResult(0);
                    },
                    AuthorizationCodeReceived = (context1) =>
                    {
                        DILog.UILogger.WriteLog(null,"", LogLevelType.Info, "AD Forgot Password: AuthorizationCodeReceived", null);
                        return System.Threading.Tasks.Task.FromResult(0);
                    },
                    SecurityTokenValidated = n =>
                    {
                        DILog.UILogger.WriteLog(null, "", LogLevelType.Info, "AD Forgot Password: SecurityTokenValidated", null);
                        var id = n.AuthenticationTicket.Identity;
                        
                        //// we want to keep first name, last name, subject and roles
                        //var givenName = id.FindFirst(Constants.ClaimTypes.GivenName);
                        //var familyName = id.FindFirst(Constants.ClaimTypes.FamilyName);
                        //var sub = id.FindFirst(Constants.ClaimTypes.Subject);
                        //var roles = id.FindAll(Constants.ClaimTypes.Role);

                        //// create new identity and set name and role claim type
                        var nid = new ClaimsIdentity(
                            id.AuthenticationType);

                        nid.AddClaims(id.Claims);
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        //nid.AddClaim(new Claim("access_Token", n.ProtocolMessage.AccessToken));
                        var str = string.Empty;
                        if (n.AuthenticationTicket.Properties.Dictionary.TryGetValue("login_hint", out str))
                        {
                            nid.AddClaim(new Claim("IDMSUserName", str));
                        }
                        
                        ////nid.AddClaim(givenName);
                        ////nid.AddClaim(familyName);
                        ////nid.AddClaim(sub);
                        ////nid.AddClaims(roles);

                        ////// add some other app specific claim
                        // Connect to you ASP.NET database for example
                        ////nid.AddClaim(new Claim("app_specific", "some data"));

                        //// keep the id_token for logout
                        //nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);

                        return System.Threading.Tasks.Task.FromResult(0);
                    }
                }
            });
        }
        private System.Threading.Tasks.Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            if (context.Exception.Message?.IndexOf("IDX21323",StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                DILog.UILogger.WriteLog(null, "", LogLevelType.Info, $"Nonce cookie validation failed. Exception:{context.Exception.Message}", null);
                context.SkipToNextMiddleware();
                //context.OwinContext.Authentication.Challenge();
                //return System.Threading.Tasks.Task.FromResult(0);
            }
            else
            {
                DILog.UILogger.WriteLog(null, "", LogLevelType.Info, context.Exception.Message, null);
                context.Response.Redirect(System.Web.HttpUtility.UrlDecode($"{System.Configuration.ConfigurationManager.AppSettings["ida:ErrorRedirect"]}"));
            }
            return System.Threading.Tasks.Task.FromResult(0);
        }
        private System.Threading.Tasks.Task OnMessageReceived(MessageReceivedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            DILog.UILogger.WriteLog(null, "", LogLevelType.Info, context.Response.Body.ToString(), null);
            //System.Web.Htt Response.Redirect("../../login.aspx" + context.Exception.Message);
            return System.Threading.Tasks.Task.FromResult(0);
        }
        private System.Threading.Tasks.Task AuthorizationCodeReceived(AuthorizationCodeReceivedNotification context)
        {
            DILog.UILogger.WriteLog(null, "", LogLevelType.Info, context.Response.Body.ToString(), null);
            //System.Web.Htt Response.Redirect("../../login.aspx" + context.Exception.Message);
            return System.Threading.Tasks.Task.FromResult(0);
        }
    }
}