using Microsoft.Data.Sqlite;

namespace otp_service_template.Services;

public class UserService: IUserService
{
  public AppSettings AppSettings { get; set; }
  public UserService(AppSettings appSettings)
  {
    AppSettings = appSettings?? throw new ArgumentNullException(nameof(appSettings));
  }

  public async Task<User?> GetByEmail(string email)
  {
    await Task.Delay(100);

    User? user = null;
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
          user = MapValues(reader);
        }
      }
    }

    Console.WriteLine("Found user " + user?.Email);

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

    Console.WriteLine("Created user " + user.Email);

    return user;
  }

  public async Task Activate(Guid userId)
  {
    await Task.Delay(100);

    var sql = "UPDATE otp_users SET status = @status "
      + " WHERE Id = @id ";

    using (var connection = new SqliteConnection(AppSettings.SqLiteConnectionString))
    {
      connection.Open();
      var command = connection.CreateCommand();
      command.CommandText = sql;
      command.Parameters.AddWithValue("@id", Guid.NewGuid());
      command.Parameters.AddWithValue("@status", UserStatus.Active);

      command.ExecuteNonQuery();
    }
  }

  private User? MapValues(SqliteDataReader? reader)
  {
    if(reader == null)
    {
      return null;
    }

    var model = new User();

    var parsedIdSucessfully =  Guid.TryParse(reader.GetString(0), out Guid id);
    if (parsedIdSucessfully)
      model.Id = id;
    model.Email = reader.GetString(1);
    model.FullName = reader.GetString(2);
    var parsedStatusSucessfully =  Enum.TryParse(reader.GetString(3), out UserStatus userStatus);
    if (parsedStatusSucessfully)
      model.Status = userStatus;

    return model;
  }
}