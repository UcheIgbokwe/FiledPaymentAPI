using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Domain.Validation
{
    public static class ValidationExtension
    {
        public static bool IsValid(this Payment applicant, out IEnumerable<string> errors)
        {
            var validator = new PaymentModelValidator();

            var validationResult = validator.Validate(applicant);
            errors = AggregateErrors(validationResult);

            return validationResult.IsValid;
        }

        private static List<string> AggregateErrors(ValidationResult validationResult)
        {
            var errors = new List<string>();

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                    errors.Add(error.ErrorMessage);
            }

            return errors;
        }
    }
}