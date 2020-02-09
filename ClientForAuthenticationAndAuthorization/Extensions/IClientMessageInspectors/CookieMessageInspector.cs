using ClientForAuthenticationAndAuthorization.Dialogs;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace ClientForAuthenticationAndAuthorization.Extensions.IClientMessageInspectors
{
    public class CookieMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (IsAuthenticate(Globals.Cookie))
            {
                new NotificationDialog("Please Log In!", "Service Response").ShowErrorMessageBox();

                return null;
            }

            request.Properties[HttpRequestMessageProperty.Name] = CreateRequestWithSetCookieAttribute();

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        { }

        #region PRIVATE Helper Methods

        private bool IsAuthenticate(string cookie) =>
            string.IsNullOrEmpty(cookie);

        private HttpRequestMessageProperty CreateRequestWithSetCookieAttribute()
        {
            HttpRequestMessageProperty request = new HttpRequestMessageProperty
            {
                Headers = { [HttpResponseHeader.SetCookie] = Globals.Cookie }
            };

            return request;
        }

        #endregion
    }
}