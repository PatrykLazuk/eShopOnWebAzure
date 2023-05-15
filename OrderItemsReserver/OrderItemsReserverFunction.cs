using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrderItemsReserver;

public static class OrderItemsReserverFunction
{
    [FunctionName("OrderItemsReserver")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        [CosmosDB(
            databaseName: "eshoponline",
            containerName: "orders",
            Connection = "CosmosDBConnectionString")] IAsyncCollector<dynamic> documents,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        // Read the order details from the HTTP request body
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic order = JsonConvert.DeserializeObject(requestBody);

        // Store the order in Cosmos DB
        await documents.AddAsync(new
        {
            // create a random ID
            id = System.Guid.NewGuid().ToString(),
            order = order
        });

        return new OkResult();
    }

}
