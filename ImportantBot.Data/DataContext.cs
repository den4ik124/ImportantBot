using Microsoft.Data.SqlClient;
using System.Text;

namespace ImportantBot.Data;

public class DataContext
{
    private readonly string _sqlConnection;
    private const string TableName = "ChatMessages";

    public DataContext(string sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task InsertData<TModel>(TModel message)
    {
        var connectionString = new SqlConnectionStringBuilder(_sqlConnection);

        using var connection = new SqlConnection(connectionString.ConnectionString);
        var sqlCommand = GetInsertValuesCommand(connection, message);
        await connection.OpenAsync();
        var numberOfNotes = await sqlCommand.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyCollection<TModel>> GetMessages<TModel>(long chatId)
    {
        var connectionString = new SqlConnectionStringBuilder(_sqlConnection);

        using var connection = new SqlConnection(connectionString.ConnectionString);

        var dateTimeFrom = DateTime.Now.AddDays(-1);
        var sqlCommandText = $"SELECT * FROM [dbo].[{TableName}] WHERE [DateTime] >= @dateTimeFrom AND [ChatId] = @chatId";
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
            var messages = await GetMessagesFromDb<TModel>(connection, sqlCommand);
            return messages;
        }
        catch (Exception ex)
        {
            throw ex;
        }
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

    private static async Task<IReadOnlyCollection<TModel>> GetMessagesFromDb<TModel>(SqlConnection connection, SqlCommand sqlCommand)
    {
        var messages = new List<TModel>();
        using (connection)
        {
            await connection.OpenAsync();
            var reader = sqlCommand.ExecuteReader();

            while (await reader.ReadAsync())
            {
                var properties = typeof(TModel).GetProperties();

                var message = Activator.CreateInstance<TModel>();

                foreach (var property in properties)
                {
                    property.SetValue(message, reader[property.Name]);
                }

                messages.Add(message);
            }
        }

        return messages;
    }

    private static SqlCommand GetInsertValuesCommand<TModel>(SqlConnection connection, TModel model)
    {
        var sb = new StringBuilder($"INSERT INTO [dbo].[{TableName}] (");
        var properties = model?.GetType().GetProperties()
            ?? throw new ArgumentNullException(nameof(model));
        var values = new List<string>();
        var parameters = new List<SqlParameter>();

        foreach (var property in properties)
        {
            sb.Append($"[{property.Name}], ");
            values.Add($"@{property.Name}");
            parameters.Add(new SqlParameter($"@{property.Name}", property.GetValue(model)));
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(")\r\nVALUES (");

        foreach (var value in values)
        {
            sb.Append($"{value}, ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(");");

        var sqlCommand = new SqlCommand(sb.ToString(), connection);
        sqlCommand.Parameters.AddRange(parameters.ToArray());
        return sqlCommand;
    }
}