using Microsoft.AspNetCore.Mvc;
using SteelIndustry.Application.DTOs;
using SteelIndustry.Application.Interfaces;

namespace SteelIndustry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthAppService _authAppService;

    public AuthController(IAuthAppService authAppService)
    {
        _authAppService = authAppService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Usuário e senha são obrigatórios." });
        }

        var result = await _authAppService.LoginAsync(request);
        if (result == null)
        {
            return Unauthorized(new { message = "Credenciais inválidas ou usuário inativo." });
        }

        return Ok(result);
    }
}
