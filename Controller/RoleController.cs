using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagementAPI.Model;
using UserManagementAPI.Dto.Person;
[Route("api/roles")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly UserManager<Person> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(UserManager<Person> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("assign")]
    /* [Authorize(Roles = "Admin")] */ // sadece adminler rol atayabilir fakat startup'da kimse admin degil
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null) return NotFound();

        if (!await _roleManager.RoleExistsAsync(dto.Role))
            await _roleManager.CreateAsync(new IdentityRole(dto.Role));

        await _userManager.AddToRoleAsync(user, dto.Role);
        return Ok("Rol atandı.");
    }
}