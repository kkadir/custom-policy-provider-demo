using System;

namespace CustomPolicyProvidersDemo.Authorization
{
    public interface IIdentifiable
    {
        Guid Identifier { get; }
    }
}
