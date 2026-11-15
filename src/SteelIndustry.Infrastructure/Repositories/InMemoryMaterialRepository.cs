using System.Collections.Concurrent;
using SteelIndustry.Domain.Entities;
using SteelIndustry.Domain.Repositories;

namespace SteelIndustry.Infrastructure.Repositories;

public class InMemoryMaterialRepository : IMaterialRepository
{
    private static readonly ConcurrentDictionary<string, Material> _materials = new();

    public InMemoryMaterialRepository()
    {
        if (_materials.IsEmpty)
        {
            SeedMaterials();
        }
    }

    private static void SeedMaterials()
    {
        var sae1020 = new Material(
            id: "mat-1020-001",
            code: "STL-1020-001",
            steelGrade: "SAE 1020",
            standard: "ABNT NBR NM 87 / SAE J403",
            heatNumber: "CR-2026-A12",
            specificationsJson: """
            {
              "material": "Aço Carbono SAE 1020",
              "tipoProduto": "Barra Laminada a Quente",
              "dimensoes": {
                "diametroMm": 50.8,
                "comprimentoMm": 6000,
                "toleranciaDiametro": "ISO h11"
              },
              "composicaoQuimicaPercentual": {
                "carbono": 0.21,
                "manganes": 0.45,
                "silicio": 0.22,
                "fosforo": 0.018,
                "enxofre": 0.021,
                "cromo": 0.04,
                "niquel": 0.02
              },
              "propriedadesMecanicas": {
                "limiteEscoamentoMpa": 245,
                "limiteResistenciaTracaoMpa": 420,
                "alongamentoPercentual": 28.5,
                "durezaBrinell": 131
              },
              "ensaiosQualidade": {
                "ultrassom": "Conforme ASTM A388",
                "tamanhoGrauAustenitico": 7.5,
                "inclusoesNaoMetalicas": "Metodo JK Severidade Leve",
                "descarbonetacaoMm": 0.15
              },
              "certificacao": {
                "laboratorio": "Laboratorio Metalurgico Central",
                "emissao": "2026-11-01T08:30:00Z",
                "statusInspecao": "Aprovado"
              }
            }
            """
        );

        var sae1045 = new Material(
            id: "mat-1045-002",
            code: "STL-1045-002",
            steelGrade: "SAE 1045",
            standard: "SAE J403 / DIN C45",
            heatNumber: "CR-2026-B88",
            specificationsJson: """
            {
              "material": "Aço Médio Carbono SAE 1045",
              "tipoProduto": "Tarugo Forjado e Beneficiado",
              "dimensoes": {
                "secaoQuadradaMm": 120.0,
                "comprimentoMm": 4500
              },
              "composicaoQuimicaPercentual": {
                "carbono": 0.46,
                "manganes": 0.72,
                "silicio": 0.25,
                "fosforo": 0.015,
                "enxofre": 0.019
              },
              "propriedadesMecanicas": {
                "limiteEscoamentoMpa": 380,
                "limiteResistenciaTracaoMpa": 640,
                "alongamentoPercentual": 18.0,
                "durezaBrinell": 192
              },
              "tratamentoTermico": {
                "tipo": "Normalizacao a 860C",
                "resfriamento": "Ar Forçado"
              }
            }
            """
        );

        var inox304 = new Material(
            id: "mat-304-003",
            code: "STL-304-003",
            steelGrade: "Inox AISI 304",
            standard: "ASTM A240 / NBR 5601",
            heatNumber: "CR-2026-SS09",
            specificationsJson: """
            {
              "material": "Aço Inoxidável Austenítico AISI 304",
              "tipoProduto": "Chapa Laminada a Frio",
              "dimensoes": {
                "espessuraMm": 3.0,
                "larguraMm": 1500,
                "comprimentoMm": 3000
              },
              "composicaoQuimicaPercentual": {
                "carbono": 0.045,
                "cromo": 18.25,
                "niquel": 8.12,
                "manganes": 1.45,
                "silicio": 0.48,
                "fosforo": 0.028,
                "enxofre": 0.006
              },
              "propriedadesMecanicas": {
                "limiteEscoamentoMpa": 290,
                "limiteResistenciaTracaoMpa": 620,
                "alongamentoPercentual": 52.0,
                "durezaRockwellB": 82
              },
              "resistenciaCorrosao": {
                "ensaioNievoaSalinaHoras": 720,
                "resultado": "Sem Corrosão Pite Observada"
              }
            }
            """
        );

        var astmA36 = new Material(
            id: "mat-a36-004",
            code: "STL-A36-004",
            steelGrade: "ASTM A36",
            standard: "ASTM A36 / A36M",
            heatNumber: "CR-2026-STR44",
            specificationsJson: """
            {
              "material": "Aço Estrutural Laminado ASTM A36",
              "tipoProduto": "Perfil I Estrutural 150x18.5",
              "dimensoes": {
                "alturaMm": 152.4,
                "larguraAbaMm": 100.2,
                "espessuraAlmaMm": 6.1
              },
              "composicaoQuimicaPercentual": {
                "carbono": 0.24,
                "manganes": 0.95,
                "silicio": 0.20,
                "fosforo": 0.025,
                "enxofre": 0.030
              },
              "propriedadesMecanicas": {
                "limiteEscoamentoMpa": 275,
                "limiteResistenciaTracaoMpa": 460,
                "alongamentoPercentual": 24.0
              }
            }
            """
        );

        _materials.TryAdd(sae1020.Id, sae1020);
        _materials.TryAdd(sae1045.Id, sae1045);
        _materials.TryAdd(inox304.Id, inox304);
        _materials.TryAdd(astmA36.Id, astmA36);
    }

    public Task<Material?> GetByIdAsync(string id)
    {
        _materials.TryGetValue(id, out var material);
        return Task.FromResult(material);
    }

    public Task<Material?> GetByCodeAsync(string code)
    {
        var material = _materials.Values.FirstOrDefault(m => 
            string.Equals(m.Code, code, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(material);
    }

    public Task<Material[]> GetAllAsync()
    {
        return Task.FromResult(_materials.Values.ToArray());
    }

    public Task AddAsync(Material material)
    {
        _materials.TryAdd(material.Id, material);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Material material)
    {
        _materials[material.Id] = material;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        _materials.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
