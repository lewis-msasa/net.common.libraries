﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.EventSourcing
{
   /* public abstract class CommandApi<T> 
         where T : AggregateRoot
    {
        readonly ILogger _log;

        protected CommandApi(
            ApplicationService<T> applicationService,
            ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger(GetType());
            Service = applicationService;
        }

        ApplicationService<T> Service { get; }

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
                await Service.Handle(command);
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
    }*/
}
