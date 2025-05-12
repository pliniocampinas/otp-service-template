namespace otp_service_template.Services;

public interface ITokenService
{
  Task<bool> VerifyToken(string token);
  string GenerateToken(string email);
}