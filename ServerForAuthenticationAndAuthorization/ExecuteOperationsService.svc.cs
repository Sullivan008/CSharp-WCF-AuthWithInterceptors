using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace ServerForAuthenticationAndAuthorization
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ExecuteOperationsService : IExecuteOperationsService
    {
        [PrincipalPermission(SecurityAction.Demand, Name = "user1")]
        public int MulOperation(int multiplier, int multiplicand) => 
            multiplier * multiplicand;

        [PrincipalPermission(SecurityAction.Demand, Role = "Read")]
        public string ReadOperation() => 
            "You have received the message successfully! You have access to the service with the following permission: Read";

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public string WriteOperation() => 
            "You have received the message successfully! You have access to the service with the following permission: Write";
    }
}