namespace Forge.Api.Domain;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;    
    public DateTimeOffset CreatedUtc { get; set; }
}
