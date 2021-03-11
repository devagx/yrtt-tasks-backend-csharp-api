using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AwsDotnetCsharp.Model;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AwsDotnetCsharp
{
    public class TaskHandler
    {
        private string DB_HOST = System.Environment.GetEnvironmentVariable("DB_HOST");
        private string DB_PORT = System.Environment.GetEnvironmentVariable("DB_PORT");
        private string DB_NAME = System.Environment.GetEnvironmentVariable("DB_NAME");
        private string DB_USER = System.Environment.GetEnvironmentVariable("DB_USER");
        private string DB_PASSWORD = System.Environment.GetEnvironmentVariable("DB_PASSWORD");
        public APIGatewayProxyResponse GetTasks(APIGatewayProxyRequest request)
        {
            LambdaLogger.Log("Entering method: APIGatewayProxyResponse");

            string userId = request.PathParameters["userId"];
            LambdaLogger.Log("Getting tasks for: " + userId);

            LambdaLogger.Log("Setting up environment variables: " + userId);
            string connStr = $"server={DB_HOST};user={DB_USER};database={DB_NAME};port={DB_PORT};password={DB_PASSWORD}";
            MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM task WHERE userId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);

            MySqlDataReader reader = cmd.ExecuteReader();
            ArrayList tasks = new ArrayList();

            while (reader.Read())
            {
                tasks.Add(new Task(reader.GetString("taskId"), reader.GetString("description"), reader.GetBoolean("completed")));
            }
            connection.Close();

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(tasks),
                //Tell the application what format the data is in.
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                },
                StatusCode = 200
            };

        }
        public APIGatewayProxyResponse SaveTask(APIGatewayProxyRequest request)
        {
            LambdaLogger.Log("Entering method: APIGatewayProxyResponse");

            string requestBody = request.Body;
            Task t = JsonSerializer.Deserialize<Task>(requestBody);
            LambdaLogger.Log("Saving Task: " + t.Description);
            return new APIGatewayProxyResponse
            {
                Body = "Task Saved",
                //Tell the application what format the data is in.
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                },
                StatusCode = 200
            };
        }
    }
}
