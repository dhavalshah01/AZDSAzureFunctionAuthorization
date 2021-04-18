using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AYKA.AZDSQueueTriggerFunction
{
    public static class AZDSQueueTriggerFunction
    {
        [FunctionName("AZDSQueueTriggerFunction")]
        public static void Run([QueueTrigger("azdsqueueitems", Connection = "azdsauthenticationstorag_STORAGE")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            log.LogInformation(System.Environment.GetEnvironmentVariable("MyVaultSecret"));
        }
    }
}
