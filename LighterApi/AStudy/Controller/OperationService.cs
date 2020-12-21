using System;

namespace LighterApi
{
    public class OperationService : IOperationTransient, IOperationScoped, IOperationSingleton
    {
        public OperationService()
        {
            OperationId = Guid.NewGuid().ToString()[^8..];
        }
        public string OperationId { get; }
    }
}
