namespace otp_service_template.Services;

public class EmailService: IEmailService
{
  public EmailService()
  {

  }

  public async Task Send(ValidationEmail email)
  {
    await Task.Delay(100);
    Console.WriteLine($"EmailService.Send: {email.Email}-{email.Password}");
  }
}