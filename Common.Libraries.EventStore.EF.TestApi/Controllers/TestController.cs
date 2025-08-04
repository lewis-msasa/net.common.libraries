using Common.Libraries.EventSourcing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Common.Libraries.EventStore.EF.TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : CommandApi<User>
    {
        public TestController(
           ApplicationService<User> applicationService,
           ILoggerFactory loggerFactory)
           : base(applicationService, loggerFactory) { }

        [HttpPost]
        public Task<IActionResult> Post(CreateRequest request)
        {
              
              return HandleCommand(new Create
              {
                   Id = Guid.NewGuid(),
                   Name = request.Name,
                   SpouseId = Guid.NewGuid()
              });

        }
    }
}
