using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Src.Services
{
    public interface ISysMLApiService
    {
        Task<string> GetModelAsync(string modelId);
        Task<string> CreateModelAsync(string modelData);
        Task<string> UpdateModelAsync(string modelId, string modelData);
        Task<bool> DeleteModelAsync(string modelId);
    }

    public class SysMLApiService : ISysMLApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SysMLApiService> _logger;
        private const string BaseUrl = "https://sysmlv2-api.example.com/api/models";

        public SysMLApiService(HttpClient httpClient, ILogger<SysMLApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetModelAsync(string modelId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{modelId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CreateModelAsync(string modelData)
        {
            var content = new StringContent(modelData, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(BaseUrl, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UpdateModelAsync(string modelId, string modelData)
        {
            var content = new StringContent(modelData, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{BaseUrl}/{modelId}", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> DeleteModelAsync(string modelId)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{modelId}");
            return response.IsSuccessStatusCode;
        }
    }
}