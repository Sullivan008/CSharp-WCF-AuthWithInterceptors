using System.Security.Principal;

namespace ServerForAuthenticationAndAuthorization.Models.Identities
{
    public class CustomIdentity : IIdentity
    {
        public string Name { get; set; }

        public bool IsAuthenticated { get; set; }

        public string AuthenticationType { get; set; }
    }
}