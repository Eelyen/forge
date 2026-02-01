namespace Forge.Api.Domain.Abstractions;

public interface IStronglyTypedId<TSelf> where TSelf : struct
{
    Guid Value { get; }
    static abstract TSelf New();
    static abstract TSelf FromGuid(Guid value);
}
