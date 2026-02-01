using Forge.Api.Domain.Abstractions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Forge.Api.Infrastructure.Data.ValueConverters;

public class SqlSequentialIdConverter<TId> : ValueConverter<TId, Guid>
    where TId : struct, IStronglyTypedId<TId>
{
    public SqlSequentialIdConverter() : base(
            id => ToProviderGuid(id),
            guid => FromProviderGuid(guid)
    ) { }

    private static Guid ToProviderGuid(TId id) => id.Value.ToSqlSequential();
    private static TId FromProviderGuid(Guid guid) => TId.FromGuid(guid.FromSqlSequential());
}
