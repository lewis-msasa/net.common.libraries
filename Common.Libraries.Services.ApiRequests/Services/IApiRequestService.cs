using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.ApiRequests.Services
{
    /// <summary>
    /// Defines an abstraction for making HTTP API requests (GET, POST, PUT, DELETE, PATCH)
    /// with support for headers, payloads, and optional logging.
    /// </summary>
    public interface IApiRequestService
    {
        /// <summary>
        /// Sends a POST request with `application/x-www-form-urlencoded` content type.
        /// </summary>
        /// <typeparam name="T">The expected response type (will be deserialized).</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="headers">HTTP headers to include in the request.</param>
        /// <param name="data">Form data to be URL-encoded and sent in the body.</param>
        /// <param name="logRequest">Optional async logging function: accepts request body, response body, and status code.</param>
        /// <returns>A tuple containing the deserialized result and HTTP status code.</returns>
        Task<(T result, int statusCode)> PostUrlEncodeAsync<T>(
            string url,
            Dictionary<string, string> headers,
            Dictionary<string, string> data,
            Func<string, string, int, Task> logRequest = null
        ) where T : class;

        /// <summary>
        /// Sends a DELETE request with an optional request body.
        /// </summary>
        /// <typeparam name="T">The expected response type (will be deserialized).</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="headers">HTTP headers to include in the request.</param>
        /// <param name="data">Optional object to be serialized and sent in the body.</param>
        /// <param name="logRequest">Optional async logging function: accepts request body, response body, and status code.</param>
        /// <returns>A tuple containing the deserialized result and HTTP status code.</returns>
        Task<(T result, int statusCode)> DeleteAsync<T>(
            string url,
            Dictionary<string, string> headers,
            object data,
            Func<string, string, int, Task> logRequest = null
        ) where T : class;

        /// <summary>
        /// Sends a GET request.
        /// </summary>
        /// <typeparam name="T">The expected response type (will be deserialized).</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="headers">HTTP headers to include in the request.</param>
        /// <param name="logRequest">Optional async logging function: accepts request body, response body, and status code.</param>
        /// <returns>A tuple containing the deserialized result and HTTP status code.</returns>
        Task<(T result, int statusCode)> GetAsync<T>(
            string url,
            Dictionary<string, string> headers,
            Func<string, string, int, Task> logRequest = null
        ) where T : class;

        /// <summary>
        /// Sends a PATCH request with a JSON body.
        /// </summary>
        /// <typeparam name="T">The expected response type (will be deserialized).</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="headers">HTTP headers to include in the request.</param>
        /// <param name="data">Object to be serialized and sent in the body.</param>
        /// <param name="logRequest">Optional async logging function: accepts request body, response body, and status code.</param>
        /// <returns>A tuple containing the deserialized result and HTTP status code.</returns>
        Task<(T result, int statusCode)> PatchAsync<T>(
            string url,
            Dictionary<string, string> headers,
            object data,
            Func<string, string, int, Task> logRequest = null
        ) where T : class;

        /// <summary>
        /// Sends a POST request with a JSON body.
        /// </summary>
        /// <typeparam name="T">The expected response type (will be deserialized).</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="headers">HTTP headers to include in the request.</param>
        /// <param name="data">Object to be serialized and sent in the body.</param>
        /// <param name="logRequest">Optional async logging function: accepts request body, response body, and status code.</param>
        /// <returns>A tuple containing the deserialized result and HTTP status code.</returns>
        Task<(T result, int statusCode)> PostAsync<T>(
            string url,
            Dictionary<string, string> headers,
            object data,
            Func<string, string, int, Task> logRequest = null
        ) where T : class;

        /// <summary>
        /// Sends a PUT request with a JSON body.
        /// </summary>
        /// <typeparam name="T">The expected response type (will be deserialized).</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="headers">HTTP headers to include in the request.</param>
        /// <param name="data">Object to be serialized and sent in the body.</param>
        /// <param name="logRequest">Optional async logging function: accepts request body, response body, and status code.</param>
        /// <returns>A tuple containing the deserialized result and HTTP status code.</returns>
        Task<(T result, int statusCode)> PutAsync<T>(
            string url,
            Dictionary<string, string> headers,
            object data,
            Func<string, string, int, Task> logRequest = null
        ) where T : class;
    }

    public class Auth
    {
        public string Token { get; set; }
        public AuthType AuthType { get; set; }
    }
    public enum AuthType
    {
        BEARER,
        BASIC

    }
}
