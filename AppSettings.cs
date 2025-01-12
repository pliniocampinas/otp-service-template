namespace otp_service_template;

public class AppSettings
{
  public AppSettings(IConfiguration configuration)
  {
    AuthSecret = configuration.GetSection("AuthSecret").Get<string>()?? "";
    SqLiteConnectionString = configuration.GetSection("SqliteDbConnectionString").Get<string>()?? "";
  }
  
  public string AuthSecret { get; set; } = string.Empty;
  public string SqLiteConnectionString { get; set; } = string.Empty;
}