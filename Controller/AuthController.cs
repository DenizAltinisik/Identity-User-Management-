using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Data;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagementAPI.Model;
using Microsoft.AspNetCore.Identity;

namespace UserManagementAPI.Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<Person> _userManager;

        public AuthController(UserManager<Person> userManager)
        {
 
            _userManager = userManager;
        }

        [HttpGet("roles-in-token")]
        public IActionResult GetRolesInToken()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return BadRequest("No JWT token found in Authorization header.");

            var tokenString = authHeader.Substring("Bearer ".Length);
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken;
            try
            {
                jwtToken = handler.ReadJwtToken(tokenString);
            }
            catch
            {
                return BadRequest("Invalid JWT token.");
            }

            var roles = jwtToken.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
                .Select(c => c.Value)
                .ToList();

            return Ok(new { roles });
        }

[HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Dto.Person.PersonLoginDTO loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return BadRequest("Invalid email or password.");
        }
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            return BadRequest("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("FirstName", user.FirstName ?? ""),
            new Claim("LastName", user.LastName ?? "")
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASuperSecretKey1234567890123456"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "http://localhost:5074",
            audience: "http://localhost:5074", 
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = tokenString });
    }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Dto.Person.PersonRegisterDTO registerDto)
        {
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            {
                return BadRequest("Email already exists.");
            }

            if (string.IsNullOrWhiteSpace(registerDto.Password) || registerDto.Password.Length < 6)
                return BadRequest("Password must be at least 6 characters long.");
            if (registerDto.Password.Any(char.IsWhiteSpace))
                return BadRequest("Password cannot contain whitespace.");

            var newUser = new Model.Person
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                RegisterDate = DateOnly.FromDateTime(DateTime.Now),
                IsActive = true
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()?.Description ?? "User creation failed.");
            }

            return CreatedAtAction(nameof(Register), new { id = newUser.Id }, newUser);
        }
    }
}