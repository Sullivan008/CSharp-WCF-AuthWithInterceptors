using System.ServiceModel;

namespace ServerForAuthenticationAndAuthorization
{
    [ServiceContract]
    public interface IExecuteOperationsService
    {
        [OperationContract]
        int MulOperation(int multiplier, int multiplicand);

        [OperationContract]
        string ReadOperation();

        [OperationContract]
        string WriteOperation();
    }
}
