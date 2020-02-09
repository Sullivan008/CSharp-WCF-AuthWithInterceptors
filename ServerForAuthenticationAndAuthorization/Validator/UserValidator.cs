using ServerForAuthenticationAndAuthorization.StaticDatas;
using System;
using System.Collections.Generic;
using System.Web.ApplicationServices;

namespace ServerForAuthenticationAndAuthorization.Validator
{
    public class UserValidator
    {
        public bool IsValidUser(AuthenticatingEventArgs credentials, ref string roles)
        {
            return UserIsValidUser(credentials, ref roles, UserDatas.GetFirstUser) ||
                   UserIsValidUser(credentials, ref roles, UserDatas.GetSecondUser) ||
                   UserIsValidUser(credentials, ref roles, UserDatas.GetThirdUser);
        }

        #region PRIVATE Helper Methods

        private bool UserIsValidUser(AuthenticatingEventArgs credentials, ref string roles, KeyValuePair<string, string> staticUserData)
        {
            if (credentials.UserName == staticUserData.Key && credentials.Password == staticUserData.Value)
            {
                roles = SetRoles(staticUserData.Key);

                return true;
            }

            return false;
        }

        private string SetRoles(string userName)
        {
            switch (userName)
            {
                case "user1":
                    return UserRoles.FirstUserRole;
                case "user2":
                    return UserRoles.SecondUserRole;
                case "user3":
                    return UserRoles.ThirdUserRole;
                default:
                    throw new ArgumentOutOfRangeException($@"Switching Type is not exists this method: {nameof(SetRoles)}!");
            }
        }

        #endregion
    }
}