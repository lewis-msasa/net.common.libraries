using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Validation.FluentValidation
{
    public static class FluentValidationExtensions
    {
        /// <summary>
        /// Validates that the string property is a valid absolute URL.
        /// </summary>
        public static IRuleBuilderOptions<T, string?> MustBeValidUrl<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .Must(url => !string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("{PropertyName} must be a well‑formed absolute URL.");
        }
        public static IRuleBuilderOptions<T, string?> MustBeValidMalawiPhone<T>(
       this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .Must(phone =>
                {
                    if (string.IsNullOrWhiteSpace(phone)) return false;

                    // Normalize by removing spaces, dashes, etc.
                    var digitsOnly = new string(phone.Where(c => char.IsDigit(c) || c == '+').ToArray());

                    // Regex: optional +265 prefix, then 7–9 digits
                    var pattern = @"^(\+265)?\d{7,9}$";
                    return System.Text.RegularExpressions.Regex.IsMatch(digitsOnly, pattern);
                })
                .WithMessage("{PropertyName} must be a valid Malawian phone number (local or +265 format).");
        }
    }

}
