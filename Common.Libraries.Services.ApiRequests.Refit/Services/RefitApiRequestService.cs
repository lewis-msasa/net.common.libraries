using Common.Libraries.Services.ApiRequests.Services;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.ApiRequests.Refit.Services
{
    public interface IGenericRefitApi
    {
        [Get("")]
        Task<ApiResponse<T>> GetAsync<T>(string url);

        [Post("")]
        Task<ApiResponse<T>> PostAsync<T>(string url,[Body] object data);

        [Put("")]
        Task<ApiResponse<T>> PutAsync<T>(string url,[Body] object data);

        [Patch("")]
        Task<ApiResponse<T>> PatchAsync<T>(string url, [Body] object data);

        [Delete("")]
        Task<ApiResponse<T>> DeleteAsync<T>(string url, [Body] object data);

        [Post("")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<ApiResponse<T>> PostUrlEncodedAsync<T>(string url, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> formData);
    }
    public class RefitApiRequestService(IHttpClientFactory httpClientFactory) : IApiRequestService
    {
        public IGenericRefitApi CreateRefitClient(string url, Dictionary<string, string> headers)
        {
            var httpClient = httpClientFactory.CreateClient();

            httpClient.BaseAddress = new Uri(url);
            httpClient.Timeout = TimeSpan.FromSeconds(60);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return RestService.For<IGenericRefitApi>(httpClient);
        }
        private async Task LogIfNeeded<T>(Func<string, string, int, Task> log, string req, T res, int status) where T : class
        {
            if (log != null)
            {
                await log(req, JsonConvert.SerializeObject(res), status);
            }
        }
        public async Task<(T result, int statusCode)> GetAsync<T>(string url, Dictionary<string, string> headers, Func<string, string, int, Task> logRequest = null) where T : class
        {
            var client = CreateRefitClient(GetBaseUrl(url), headers);
            var path = GetRelativeUrl(url);
            var response = await client.GetAsync<T>(path);

            await LogIfNeeded<T>(logRequest, "GET " + url, response.Content, (int)response.StatusCode);
            return (response.Content, (int)response.StatusCode);
        }

        public async Task<(T result, int statusCode)> PostAsync<T>(string url, Dictionary<string, string> headers, object data, Func<string, string, int, Task> logRequest = null) where T : class
        {
            var client = CreateRefitClient(GetBaseUrl(url), headers);
            var response = await client.PostAsync<T>(url,data);

            await LogIfNeeded(logRequest, "POST " + url, response.Content, (int)response.StatusCode);
            return (response.Content, (int)response.StatusCode);
        }

        public async Task<(T result, int statusCode)> PutAsync<T>(string url, Dictionary<string, string> headers, object data, Func<string, string, int, Task> logRequest = null) where T : class
        {
            var client = CreateRefitClient(GetBaseUrl(url), headers);
            var response = await client.PutAsync<T>(url,data);

            await LogIfNeeded(logRequest, "PUT " + url, response.Content, (int)response.StatusCode);
            return (response.Content, (int)response.StatusCode);
        }

        public async Task<(T result, int statusCode)> PatchAsync<T>(string url, Dictionary<string, string> headers, object data, Func<string, string, int, Task> logRequest = null) where T : class
        {
            var client = CreateRefitClient(GetBaseUrl(url), headers);
            var response = await client.PatchAsync<T>(url,data);

            await LogIfNeeded(logRequest, "PATCH " + url, response.Content, (int)response.StatusCode);
            return (response.Content, (int)response.StatusCode);
        }

        public async Task<(T result, int statusCode)> DeleteAsync<T>(string url, Dictionary<string, string> headers, object data, Func<string, string, int, Task> logRequest = null) where T : class
        {
            var client = CreateRefitClient(GetBaseUrl(url), headers);
            var response = await client.DeleteAsync<T>(url,data);

            await LogIfNeeded(logRequest, "DELETE " + url, response.Content, (int)response.StatusCode);
            return (response.Content, (int)response.StatusCode);
        }

        public async Task<(T result, int statusCode)> PostUrlEncodeAsync<T>(string url, Dictionary<string, string> headers, Dictionary<string, string> data, Func<string, string, int, Task> logRequest = null) where T : class
        {
            var client = CreateRefitClient(GetBaseUrl(url), headers);
            var response = await client.PostUrlEncodedAsync<T>(url,data);

            await LogIfNeeded(logRequest, "POST URLENCODED " + url, response.Content, (int)response.StatusCode);
            return (response.Content, (int)response.StatusCode);
        }

        private string GetBaseUrl(string url)
        {
            var uri = new Uri(url);
            return uri.GetLeftPart(UriPartial.Authority);
        }
        private string GetRelativeUrl(string url)
        {
            var uri = new Uri(url);
            var baseUrl = uri.GetLeftPart(UriPartial.Authority);
            return url.Replace(baseUrl, "");
        }
    }

}
