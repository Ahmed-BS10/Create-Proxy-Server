using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Proxy_Server.ProxyEntity;

namespace ProxyServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ProxyController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> HandleRequest([FromBody] ProxyRequest proxyRequest)
        {
            if (!Uri.IsWellFormedUriString(proxyRequest.Url, UriKind.Absolute))
            {
                return BadRequest("The provided URL is not valid.");
            }

            var requestMessage = new HttpRequestMessage(new HttpMethod(proxyRequest.Method), proxyRequest.Url);

            foreach (var header in proxyRequest.Headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (proxyRequest.Body != null)
            {
                requestMessage.Content = new StringContent(proxyRequest.Body, System.Text.Encoding.UTF8, "application/json");
            }

            var responseMessage = await _httpClient.SendAsync(requestMessage);

            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            var responseHeaders = responseMessage.Headers.ToDictionary(h => h.Key, h => string.Join(",", h.Value));

            var proxyResponse = new ProxyResponse
            {
                StatusCode = (int)responseMessage.StatusCode,
                Headers = responseHeaders,
                Body = responseContent
            };

            return Ok(proxyResponse);
        }
    }
}
