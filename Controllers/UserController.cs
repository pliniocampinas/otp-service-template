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

    return Ok(user);
  }

  [HttpPost("register")]
  public async Task<IActionResult> Start(
    [FromBody] RegisterUserRequest request,
    [FromServices] IUserService userService,
    [FromServices] IOneTimePasswordService otpService
  )
  {
    if (string.IsNullOrEmpty(request?.Email))
      return BadRequest(ApiError.InvalidEmail.AsApiResponse());

    var user = await userService.GetByEmail(request.Email);

    if (user is not null)
    {
      return NoContent();
    }

    await userService.Save(new User()
    {
      Email = request.Email,
      FullName = request.FullName,
      Status = UserStatus.Created
    });

    await otpService.RequestValidation(request.Email);

    return NoContent();
  }

  [HttpPost("confirm-register")]
  public async Task<IActionResult> Confirm(
    [FromBody] ConfirmRegisterRequest request,
    [FromServices] IUserService userService,
    [FromServices] IOneTimePasswordService otpService
  )
  {
    if (string.IsNullOrEmpty(request?.Email))
      return BadRequest(ApiError.InvalidEmail.AsApiResponse());

    var user = await userService.GetByEmail(request.Email);

    if (user is null)
    {
      return BadRequest();
    }

    var result = await otpService.ConfirmValidation(request.Email, request.Password);

    if (result.Status != ConfirmationStatus.Authorized)
      return Unauthorized();

    await userService.Activate(user.Id.Value);

    return Ok();
  }
}