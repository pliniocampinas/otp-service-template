using Microsoft.Data.Sqlite;

namespace otp_service_template.Services;

public class DatabaseService: IDatabaseService
{
  public AppSettings AppSettings { get; set; }
  public DatabaseService(AppSettings appSettings)
  {
    AppSettings = appSettings?? throw new ArgumentNullException(nameof(appSettings));
  }

  public async Task CreateTables()
  {
    await Task.Delay(100);

    var sql = @"
        CREATE TABLE IF NOT EXISTS otp_users (
          id TEXT,
          email TEXT,
          full_name TEXT,
          status TEXT,
          PRIMARY KEY (id)
      );";

    using (var connection = new SqliteConnection(AppSettings.SqLiteConnectionString))
    {
      connection.Open();
      var command = connection.CreateCommand();
      command.CommandText = sql;
      command.ExecuteNonQuery();
    }
  }
}