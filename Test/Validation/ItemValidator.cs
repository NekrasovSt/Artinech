using Common.Interfaces;
using FluentValidation;

namespace Test.Validation;

internal class ItemValidator : AbstractValidator<IItem>
{
    public const int MAX_WEIGHT = 100;

    public ItemValidator()
    {
        RuleFor(customer => customer.Weight)
            .LessThan(MAX_WEIGHT)
            .GreaterThan(0);
    }
}