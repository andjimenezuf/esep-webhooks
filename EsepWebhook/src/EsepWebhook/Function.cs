using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    public string FunctionHandler(object input, ILambdaContext context)
    {
        context.Logger.LogInformation($"FunctionHandler received: {input}");

        // Deserialize the incoming JSON payload
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        string payload = $"{{\"text\":\"Issue Created: {json.issue.html_url}\"}}";

        // Retrieve the Slack URL from environment variable
        string slackUrl = Environment.GetEnvironmentVariable("SLACK_URL");

        // Prepare and send the HTTP request
        using (var client = new HttpClient())
        {
            var webRequest = new HttpRequestMessage(HttpMethod.Post, slackUrl)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            var response = client.Send(webRequest);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful
            
            // Return the response content if needed
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
