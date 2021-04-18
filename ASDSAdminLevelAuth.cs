using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Threading;
using System.Collections.Generic;

namespace AYKA.AZDSAdminLevelAuth
{
    public class ASDSAdminLevelAuth
    {
        private readonly TelemetryClient telemetryClient;
        public ASDSAdminLevelAuth(TelemetryConfiguration config)
        {
            this.telemetryClient = new TelemetryClient(config);
        }
        private static string GetEnvironmentVariable(string name)
        {
            return name + ": " +
                System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
        [FunctionName("ASDSAdminLevelAuth")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("####################ADMIN LEVEL AUTHORIZATION FUNCTION#############################");
            log.LogInformation("C# HTTP trigger function processed a request.");
            DateTime start = DateTime.UtcNow;


            var identity = req.HttpContext?.User?.Identity as System.Security.Claims.ClaimsIdentity;
            log.LogInformation("IsAuthenticated: {isAuthenticated}", identity?.IsAuthenticated);
            log.LogInformation("Identity name: {name}", identity?.Name);
            log.LogInformation("AuthenticationType: {authenticationType}",
                identity?.AuthenticationType);
            foreach (var claim in identity?.Claims)
            {
                log.LogInformation("Claim: {type} : {value}", claim.Type, claim.Value);
            }
            log.LogInformation(GetEnvironmentVariable("AzureWebJobsStorage"));
            log.LogInformation(GetEnvironmentVariable("WEBSITE_SITE_NAME"));
            log.LogInformation(GetEnvironmentVariable("ENV_VAR_1"));
            log.LogInformation(GetEnvironmentVariable("ENV_VAR_2"));
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            var evt = new EventTelemetry("Function called");
            evt.Context.User.Id = name;
            this.telemetryClient.TrackEvent(evt);
            Thread.Sleep(1000);
            var rand = new Random();
            // Generate a custom metric, in this case let's use ContentLength.
            this.telemetryClient.GetMetric("contentLength").TrackValue(req.ContentLength);
            var sample = new MetricTelemetry();
            sample.Name = "DSHAHMetric";
            sample.Value = rand.Next(101);
            this.telemetryClient.TrackMetric(sample);
            try
            {
                int zero = 0;
                int i = (1 / zero);
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
            }

            telemetryClient.TrackTrace("Slow database response",
                            SeverityLevel.Warning,
                            new Dictionary<string, string> { { "customProp1", rand.Next(101).ToString() }, { "customProp2", rand.Next(1000).ToString() } });
            // Log a custom dependency in the dependencies table.
            var dependency = new DependencyTelemetry
            {
                Name = "GET api/planets/1/",
                Target = "swapi.co",
                Data = "https://swapi.co/api/planets/1/",
                Timestamp = start,
                Duration = DateTime.UtcNow - start,
                Success = true
            };
            dependency.Context.User.Id = name;
            this.telemetryClient.TrackDependency(dependency);
            return new OkObjectResult(responseMessage);
        }
    }
}
