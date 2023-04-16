using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
    private readonly HttpClient _httpClient;

    public OrderItemsReserverRepository(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task SendOrderAsync(Order order)
    {
        var orderJson = JsonSerializer.Serialize(order);
        var content = new StringContent(orderJson, Encoding.UTF8, "application/json");

        var orderItemsReserverUrl = _config.GetSection("OrderItemsReserverUrl").Value;
        var response = await _httpClient.PostAsync(orderItemsReserverUrl, content);

        response.EnsureSuccessStatusCode();
    }
}
