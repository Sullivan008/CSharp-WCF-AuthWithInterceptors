using ServerForAuthenticationAndAuthorization.Models.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Web.Security;

namespace ServerForAuthenticationAndAuthorization.Extensions.IDispatchMessageInspector
{
    public class IdentityMessageInspector : System.ServiceModel.Dispatcher.IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            FormsAuthenticationTicket ticket = null;
            List<Tuple<string, string>> setCookieTable = GetCookieTable(GetCookieInformation());

            string encryptedTicket = GetEncryptedAuthCookieTicket(setCookieTable);

            if (!string.IsNullOrEmpty(encryptedTicket))
            {
                ticket = GetDecryptTicket(encryptedTicket);
            }

            if (ticket != null)
            {
                GenericPrincipal threadCurrentPrincipal = new GenericPrincipal(new CustomIdentity
                {
                    IsAuthenticated = true,
                    Name = GetUserName(ticket)
                }, GetUserRoles(ticket).Split(','));

                Thread.CurrentPrincipal = threadCurrentPrincipal;
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {

        }

        #region USER Methods

        private string GetUserName(FormsAuthenticationTicket ticket) =>
            ticket.Name;

        private string GetUserRoles(FormsAuthenticationTicket ticket) =>
            ticket.UserData;

        #endregion

        #region COOKIE Operations

        private string GetCookieInformation()
        {
            return GetCookieInformationInRequestChannelHeaders("Set-Cookie") ??
                   GetCookieInformationInRequestChannelHeaders("Cookie") ?? string.Empty;
        }

        private List<Tuple<string, string>> GetCookieTable(string setCookieAttribute)
        {
            List<Tuple<string, string>> setCookieTable = new List<Tuple<string, string>>();

            foreach (string cookieItem in setCookieAttribute.Split(';'))
            {
                if (!cookieItem.Contains(','))
                {
                    AddTupleItemFromSplitArray(setCookieTable, (cookieItem.Replace(" ", string.Empty).Split('=')));
                }
                else if (cookieItem.Contains(',') && !cookieItem.Contains("expires"))
                {
                    foreach (string element in (cookieItem.Replace(" ", string.Empty)).Split(',').ToList())
                    {
                        AddTupleItemFromSplitArray(setCookieTable, (element.Replace(" ", string.Empty)).Split('='));
                    }
                }
                else if (cookieItem.Contains(',') && cookieItem.Contains("expires"))
                {
                    AddTupleItemFromSplitArray(setCookieTable, ((cookieItem.StartsWith(" ") ? cookieItem.Substring(1) : cookieItem)).Split('='));
                }
            }

            return setCookieTable;
        }

        #endregion

        #region CRYPTOGRAPHIC Operations

        private string GetEncryptedAuthCookieTicket(IEnumerable<Tuple<string, string>> setCookieTable)
        {
            Tuple<string, string> authCookieTicket =
                setCookieTable.FirstOrDefault(x => x.Item1 == FormsAuthentication.FormsCookieName);

            return authCookieTicket?.Item2;
        }

        private FormsAuthenticationTicket GetDecryptTicket(string encryptedTicket) =>
            FormsAuthentication.Decrypt(encryptedTicket);

        #endregion

        #region PRIVATE Helper Methods

        private string GetCookieInformationInRequestChannelHeaders(string headerKey) =>
            ((HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name])
            .Headers.Get(headerKey);

        private void AddTupleItemFromSplitArray(ICollection<Tuple<string, string>> setCookieTable, IReadOnlyList<string> splittedItem)
        {
            if (splittedItem.Count >= 2)
            {
                AddNewTupleItem(setCookieTable, splittedItem[0], splittedItem[1]);
            }
            else if (splittedItem.Count >= 1)
            {
                AddNewTupleItem(setCookieTable, splittedItem[0], null);
            }
        }

        private void AddNewTupleItem(ICollection<Tuple<string, string>> setCookieTable, string firstItem, string secondItem)
        {
            setCookieTable.Add(new Tuple<string, string>(firstItem, secondItem));
        }

        #endregion
    }
}