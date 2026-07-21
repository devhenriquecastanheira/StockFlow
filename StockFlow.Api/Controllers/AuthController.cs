using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Auth;
using StockFlow.Application.Email;

namespace StockFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    private readonly IEmailSender _emailSender;

    public AuthController(IAuthService service, IEmailSender emailSender)
    {
        _service = service;
        _emailSender = emailSender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _service.RegisterAsync(request);

        if (!result)
        {
            return BadRequest("Não foi possível cadastrar o usuário.");
        }

        await _emailSender.SendEmailAsync(
            request.Email,
            "Bem-vindo ao StockFlow!",
            $"<h1>Olá, {request.Name}!</h1><p>Seu cadastro foi realizado com sucesso.</p>");

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
