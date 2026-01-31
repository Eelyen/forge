namespace Forge.Api.Domain.Abstractions;

public interface IAuditable
{
    DateTimeOffset CreatedUtc { get; }
    DateTimeOffset UpdatedUtc { get; }

    void SetCreated(DateTimeOffset nowUtc);
    void Touch(DateTimeOffset nowUtc);
}
