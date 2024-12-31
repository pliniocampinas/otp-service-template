namespace otp_service_template.Services;

public class UserService: IUserService
{
  public UserService()
  {

  }

  public async Task<User> GetByEmail(string email)
  {
    await Task.Delay(100);
    return new User()
    {
      Email = email,
      Status = UserStatus.Active
    };
  }
}