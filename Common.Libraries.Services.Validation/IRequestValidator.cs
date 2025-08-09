using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Validation
{
    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();

        public List<string> Errors { get; init; } = new List<string>();

    }
    public interface IRequestValidator<T>
    {
        Task<ValidationResult> ValidateAsync(T request);
    }
}
