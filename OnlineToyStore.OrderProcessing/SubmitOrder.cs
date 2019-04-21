using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnlineToyStore.Core.Models;
using OnlineToyStore.Core.Interfaces;
using OnlineToyStore.Core.Repositories;



namespace OnlineToyStore.OrderProcessing
{
    public static class SubmitOrder
    {
        private const string QueueEndPoint = "DefaultEndpointsProtocol=https;AccountName=onlinetoystorestorage;AccountKey=AaiVztcTzXAZiqv7TDlpQJlSCeCQ91P2jT/sRS6iUAgW0odMZkuLiVnjuTrd0kznzoiobFFsfX9IZqG4rQ5cAg==;EndpointSuffix=core.windows.net";
        private static readonly IOrdersRepository<Order> Repository = new OrdersRepository<Order>();

        [FunctionName("SubmitOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Orders")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            if (string.IsNullOrEmpty(requestBody))
            {
                return new BadRequestObjectResult("Object not valid");
            }
            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            var storageAccount = CloudStorageAccount.Parse(QueueEndPoint);

            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("onlinetoystore-orderprocessing");

            await queue.CreateIfNotExistsAsync();

            var message = new CloudQueueMessage(JsonConvert.SerializeObject(order));

            await queue.AddMessageAsync(message);


            return new OkObjectResult(order);
        }
    }
}
