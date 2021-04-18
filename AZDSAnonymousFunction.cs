using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AYKA.AnonymousFunction
{
    public static class AZDSAnonymousFunction
    {
        [FunctionName("AZDSAnonymousFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
log.LogInformation("#################### ANONYMOUS LEVEL AUTHORIZATION FUNCTION #############################");
            log.LogInformation("C# HTTP trigger function processed a request.");

            var identity = req.HttpContext?.User?.Identity as ClaimsIdentity;
            log.LogInformation("IsAuthenticated: {isAuthenticated}", identity?.IsAuthenticated);
            log.LogInformation("Identity name: {name}", identity?.Name);
            log.LogInformation("AuthenticationType: {authenticationType}",
                identity?.AuthenticationType);
            foreach (var claim in identity?.Claims)
            {
                log.LogInformation("Claim: {type} : {value}", claim.Type, claim.Value);
            }
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "Azure Function Triggered Successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hola! {name}. Azure Function Triggered Successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
