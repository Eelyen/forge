namespace Forge.Api.Domain.Enums;

public enum UnitKind : byte
{
    Item = 0, // Discrete items (e.g., Satisfactory: items/min)
    Volume = 1, // Volume-based items (e.g., Satisfactory fluids: m³/min)
}
