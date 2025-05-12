using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace otp_service_template.Services;

public class TokenService: ITokenService
{
  public AppSettings AppSettings { get; set; }

  public TokenService(AppSettings appSettings)
  {
    AppSettings = appSettings?? throw new ArgumentNullException(nameof(appSettings));
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

  public string GenerateToken(string email)
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