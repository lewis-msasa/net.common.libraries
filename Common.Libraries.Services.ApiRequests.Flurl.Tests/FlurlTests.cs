namespace Common.Libraries.Services.ApiRequests.Flurl.Tests
{
    using Common.Libraries.Services.ApiRequests.Flurl.Services;
    using global::Flurl.Http.Testing;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class TestObject
    {
        public string Message {  get; set; }
       
    }
    public class FlurlApiRequestServiceTests
    {
        private readonly FlurlApiRequestService _service;

        public FlurlApiRequestServiceTests()
        {
            _service = new FlurlApiRequestService();
        }

        [Fact]
        public async Task PostUrlEncodedAsync_ShouldReturnExpectedResult()
        {
            // Arrange
            var expectedResponse = new TestObject { Message = "Created" };
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(expectedResponse, 201);

            var url = "https://api.example.com/resource";
            var headers = new Dictionary<string, string> { { "Content-Type", "application/x-www-form-urlencoded" }, { "Authorization", "Bearer token" } };
            var data = new Dictionary<string, string> { { "data1", "1" }, { "data2", "2" } };

            // Act
            var (result, statusCode) = await _service.PostUrlEncodeAsync<TestObject>(url, headers, data);

            // Assert
            Assert.Equal(201, statusCode);
            Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(result));

            httpTest.ShouldHaveCalled(url)
                    .WithVerb(HttpMethod.Post)
                    .WithHeaders(["Authorization", "Content-Type"])
                    .WithOAuthBearerToken("token")
                    .WithContentType("application/x-www-form-urlencoded")
                    .WithRequestUrlEncoded(data);
        }

        [Fact]
        public async Task PostAsync_ShouldReturnExpectedResult()
        {
            // Arrange
            var expectedResponse = new TestObject{ Message = "Created" };
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(expectedResponse, 201);

            var url = "https://api.example.com/resource";
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var data = new { Name = "New Resource" };

            // Act
            var (result, statusCode) = await _service.PostAsync<TestObject>(url, headers, data);

            // Assert
            Assert.Equal(201, statusCode);
            Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(result));
           
            httpTest.ShouldHaveCalled(url)
                    .WithVerb(HttpMethod.Post)
                    .WithHeaders(["Authorization","Content-Type"])
                    .WithOAuthBearerToken("token")
                    .WithContentType("application/json")
                    .WithRequestJson(data);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnExpectedResult()
        {
            // Arrange
            var expectedResponse = new TestObject{ Message = "Deleted" };
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(expectedResponse, 200);

            var url = "https://api.example.com/resource/1";
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var data = new { Id = 1 };

            // Act
            var (result, statusCode) = await _service.DeleteAsync<TestObject>(url, headers, data);

            // Assert
            Assert.Equal(200, statusCode);
            Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(result));
            httpTest.ShouldHaveCalled(url)
                    .WithVerb(HttpMethod.Delete)
                    .WithHeaders(["Authorization", "Content-Type"])
                    .WithOAuthBearerToken("token")
                    .WithContentType("application/json")
                    .WithRequestJson(data);
        }

        [Fact]
        public async Task PutAsync_ShouldReturnExpectedResult()
        {
            // Arrange
            var expectedResponse = new TestObject{ Message = "Updated" };
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(expectedResponse, 200);

            var url = "https://api.example.com/resource/1";
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var data = new { Name = "Updated Resource" };

            // Act
            var (result, statusCode) = await _service.PutAsync<TestObject>(url, headers, data);

            // Assert
            Assert.Equal(200, statusCode);
            Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(result));
            httpTest.ShouldHaveCalled(url)
                    .WithVerb(HttpMethod.Put)
                    .WithHeaders(["Authorization", "Content-Type"])
                    .WithOAuthBearerToken("token")
                    .WithContentType("application/json")
                    .WithRequestJson(data);
        }

        [Fact]
        public async Task PatchAsync_ShouldReturnExpectedResult()
        {
            // Arrange
            var expectedResponse = new TestObject{ Message = "Patched" };
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(expectedResponse, 200);

            var url = "https://api.example.com/resource/1";
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var data = new { Name = "Patched Resource" };

            // Act
            var (result, statusCode) = await _service.PatchAsync<TestObject>(url, headers, data);

            // Assert
            Assert.Equal(200, statusCode);
            Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(result));
            httpTest.ShouldHaveCalled(url)
                    .WithVerb(HttpMethod.Patch)
                    .WithHeaders(["Authorization", "Content-Type"])
                    .WithOAuthBearerToken("token")
                    .WithContentType("application/json")
                    .WithRequestJson(data);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnExpectedResult()
        {
            // Arrange
            var expectedResponse = new TestObject{ Message = "Success" };
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(expectedResponse, 200);

            var url = "https://api.example.com/resource/1";
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json"} ,{"Authorization", "Bearer token" } };

            // Act
            var (result, statusCode) = await _service.GetAsync<TestObject>(url, headers);

            // Assert
            Assert.Equal(200, statusCode);
            Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(result));
            httpTest.ShouldHaveCalled(url)
                    .WithVerb(HttpMethod.Get)
                    .WithHeaders(["Authorization"])
                    .WithContentType("application/json");
        }
    }


}
