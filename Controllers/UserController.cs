using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace otp_service_template.Services;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("")]
    [Authorize]
    public async Task<IActionResult> Get(
      [FromServices] IUserService userService
    )
    {
        var email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        Console.WriteLine("email" + email);

        var user = await userService.GetByEmail(email);

        if (user is null)
        {
            return NotFound(ApiError.UserNotFound.AsApiResponse());
        }

        return Ok(user);
    }

    [HttpPost("submit")]
    [Authorize]
    public async Task<IActionResult> Submit(
      [FromBody] SubmitUserRequest request,
      [FromServices] IUserService userService,
      [FromServices] IOneTimePasswordService otpService
    )
    {
        var email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        if (string.IsNullOrEmpty(email))
            return BadRequest(ApiError.InvalidEmail.AsApiResponse());

        var user = await userService.GetByEmail(email);

        if (user is not null)
        {
            return NoContent();
        }

        await userService.Save(new User()
        {
            Email = email,
            FullName = request.FullName
        });

        return NoContent();
    }
}