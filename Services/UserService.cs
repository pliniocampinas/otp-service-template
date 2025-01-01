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

  public async Task<User> Save(User user)
  {
    await Task.Delay(100);
    return user;
  }
}