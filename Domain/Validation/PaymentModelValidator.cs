using System;
using FluentValidation;
using FluentValidation.Results;

namespace Domain.Validation
{
    public class PaymentModelValidator : AbstractValidator<Payment>
    {
        public PaymentModelValidator()
        {
            RuleFor(p => p.CreditCardNumber).NotEmpty().WithMessage("Credit Card is required").CreditCard().WithMessage("Credit Card must be valid");
            RuleFor(p => p.CardHolder).NotEmpty().WithMessage("Card holder is required");
            RuleFor(p => p.ExpirationDate).NotEmpty().WithMessage("Expiration date is required").Must(BeAValidDate).WithMessage("Expiration date must be in the future.");
            RuleFor(p => p.SecurityCode).Length(3,3).WithMessage("Security code must be 3 characters.");
            RuleFor(p => p.Amount).ScalePrecision(2,5);
        }

        protected override bool PreValidate(ValidationContext<Payment> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "Please submit a non-null model"));

                return false;
            }
            return true;
        }

        private bool BeAValidDate(DateTime date)
        {
            return date >= DateTime.Now;
        }
    }
}