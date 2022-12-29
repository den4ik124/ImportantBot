using Microsoft.Data.SqlClient;
using System.Text;

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

        using var connection = new SqlConnection(connectionString.ConnectionString);
        var sqlCommand = GetInsertValuesCommand(connection, message, "ChatMessages");
        await connection.OpenAsync();
        var numberOfNotes = await sqlCommand.ExecuteNonQueryAsync();
    }

    public async Task<List<MessageModel>> GetMessages(long chatId)
    {
        var messages = new List<MessageModel>();
        var connectionString = new SqlConnectionStringBuilder(_sqlConnection);

        using var connection = new SqlConnection(connectionString.ConnectionString);

        var dateTimeFrom = DateTime.Now.AddDays(-1);
        var sqlCommandText = $"SELECT * FROM [dbo].[ChatMessages] WHERE [DateTime] >= @dateTimeFrom AND [ChatId] = @chatId";
        var parameterDateTime = new SqlParameter("@dateTimeFrom", dateTimeFrom);
        var parameterChatId = new SqlParameter("@chatId", chatId);
        var sqlCommand = new SqlCommand(sqlCommandText, connection);
        sqlCommand.Parameters.AddRange(new[]
                                        {
                                            parameterDateTime,
                                            parameterChatId
                                        });

        try
        {
            await connection.OpenAsync();
            var reader = sqlCommand.ExecuteReader();

            while (await reader.ReadAsync())
            {
                var properties = typeof(MessageModel).GetProperties();
                var message = new MessageModel();
                foreach (var property in properties)
                {
                    property.SetValue(message, reader[property.Name]);
                }

                messages.Add(message);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return messages;
    }

    public async Task ClearTable()
    {
        var connectionString = new SqlConnectionStringBuilder(_sqlConnection);

        var sqlQuery = "TRUNCATE TABLE [dbo].[ChatMessages]";

        using var connection = new SqlConnection(connectionString.ConnectionString);
        await connection.OpenAsync();
        var sqlCommand = new SqlCommand(sqlQuery, connection);
        var numberOfNotes = await sqlCommand.ExecuteNonQueryAsync();
    }

    private SqlCommand GetInsertValuesCommand<TModel>(SqlConnection connection, TModel model, string dataBaseName)
    {
        var sb = new StringBuilder($"INSERT INTO [dbo].[{dataBaseName}] (");
        var properties = model.GetType().GetProperties();
        var values = new Dictionary<string, object>();
        var parameters = new List<SqlParameter>();

        foreach (var property in properties)
        {
            sb.Append($"[{property.Name}], ");
            values.Add($"@{property.Name}", property.GetValue(model));
            parameters.Add(new SqlParameter($"@{property.Name}", property.GetValue(model)));
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(")\r\nVALUES (");

        foreach (var value in values)
        {
            sb.Append($"{value.Key}, ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(");");

        var sqlCommand = new SqlCommand(sb.ToString(), connection);
        sqlCommand.Parameters.AddRange(parameters.ToArray());
        return sqlCommand;
    }
}