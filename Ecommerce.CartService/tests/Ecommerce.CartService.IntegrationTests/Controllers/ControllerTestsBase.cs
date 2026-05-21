using Ecommerce.CartService.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.CartService.IntegrationTests.Controllers
{
    public abstract class ControllerTestsBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Program> _factory;

        protected ControllerTestsBase(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        protected async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _client.GetAsync(url);
        }

        protected async Task<T?> GetFromJsonAsync<T>(string url)
        {
            return await _client.GetFromJsonAsync<T>(url);
        }

        protected async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T data)
        {
            return await _client.PostAsJsonAsync(url, data);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await _client.DeleteAsync(url);
        }
    }
}
