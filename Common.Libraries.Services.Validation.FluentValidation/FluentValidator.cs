using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Validation.FluentValidation
{
    public class FluentRequestValidator<T> : IRequestValidator<T>
    {
        private readonly IValidator<T> _validator;

        public FluentRequestValidator(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async Task<ValidationResult> ValidateAsync(T request)
        {
            var result = await _validator.ValidateAsync(request);
            return new ValidationResult
            {
                Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }
    }
}
