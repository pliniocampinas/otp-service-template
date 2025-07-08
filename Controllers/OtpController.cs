using Microsoft.AspNetCore.Mvc;

namespace otp_service_template.Services;

[ApiController]
[Route("[controller]")]
public class OtpController : ControllerBase
{
    [HttpPost("start")]
    public async Task<IActionResult> Start([FromServices] IOneTimePasswordService otpService, [FromBody] StartValidationRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
            return BadRequest("Email required");

        await otpService.RequestValidation(request.Email);

        return Ok("Email sent");
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(
      [FromServices] IOneTimePasswordService otpService,
      [FromBody] ConfirmValidationRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
            return BadRequest("Email required");

        if (string.IsNullOrEmpty(request.Password))
            return BadRequest("Password required");

        var result = await otpService.ConfirmValidation(request.Email, request.Password);

        if (result.Status != ConfirmationStatus.Authorized)
            return Unauthorized();

        return Ok(new
        {
            result.Token
        });
    }
}