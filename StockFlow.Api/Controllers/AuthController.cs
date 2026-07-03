using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Auth;

namespace StockFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _service.RegisterAsync(request);

        if (!result)
        {
            return BadRequest("Não foi possível cadastrar o usuário.");
        }

        return Ok("Usuário cadastrado com sucesso.");
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _service.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized("Email ou senha inválidos.");
        }

        return Ok(result);
    }
}
