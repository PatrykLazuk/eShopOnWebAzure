using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace OrderItemsReserver;

public static class OrderItemsReserverFunction
{
    [FunctionName("OrderItemsReserver")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [Blob("orders/{rand-guid}.json", FileAccess.Write)] Stream outputBlob,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        // Read the order details from the HTTP request body
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic order = JsonConvert.DeserializeObject(requestBody);

        // Create a JSON string with the order details
        string orderJson = JsonConvert.SerializeObject(order);

        // Write the JSON string to the output blob
        byte[] orderBytes = Encoding.UTF8.GetBytes(orderJson);
        await outputBlob.WriteAsync(orderBytes, 0, orderBytes.Length);

        return new OkResult();
    }

}
