using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace otp_service_template.Services;

public class OneTimePasswordService: IOneTimePasswordService
{
  public IUserService UserService { get; set; }
  public IEmailService EmailService { get; set; }
  public AppSettings AppSettings { get; set; }

  public OneTimePasswordService(
    IUserService userService, 
    IEmailService emailService,
    AppSettings appSettings)
  {
    UserService = userService?? throw new ArgumentNullException(nameof(UserService));
    EmailService = emailService?? throw new ArgumentNullException(nameof(EmailService));
    AppSettings = appSettings?? throw new ArgumentNullException(nameof(appSettings));
  }

  public async Task RequestValidation(string email)
  {
    var user = await UserService.GetByEmail(email);

    if (user is null || user.Status == UserStatus.None)
      return;

    await EmailService.Send(new ValidationEmail()
    {
      Email = user.Email,
      FullName = user.FullName,
      Password = GeneratePassword()
    });

    await Task.Delay(100);
  }

  public async Task<OneTimePasswordConfirmResult> ConfirmValidation(string email, string password)
  {
    await Task.Delay(100);

    return new OneTimePasswordConfirmResult()
    {
      Status = ConfirmationStatus.Authorized,
      Token = GenerateToken(email)
    };
  }

  public async Task<bool> VerifyToken(string token)
  {
    await Task.Delay(100);
    var tokenHandler = new JwtSecurityTokenHandler();
    var result = await tokenHandler.ValidateTokenAsync(token, GetValidationParameters());

    if(result.IsValid == false)
    {
      Console.WriteLine("Invalid token");
      return false;
    }

    return result.IsValid;
  }

  private string GeneratePassword()
  {
    var password = new Random()
      .Next(0, 10_000)
      .ToString();

    return password.PadLeft(4, '0');
  }

  private string GenerateToken(string email)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(AppSettings.AuthSecret);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(
      [
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, "User")
      ]),
      Expires = DateTime.UtcNow.AddMinutes(10),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  private TokenValidationParameters GetValidationParameters()
  {
    return new TokenValidationParameters()
    {
      ValidateAudience = false,
      ValidateIssuer = false,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.AuthSecret))
    };
  }
}