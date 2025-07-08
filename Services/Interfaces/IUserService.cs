namespace otp_service_template.Services;

public interface IUserService
{
    Task<User?> GetByEmail(string email);
    Task<Guid> Create(string email);
    Task<User> Save(User user);
}