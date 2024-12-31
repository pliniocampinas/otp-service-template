namespace otp_service_template.Services;

public class OneTimePasswordService: IOneTimePasswordService
{
  public OneTimePasswordService()
  {

  }

  public async Task RequestValidation(string email)
  {
    await Task.Delay(100);
  }

  public async Task<OneTimePasswordConfirmResult> ConfirmValidation(string email, string password)
  {
    await Task.Delay(100);

    return new OneTimePasswordConfirmResult()
    {
      Status = ConfirmationStatus.Authorized,
      Token = "to be determined"
    };
  }
}