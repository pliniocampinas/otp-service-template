namespace otp_service_template.Services;

public class OneTimePasswordService: IOneTimePasswordService
{

  public IUserService UserService { get; set; }
  public IEmailService EmailService { get; set; }

  public OneTimePasswordService(IUserService userService, IEmailService emailService)
  {
    UserService = userService?? throw new ArgumentNullException(nameof(UserService));
    EmailService = emailService?? throw new ArgumentNullException(nameof(EmailService));
  }

  public async Task RequestValidation(string email)
  {
    var user = await UserService.GetByEmail(email);

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
      Token = GenerateToken()
    };
  }

  private string GeneratePassword()
  {
    var password = new Random()
      .Next(0, 10_000)
      .ToString();

    return password.PadLeft(4, '0');
  }

  private string GenerateToken()
  {
    return "to be determined";
  }
}