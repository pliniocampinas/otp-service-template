using Microsoft.Data.Sqlite;

namespace otp_service_template.Services;

public class UserService: IUserService
{
  public AppSettings AppSettings { get; set; }
  public UserService(AppSettings appSettings)
  {
    AppSettings = appSettings?? throw new ArgumentNullException(nameof(appSettings));
  }

  public async Task<User> GetByEmail(string email)
  {
    await Task.Delay(100);

    var user = new User();
    var sql = "SELECT id, email, full_name, status FROM otp_users WHERE email = @email";

    using (var connection = new SqliteConnection(AppSettings.SqLiteConnectionString))
    {
      connection.Open();
      var command = connection.CreateCommand();
      command.CommandText = sql;
      command.Parameters.AddWithValue("@email", email);

      using (var reader = command.ExecuteReader())
      {
        if (reader.Read())
        {
          MapValues(user, reader);
        }
      }
    }

    return user;
  }

  public async Task<User> Save(User user)
  {
    await Task.Delay(100);

    var sql = "INSERT INTO otp_users (id, email, full_name, status) "
      + " VALUES (@id, @email, @full_name, @status)";

    using (var connection = new SqliteConnection(AppSettings.SqLiteConnectionString))
    {
      connection.Open();
      var command = connection.CreateCommand();
      command.CommandText = sql;
      command.Parameters.AddWithValue("@id", Guid.NewGuid());
      command.Parameters.AddWithValue("@email", user.Email);
      command.Parameters.AddWithValue("@full_name", user.FullName);
      command.Parameters.AddWithValue("@status", user.Status);

      command.ExecuteNonQuery();
    }

    return user;
  }

  private void MapValues(User model, SqliteDataReader? reader)
  {
    if(reader == null)
    {
      return;
    }

    var parsedIdSucessfully =  Guid.TryParse(reader.GetString(0), out Guid id);
    if (parsedIdSucessfully)
      model.Id = id;
    model.Email = reader.GetString(1);
    model.FullName = reader.GetString(2);
    var parsedStatusSucessfully =  Enum.TryParse(reader.GetString(3), out UserStatus userStatus);
    if (parsedStatusSucessfully)
      model.Status = userStatus;
  }
}