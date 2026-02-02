namespace Forge.Api.Domain.Abstractions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3246:Generic type parameters should be co/contravariant when possible", Justification = "TSelf represents the implementing type, not a variant type parameter")]
public interface IStronglyTypedId<TSelf> where TSelf : struct
{
    Guid Value { get; }
    static abstract TSelf Create();
    static abstract TSelf FromGuid(Guid value);
}
