using Microsoft.AspNetCore.Mvc;
using SteelIndustry.Application.DTOs;
using SteelIndustry.Application.Interfaces;

namespace SteelIndustry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserAppService _userAppService;

    public UsersController(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    /// <summary>
    /// Retorna array estruturado de usuários para otimização de alocação de memória.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<UserDto[]>> GetAll()
    {
        var users = await _userAppService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var user = await _userAppService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = $"Usuário com ID '{id}' não encontrado." });
        }
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
    {
        var created = await _userAppService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(string id, [FromBody] UpdateUserDto dto)
    {
        var updated = await _userAppService.UpdateAsync(id, dto);
        if (updated == null)
        {
            return NotFound(new { message = $"Usuário com ID '{id}' não encontrado." });
        }
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _userAppService.DeleteAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Usuário com ID '{id}' não encontrado." });
        }
        return NoContent();
    }
}
