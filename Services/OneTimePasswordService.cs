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
  public CacheService CacheService { get; set; }
  public ITokenService TokenService { get; set; }

  public OneTimePasswordService(
    IUserService userService, 
    IEmailService emailService,
    AppSettings appSettings,
    ITokenService tokenService,
    CacheService cacheService)
  {
    UserService = userService?? throw new ArgumentNullException(nameof(UserService));
    EmailService = emailService?? throw new ArgumentNullException(nameof(EmailService));
    AppSettings = appSettings?? throw new ArgumentNullException(nameof(appSettings));
    CacheService = cacheService?? throw new ArgumentNullException(nameof(cacheService));
    TokenService = tokenService?? throw new ArgumentNullException(nameof(tokenService));
  }

  public async Task RequestValidation(string email)
  {
    var password = GeneratePassword();

    await EmailService.Send(new ValidationEmail()
    {
      Email = email,
      Password = password
    });

    CacheService.Save(email, password);

    await Task.Delay(100);
  }

  public async Task<OneTimePasswordConfirmResult> ConfirmValidation(string email, string password)
  {
    await Task.Delay(100);

    var cachedPassword = CacheService.Get(email);

    if (string.IsNullOrWhiteSpace(cachedPassword) || password.Trim() != cachedPassword)
      return new OneTimePasswordConfirmResult()
      {
        Status = ConfirmationStatus.Denied,
      };

    return new OneTimePasswordConfirmResult()
    {
      Status = ConfirmationStatus.Authorized,
      Token = TokenService.GenerateToken(email)
    };
  }

  private string GeneratePassword()
  {
    var password = new Random()
      .Next(0, 10_000)
      .ToString();

    return password.PadLeft(4, '0');
  }
}