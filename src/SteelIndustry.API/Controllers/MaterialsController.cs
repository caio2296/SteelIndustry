using Microsoft.AspNetCore.Mvc;
using SteelIndustry.Application.DTOs;
using SteelIndustry.Application.Interfaces;
using SteelIndustry.Domain.Enums;

namespace SteelIndustry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialAppService _materialAppService;

    public MaterialsController(IMaterialAppService materialAppService)
    {
        _materialAppService = materialAppService;
    }

    /// <summary>
    /// Retorna array estruturado de especificações e materiais de aço para máxima performance de serialização.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<MaterialDto[]>> GetAll()
    {
        var materials = await _materialAppService.GetAllAsync();
        return Ok(materials);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MaterialDto>> GetById(string id)
    {
        var material = await _materialAppService.GetByIdAsync(id);
        if (material == null)
        {
            return NotFound(new { message = $"Material com ID '{id}' não encontrado." });
        }
        return Ok(material);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<MaterialDto>> GetByCode(string code)
    {
        var material = await _materialAppService.GetByCodeAsync(code);
        if (material == null)
        {
            return NotFound(new { message = $"Material com código '{code}' não encontrado." });
        }
        return Ok(material);
    }

    [HttpPost]
    public async Task<ActionResult<MaterialDto>> Create([FromBody] CreateMaterialDto dto)
    {
        var created = await _materialAppService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MaterialDto>> Update(string id, [FromBody] UpdateMaterialDto dto)
    {
        var updated = await _materialAppService.UpdateAsync(id, dto);
        if (updated == null)
        {
            return NotFound(new { message = $"Material com ID '{id}' não encontrado." });
        }
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _materialAppService.DeleteAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Material com ID '{id}' não encontrado." });
        }
        return NoContent();
    }

    /// <summary>
    /// Compactador Especializado de JSON de Materiais Siderúrgicos.
    /// Reduz drasticamente o tamanho do payload utilizando Minificação sem perdas ou Compressão Brotli/GZip em Base64.
    /// </summary>
    [HttpPost("compact")]
    public async Task<ActionResult<CompactedMaterialResponseDto>> Compact([FromBody] CompactMaterialRequestDto request)
    {
        try
        {
            var response = await _materialAppService.CompactMaterialAsync(request);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Descompacta o payload de material previamente compactado.
    /// </summary>
    [HttpPost("decompact")]
    public async Task<ActionResult<string>> Decompact([FromBody] DecompactRequestDto request)
    {
        try
        {
            var json = await _materialAppService.DecompactMaterialAsync(request.CompactedData, request.Strategy);
            return Ok(new { decompactedJson = json });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Falha ao descompactar payload.", detail = ex.Message });
        }
    }
}

public record DecompactRequestDto(
    string CompactedData,
    CompressionStrategy Strategy
);
