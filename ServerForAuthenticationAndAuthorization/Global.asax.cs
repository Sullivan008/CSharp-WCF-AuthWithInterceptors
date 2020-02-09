using ServerForAuthenticationAndAuthorization.Validator;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.ApplicationServices;
using System.Web.Security;

namespace ServerForAuthenticationAndAuthorization
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            AuthenticationService.Authenticating +=
                AuthenticationService_Authenticating;
        }

        private void AuthenticationService_Authenticating(object sender, AuthenticatingEventArgs e)
        {
            string roles = string.Empty;

            e.Authenticated = UserIsValid(e, ref roles);
            e.AuthenticationIsComplete = true;

            if (e.Authenticated)
            {
                string encryptedValue = FormsAuthentication.Encrypt(CreateFormsAuthenticationTicket(e, roles));

                OperationContext.Current.OutgoingMessageProperties[HttpResponseMessageProperty.Name] =
                    SetSetCookieInResponseChannelHeaders(encryptedValue);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        { }

        protected void Application_BeginRequest(object sender, EventArgs e)
        { }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        { }

        protected void Application_Error(object sender, EventArgs e)
        { }

        protected void Session_End(object sender, EventArgs e)
        { }

        protected void Application_End(object sender, EventArgs e)
        { }

        #region PRIVATE Helper Methods

        private HttpResponseMessageProperty SetSetCookieInResponseChannelHeaders(string cookieValue)
        {
            HttpResponseMessageProperty response = new HttpResponseMessageProperty();
            response.Headers[HttpResponseHeader.SetCookie] = FormsAuthentication.FormsCookieName + "=" + cookieValue;

            return response;
        }

        private FormsAuthenticationTicket CreateFormsAuthenticationTicket(AuthenticatingEventArgs e, string roles) =>
            new FormsAuthenticationTicket(1,
                e.UserName,
                DateTime.Now,
                DateTime.Now.AddHours(24),
                true,
                roles,
                FormsAuthentication.FormsCookiePath);

        private bool UserIsValid(AuthenticatingEventArgs credentials, ref string roles) =>
            new UserValidator().IsValidUser(credentials, ref roles);

        #endregion
    }
}