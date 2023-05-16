using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public interface IOrderItemsReserverRepository
{
    Task SendOrderAsync(Order order);
}

public class OrderItemsReserverRepository : IOrderItemsReserverRepository
{
    private readonly IConfiguration _config;
    private readonly IQueueClient _queueClient;

    public OrderItemsReserverRepository(IConfiguration config)
    {
        _config = config;
        var serviceBusConnectionString = _config.GetSection("ServiceBusConnectionString").Value;
        string queueName = "orderitemreserverqueue";

        _queueClient = new QueueClient(serviceBusConnectionString, queueName);
    }

    public async Task SendOrderAsync(Order order)
    {
        var orderJson = JsonSerializer.Serialize(order);
        var message = new Message(Encoding.UTF8.GetBytes(orderJson));

        await _queueClient.SendAsync(message);
    }
}
