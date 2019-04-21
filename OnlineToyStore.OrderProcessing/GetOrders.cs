using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnlineToyStore.Core.Models;
using System.Collections.Generic;

namespace OnlineToyStore.OrderProcessing
{
    public static class GetOrders
    {
        [FunctionName("GetOrders")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetOrders")] HttpRequest req, 
            [CosmosDB("OnlineToyStore", "Orders", ConnectionStringSetting ="AzureWebJobsCosmosDBConnectionString")] IEnumerable<Order> orders,
            ILogger log)
        {
            return new OkObjectResult(orders);
        }
    }
}
