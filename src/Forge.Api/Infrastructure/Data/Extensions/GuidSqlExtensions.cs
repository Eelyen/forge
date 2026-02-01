namespace System;

public static class GuidSqlExtensions
{
    /// <summary>
    /// Shuffles a standard UUID v7 into a SQL Server-optimized UniqueIdentifier.
    /// Use this for Dapper or Raw SQL parameters.
    /// </summary>
    public static Guid ToSqlSequential(this Guid guid)
    {
        if (guid == Guid.Empty) return guid;

        // SQL Server sorts uniqueidentifiers by bytes [10-15], then [8-9], then [6-7].
        // other database providers may and do use different sort orders.
        // to make UUID v7 sequential in SQL Server, we put the v7 timestamp (first 6 bytes) into the 10-15 slot.
        Span<byte> bytes = stackalloc byte[16];
        guid.TryWriteBytes(bytes, bigEndian: true, out _);

        // Standard v7: [T1, T2, T3, T4, T5, T6, ..random..]
        // SQL Target: [..random.., T1, T2, T3, T4, T5, T6]
        Span<byte> sqlBytes = stackalloc byte[16];
        bytes[6..16].CopyTo(sqlBytes[0..10]); // Move random bits to the start
        bytes[0..6].CopyTo(sqlBytes[10..16]); // Move timestamp to the end

        return new Guid(sqlBytes, bigEndian: true);
    }

    /// <summary>
    /// Restores a shuffled SQL Server UniqueIdentifier to a standard RFC UUID v7.
    /// Use this when reading raw results from Dapper.
    /// </summary>
    public static Guid FromSqlSequential(this Guid sqlGuid)
    {
        if (sqlGuid == Guid.Empty) return sqlGuid;

        // Reverse the shuffle operation
        Span<byte> sqlBytes = stackalloc byte[16];
        sqlGuid.TryWriteBytes(sqlBytes, bigEndian: true, out _);

        // SQL Target: [..random.., T1, T2, T3, T4, T5, T6]
        // Standard v7: [T1, T2, T3, T4, T5, T6, ..random..]
        Span<byte> rfcBytes = stackalloc byte[16];
        sqlBytes[10..16].CopyTo(rfcBytes[0..6]); // Move timestamp to the start
        sqlBytes[0..10].CopyTo(rfcBytes[6..16]); // Move random bits to the end

        return new Guid(rfcBytes, bigEndian: true);
    }
}
