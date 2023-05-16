using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace OrderItemsReserver;

public static class OrderItemsReserverFunction
{
    #region cosmosDBapproach
    //[FunctionName("OrderItemsReserver")]
    //public static async Task<IActionResult> Run(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
    //    [CosmosDB(
    //        databaseName: "eshoponline",
    //        containerName: "orders",
    //        Connection = "CosmosDBConnectionString")] IAsyncCollector<dynamic> documents,
    //    ILogger log)
    //{
    //    log.LogInformation("C# HTTP trigger function processed a request.");

    //    // Read the order details from the HTTP request body
    //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    //    dynamic order = JsonConvert.DeserializeObject(requestBody);

    //    // Store the order in Cosmos DB
    //    await documents.AddAsync(new
    //    {
    //        // create a random ID
    //        id = System.Guid.NewGuid().ToString(),
    //        order = order
    //    });

    //    return new OkResult();
    //}
    #endregion

    #region justBlobStorage
    //[FunctionName("OrderItemsReserver")]
    //public static async Task<IActionResult> Run(
    //    [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
    //    [Blob("orders/{rand-guid}.json", FileAccess.Write)] Stream outputBlob,
    //    ILogger log)
    //{
    //    log.LogInformation("C# HTTP trigger function processed a request.");

    //    // Read the order details from the HTTP request body
    //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    //    dynamic order = JsonConvert.DeserializeObject(requestBody);

    //    // Create a JSON string with the order details
    //    string orderJson = JsonConvert.SerializeObject(order);

    //    // Write the JSON string to the output blob
    //    byte[] orderBytes = Encoding.UTF8.GetBytes(orderJson);
    //    await outputBlob.WriteAsync(orderBytes, 0, orderBytes.Length);

    //    return new OkResult();
    //}
    #endregion

    [FunctionName("OrderItemsReserver")]
    [FixedDelayRetry(3, "00:00:10")]
    public static async Task Run(
        [ServiceBusTrigger("orderitemreserverqueue", Connection = "ServiceBusConnectionString")] string orderJson,
        [Blob("orders/{rand-guid}.json", FileAccess.Write)] Stream outputBlob,
        ILogger log)
    {
        try
        {
            byte[] orderBytes = Encoding.UTF8.GetBytes(orderJson);
            await outputBlob.WriteAsync(orderBytes, 0, orderBytes.Length);

            log.LogInformation("Order request file uploaded to Blob Storage.");
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to upload the order to Blob Storage.");

            // Throw a new exception to trigger the Logic App
            throw new Exception("Failed to upload the order to Blob Storage.", ex);
        }
    }
}
