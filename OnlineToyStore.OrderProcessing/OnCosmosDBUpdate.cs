using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace OnlineToyStore.OrderProcessing
{
    public static class OnCosmosDBUpdate
    {
        [FunctionName("OnCosmosDBUpdate")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "OnlineToyStore",
            collectionName: "Orders",
            ConnectionStringSetting = "AzureWebJobsCosmosDBConnectionString",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> updatedOrders,
            [SignalR(HubName = "orders")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            foreach(var order in updatedOrders)
            { 
                log.LogInformation("Documents modified " + updatedOrders.Count);
                log.LogInformation("First document Id " + updatedOrders[0].Id);
                await signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = "orderUpdated",
                    Arguments = new[] { order }
                });
            }
        }
    }
}
