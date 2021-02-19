using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;

namespace WifiPowerMonitor
{
    public static class WiFiPowerMonitor
    {
        private static string _powerbiUrl = "<the url of your powerbi dataset>";
        [FunctionName("WiFiPowerMonitor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var body = requestBody.Split('&');
            var power = body[1];
            var station = body[0];
            string message = power.ToString() + " " + station.ToString();
            log.LogInformation(message);


            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            
            //Generate content for http post request
            var PowerBIRow = new JObject(
                                    new JProperty("power", double.Parse(power)),
                                    new JProperty("station", station.ToString()),
                                    new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"))
                                );
            var content = new StringContent(PowerBIRow.ToString(), Encoding.UTF8, "application/json");

            //Define http client to push the row
            HttpClient powerbiclient = new HttpClient();
            HttpResponseMessage pushrow = await powerbiclient.PostAsync(_powerbiUrl, content);
            return new OkObjectResult(responseMessage);
        }
    }

}
