using System.Collections.Generic;
using System.Linq;

namespace ServerForAuthenticationAndAuthorization.StaticDatas
{
    public class UserDatas
    {
        private static string[] UserNames { get; } = { "user1", "user2", "user3" };

        private static Dictionary<string, string> Users { get; } = new Dictionary<string, string>()
        {
            {UserNames[0], "pass1"},
            {UserNames[1], "pass2"},
            {UserNames[2], "pass3"}
        };

        public static KeyValuePair<string, string> GetFirstUser =>
            Users.First(x => x.Key == UserNames[0]);

        public static KeyValuePair<string, string> GetSecondUser =>
            Users.First(x => x.Key == UserNames[1]);

        public static KeyValuePair<string, string> GetThirdUser =>
            Users.First(x => x.Key == UserNames[2]);
    }
}