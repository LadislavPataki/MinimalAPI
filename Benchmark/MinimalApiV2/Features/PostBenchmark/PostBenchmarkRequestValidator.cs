using FluentValidation;

namespace MinimalApiReprV2.Features.PostBenchmark;

public class PostBenchmarkRequestValidator : AbstractValidator<PostBenchmarkRequest>
{
    public PostBenchmarkRequestValidator()
    {
        RuleFor(x => x.PostBenchmarkRequestBody.FirstName).NotEmpty().WithMessage("name needed");
        RuleFor(x => x.PostBenchmarkRequestBody.LastName).NotEmpty().WithMessage("last needed");
        RuleFor(x => x.PostBenchmarkRequestBody.Age).GreaterThan(10).WithMessage("too young");
        RuleFor(x => x.PostBenchmarkRequestBody.PhoneNumbers).NotEmpty().WithMessage("phone needed");
        
    }
}