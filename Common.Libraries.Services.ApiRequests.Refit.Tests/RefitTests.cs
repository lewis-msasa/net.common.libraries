using Common.Libraries.Services.ApiRequests.Refit.Services;
using Moq;
using Moq.Protected;
using Refit;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Common.Libraries.Services.ApiRequests.Refit.Tests
{
   
    public class TestObject
    {
        public string Message { get; set; }
    }
    public class RefitApiRequestServiceTests
    {
        private readonly Mock<IHttpClientFactory> _factory;
        private readonly Mock<IGenericRefitApi> _apiMock;
        private readonly RefitApiRequestService _service;

        public RefitApiRequestServiceTests()
        {
            _factory = new Mock<IHttpClientFactory>();
            _apiMock = new Mock<IGenericRefitApi>();

            // Inject factory that always returns a dummy HttpClient (unused since CreateRefitClient is overridden)
            var httpClient = new HttpClient(new FakeHandler());
            _factory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Create service with override for Refit client creation
            _service = new RefitApiRequestService(_factory.Object);
        }

        [Fact]
        public async Task GetAsync_ReturnsExpectedResult()
        {
            // Arrange
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var expected = new TestObject { Message = "Hey yoh" };
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonSerializer.Serialize(expected), Encoding.UTF8, "application/json")
               });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://fakeapi.com")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var service = new RefitApiRequestService(httpClientFactory.Object);

            // Act
            var (result, statusCode) = await service.GetAsync<TestObject>("https://fakeapi.com", headers);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Message, result.Message);
            Assert.Equal(200, statusCode);
        }

        

        [Fact]
        public async Task PostAsync_InvokeRefitAndReturn()
        {


            // Arrange
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var expected = new TestObject { Message = "Hey yoh" };
            var data = new { Name = "New Resource" };
            var handlerMock = new Mock<HttpMessageHandler>();
            HttpRequestMessage captureRequest = null;
          
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .Callback<HttpRequestMessage,CancellationToken>((request,_) =>
               {
                   captureRequest = request;

               })
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonSerializer.Serialize(expected), Encoding.UTF8, "application/json")
               });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://fakeapi.com")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var service = new RefitApiRequestService(httpClientFactory.Object);

            // Act
            var (result, statusCode) = await service.PostAsync<TestObject>("https://fakeapi.com", headers,data);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Message, result.Message);
            Assert.Equal(200, statusCode);
            Assert.True(captureRequest.Headers.Contains("Authorization"));
            Assert.Equal("token", captureRequest.Headers.Authorization?.Parameter);
        }

        [Fact]
        public async Task PutAsync_InvokeRefitAndReturn()
        {
            // Arrange
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var expected = new TestObject { Message = "Hey yoh" };
            var data = new { Name = "New Resource" };
            var handlerMock = new Mock<HttpMessageHandler>();
            HttpRequestMessage captureRequest = null;

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
               {
                   captureRequest = request;

               })
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.Accepted,
                   Content = new StringContent(JsonSerializer.Serialize(expected), Encoding.UTF8, "application/json")
               });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://fakeapi.com")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var service = new RefitApiRequestService(httpClientFactory.Object);

            // Act
            var (result, statusCode) = await service.PutAsync<TestObject>("https://fakeapi.com", headers, data);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Message, result.Message);
            Assert.Equal(202, statusCode);
            Assert.True(captureRequest.Headers.Contains("Authorization"));
            Assert.Equal("token", captureRequest.Headers.Authorization?.Parameter);
        }

        [Fact]
        public async Task PatchAsync_InvokeRefitAndReturn()
        {
            // Arrange
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var expected = new TestObject { Message = "Hey yoh" };
            var data = new { Name = "New Resource" };
            var handlerMock = new Mock<HttpMessageHandler>();
            HttpRequestMessage captureRequest = null;

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
               {
                   captureRequest = request;

               })
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonSerializer.Serialize(expected), Encoding.UTF8, "application/json")
               });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://fakeapi.com")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var service = new RefitApiRequestService(httpClientFactory.Object);

            // Act
            var (result, statusCode) = await service.PatchAsync<TestObject>("https://fakeapi.com", headers, data);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Message, result.Message);
            Assert.Equal(200, statusCode);
            Assert.True(captureRequest.Headers.Contains("Authorization"));
            Assert.Equal("token", captureRequest.Headers.Authorization?.Parameter);
        }

        [Fact]
        public async Task DeleteAsync_InvokeRefitAndReturn()
        {
            // Arrange
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var expected = new TestObject { Message = "Hey yoh" };
            var data = new { Name = "New Resource" };
            var handlerMock = new Mock<HttpMessageHandler>();
            HttpRequestMessage captureRequest = null;

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
               {
                   captureRequest = request;

               })
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonSerializer.Serialize(expected), Encoding.UTF8, "application/json")
               });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://fakeapi.com")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var service = new RefitApiRequestService(httpClientFactory.Object);

            // Act
            var (result, statusCode) = await service.DeleteAsync<TestObject>("https://fakeapi.com", headers, data);

           
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Message, result.Message);
            Assert.Equal(200, statusCode);
            Assert.True(captureRequest.Headers.Contains("Authorization"));
            Assert.Equal("token", captureRequest.Headers.Authorization?.Parameter);
        }

        [Fact]
        public async Task PostUrlEncodedAsync_InvokeRefitAndReturn()
        {
            var data = new Dictionary<string, string> { { "data1", "1" }, { "data2", "2" } };
            // Arrange
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", "Bearer token" } };
            var expected = new TestObject { Message = "Hey yoh" };
            var handlerMock = new Mock<HttpMessageHandler>();
            HttpRequestMessage captureRequest = null;

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
               {
                   captureRequest = request;

               })
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonSerializer.Serialize(expected), Encoding.UTF8, "application/x-www-form-urlencoded")
               });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://fakeapi.com")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var service = new RefitApiRequestService(httpClientFactory.Object);

            // Act
            var (result, statusCode) = await service.PostUrlEncodeAsync<TestObject>("https://fakeapi.com", headers, data);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Message, result.Message);
            Assert.Equal(200, statusCode);
            Assert.True(captureRequest.Headers.Contains("Authorization"));
            Assert.Equal("token", captureRequest.Headers.Authorization?.Parameter);
        }

        
        private class FakeHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
