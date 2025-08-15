using Common.Libraries.EventSourcing;
using Microsoft.AspNetCore.Mvc;

namespace Common.Libraries.EventStore.EF.TestApi.Controllers
{
    public abstract class CommandApi<T,TSnapshot> : ControllerBase
         where T : AggregateRoot<TSnapshot>, new() where TSnapshot : ISnapshot
    {
        readonly ILogger _log;

        protected CommandApi(
            ApplicationService<T,TSnapshot> applicationService,
            ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger(GetType());
            Service = applicationService;
        }

        ApplicationService<T, TSnapshot> Service { get; }

        protected async Task<IActionResult> HandleCommand<TCommand>(
            TCommand command,
            Action<TCommand> commandModifier = null)
        {
            try
            {
                _log.LogDebug(
                    "Handling HTTP request of type {type}",
                    typeof(T).Name
                );
                commandModifier?.Invoke(command);
                var r = await Service.Handle(command);
                return new OkResult();
            }
            catch (Exception e)
            {
                _log.LogError(e, "Error handling the command");

                return new BadRequestObjectResult(
                    new
                    {
                        error = e.Message,
                        stackTrace = e.StackTrace
                    }
                );
            }
        }

        protected Guid GetUserId() => Guid.Parse(User.Identity.Name);
    }
}
