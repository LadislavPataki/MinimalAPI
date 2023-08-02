using FluentValidation;

namespace MinimalApiReprV2.Features.LoadTest;

public class LoadTestRequestValidator : AbstractValidator<LoadTestRequest>
{
    public LoadTestRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}