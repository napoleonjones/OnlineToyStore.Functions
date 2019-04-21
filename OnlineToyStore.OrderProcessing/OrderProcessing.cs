using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using OnlineToyStore.Core.Models;
using OnlineToyStore.Core.Repositories;

namespace OnlineToyStore.OrderProcessing
{
    public static class OrderProcessing
    {
        [FunctionName("OrderProcessing")]
        public async static void Run([QueueTrigger("onlinetoystore-orderprocessing", Connection = "AzureWebJobsStorage")] Order order, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {order.Id}");

            CalculateDiscount(order);
            UpdateTotal(order);

            var repository = new OrdersRepository<Order>();

            await repository.CreateItemAsync(order);
        }

        private static void CalculateDiscount(Order order)
        {
            int totalQuantity = order.LineItems.Sum(x => x.Quantity);

            if (totalQuantity >= 10)
            {
                order.Discount = .10M;
            }
            else if (totalQuantity >=5)
            {
                order.Discount = .05M;
            }
        }

        private static void UpdateTotal(Order order)
        {
            decimal runningTotal = 0M;

            foreach(var lineItem in order.LineItems)
            {
                runningTotal += lineItem.Product.UnitPrice * lineItem.Quantity;
            }

            order.TotalPrice = runningTotal * (1 - order.Discount);
        }

    }
}
