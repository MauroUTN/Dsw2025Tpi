using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ApplicationException = Dsw2025Tpi.Application.Exceptions.ApplicationException;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/auth")]

public class AuthenticateController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IRepository _repository;

    public AuthenticateController(
         UserManager<IdentityUser> userManager,
         SignInManager<IdentityUser> signInManager,
         JwtTokenService jwtTokenService,
         IRepository repository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _repository = repository;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return Unauthorized("Nombre o contraseña incorrectos");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Nombre o contraseña incorrectos");
        }
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? throw new ApplicationException("El usuario no tiene un rol asignado");

        // CAMBIO: Pasamos user.Id como primer parámetro
        var token = _jwtTokenService.GenerateToken(user.Id, request.Username, role);

        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new IdentityUser
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber 
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        var customerId = Guid.Parse(user.Id);

        var customerName = string.IsNullOrWhiteSpace(model.Name) ? model.Username : model.Name;
        var customerPhone = string.IsNullOrWhiteSpace(model.PhoneNumber) ? "Sin registrar" : model.PhoneNumber;

        var newCustomer = new Customer(customerName, model.Email, customerPhone)
        {
            Id = customerId 
        };

        await _repository.Add(newCustomer);

        return Ok("Usuario y Cliente registrados exitosamente.");
    }
}