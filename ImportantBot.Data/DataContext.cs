using Microsoft.Data.SqlClient;

namespace ImportantBot.Data;

public class DataContext
{
    private readonly string _sqlConnection;

    public DataContext(string sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task InsertData(MessageModel message)
    {
        var connectionString = new SqlConnectionStringBuilder(_sqlConnection);

        var sqlQuery = "INSERT INTO [dbo].[ChatMessages] ([UsedId], [UserName], [MessageLink], [MessageText])\r\n  VALUES (@userId, @userName, @messageLink, @messageText) ";

        using (var connection = new SqlConnection(connectionString.ConnectionString))
        {
            await connection.OpenAsync();
            var sqlCommand = new SqlCommand(sqlQuery, connection);
            var parameterId = new SqlParameter("@userId", message.UserId);
            var parameterUserName = new SqlParameter("@userName", message.UserName);
            var parameterMessageLink = new SqlParameter("@messageLink", message.Link);
            var parameterMessage = new SqlParameter("@messageText", message.Text);

            sqlCommand.Parameters.AddRange(new[] { parameterId, parameterUserName, parameterMessageLink, parameterMessage });

            var numberOfNotes = await sqlCommand.ExecuteNonQueryAsync();
        }
    }
}