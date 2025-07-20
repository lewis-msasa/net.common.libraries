# .NET Common Libraries

This project contains projects/libraries for using some of the most common .NET features e.g. Databases, Message Queues, Emails, Logging, QR codes, API calls

## Features

- Event Bus
- Message Queues
- Databases
- Emailing
- Logging

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- Optional: Visual Studio or VS Code

### Installation

Clone the repository:

```bash
git clone https://github.com/lewis-msasa/net.common.libraries.git

```

### Usage

#### API REQUEST CALLS

Project: `Common.Libraries.Services`

Folder: `Services`

File: `IApiRequestService`

Usage: You can use it to implement your own RequestService using your preferred packages e.g. Flurl

##### Example Flurl Implementation for the api request service

Project: `Common.Libraries.Services.Flurl`

Folder: `Services`

File: `FlurlApiRequestService`

Don't forget to do your dependency injection e.g.

```csharp

builder.Services.AddTransient<IApiRequestService,FlurlApiRequestService>();

```

##### Usage Example

Here’s how to use the `IApiRequestService` to send a GET request:

```csharp
public class MyService
{
    private readonly IApiRequestService _apiRequestService;

    public MyService(IApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task FetchDataAsync()
    {
        string url = "https://api.example.com/items";
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer your-token" }
        };

        var (result, statusCode) = await _apiRequestService.GetAsync<MyResponseModel>(url, headers, logRequest: async (req, resp, code) =>
                    {
                        await Task.Run(() =>
                        {
                            log.RawRequest = JsonConvert.SerializeObject(data);
                            log.RawResponse = resp;
                            log.RequestUrl = url;
                            log.ResponseStatusCode = code;
                            log.ResponseTime = DateTime.Now;
                        });

                    });

        if (statusCode == 200 && result != null)
        {
            Console.WriteLine("Success: " + result.Name);
        }
        else
        {
            Console.WriteLine($"Request failed with status: {statusCode}");
        }
    }
}

```

#### Databases

We use them as a services calling repositories. They can also be used using Unit of Work

Project: `Common.Libraries.Services`

Folder: `Services`

Files: `IService`, `Service`

Usage: You can use it to implement IRepository your own Repository using your preferred packages e.g. EFCore

##### Example EF Core

Project: `Common.Libraries.Services.EFCore`

Folder: `Repositories`

Files: `EFRepository`

dependency injection.

```csharp

 builder.Services.AddTransient<IRepository<MyEFEntity>, EFRepository<MyEFEntity, DBContext>>();

 builder.Services.AddTransient<IService<MyEFEntity, MyEFEntityDto>, Service<MyEFEntity, MyEFEntityDto>>();

```

```csharp
 app.MapPost(
  "getData/{id}",
  async (
       string id,
       IService<MyEFEntity, MyEFEntityDto> _myServiceervice
      ) =>
  {
     
      var entity = await _myService.GetOneAsync(t => t.id == id,
          includeString: ["property","anotherProperty"]);
    
       return Results.Ok(entity);
  });
```





