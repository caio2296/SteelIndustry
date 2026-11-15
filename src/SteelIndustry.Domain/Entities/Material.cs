using System.Text.Json;

namespace SteelIndustry.Domain.Entities;

public class Material
{
    public string Id { get; private set; }
    public string Code { get; private set; }
    public string SteelGrade { get; private set; }
    public string Standard { get; private set; }
    public string HeatNumber { get; private set; }
    public string SpecificationsJson { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Material(
        string id, 
        string code, 
        string steelGrade, 
        string standard, 
        string heatNumber, 
        string specificationsJson)
    {
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        SteelGrade = steelGrade ?? throw new ArgumentNullException(nameof(steelGrade));
        Standard = standard ?? throw new ArgumentNullException(nameof(standard));
        HeatNumber = heatNumber ?? throw new ArgumentNullException(nameof(heatNumber));
        SpecificationsJson = string.IsNullOrWhiteSpace(specificationsJson) ? "{}" : specificationsJson;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string steelGrade, string standard, string heatNumber, string specificationsJson)
    {
        SteelGrade = steelGrade ?? throw new ArgumentNullException(nameof(steelGrade));
        Standard = standard ?? throw new ArgumentNullException(nameof(standard));
        HeatNumber = heatNumber ?? throw new ArgumentNullException(nameof(heatNumber));
        SpecificationsJson = string.IsNullOrWhiteSpace(specificationsJson) ? "{}" : specificationsJson;
        UpdatedAt = DateTime.UtcNow;
    }

    public JsonElement GetSpecificationsAsJsonElement()
    {
        using var doc = JsonDocument.Parse(SpecificationsJson);
        return doc.RootElement.Clone();
    }
}
