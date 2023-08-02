using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinimalApiRepr;
using MinimalApiRepr.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Services
    .AddAuthorization()
    .AddSingleton<IValidator<Request>, Validator>();

builder.Services.AddScoped<PostEndpoint>();
//builder.Services.AddScoped<GetRequest>();

// var services = builder.Services;
//
// services
//     .AddApiVersioning(options =>
//     {
//         options.ReportApiVersions = true;
//     })
//     .AddApiExplorer(options =>
//     {
//         options.GroupNameFormat = "'v'V";
//         options.SubstituteApiVersionInUrl = true;
//     });
//
// services.AddEndpointsApiExplorer();
//
// // add swagger
// services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
// services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
//


var app = builder.Build();
app.UseAuthorization();

// app.MapPost("/benchmark/ok/{id}", async (
//         [FromRoute] int id,
//         [FromBody] Request req,
//         [FromServices] ILogger<Program> logger,
//         [FromServices] IValidator<Request> validator) =>
//     {
//         // logger.LogInformation("request received!");
//
//         await validator.ValidateAsync(req);
//
//         return Results.Ok(new Response()
//         {
//             Id = id,
//             Name = req.FirstName + " " + req.LastName,
//             Age = req.Age,
//             PhoneNumber = req.PhoneNumbers?.FirstOrDefault()
//         });
//     })
//     .RequireAuthorization()
//     .AllowAnonymous();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(options =>
//     {
//         var descriptions = app.DescribeApiVersions();
//
//         // build a swagger endpoint for each discovered API version
//         foreach (var description in descriptions.Reverse())
//         {
//             var url = $"/swagger/{description.GroupName}/swagger.json";
//             var name = description.GroupName.ToUpperInvariant();
//             options.SwaggerEndpoint(url, name);
//         }
//     });
// }

app.MapEndpoints();

app.Run();

namespace MinimalApiRepr
{
    public partial class Program { }

    public class Request
    {
        [FromRoute] public int Id { get; set; }
        [FromBody] public RequestBody RequestBody { get; set; }
    }

    public class RequestBody
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public IEnumerable<string>? PhoneNumbers { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.RequestBody.FirstName).NotEmpty().WithMessage("name needed");
            RuleFor(x => x.RequestBody.LastName).NotEmpty().WithMessage("last needed");
            RuleFor(x => x.RequestBody.Age).GreaterThan(10).WithMessage("too young");
            RuleFor(x => x.RequestBody.PhoneNumbers).NotEmpty().WithMessage("phone needed");
        }
    }

    public class Response
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? PhoneNumber { get; set; }
    }

    [ApiEndpoint("/v1/benchmark/ok/{id}", "POST")]
    public class PostEndpoint : EndpointBase<Request>
    {
        private readonly IValidator<Request> _validator;
        // public async Task<IResult> HandleAsync(
        //     int id,
        //     Request req,
        //     IValidator<Request> validator)
        // {
        //     // logger.LogInformation("request received!");
        //
        //     await validator.ValidateAsync(req);
        //
        //     return Results.Ok(new Response()
        //     {
        //         Id = id,
        //         Name = req.FirstName + " " + req.LastName,
        //         Age = req.Age,
        //         PhoneNumber = req.PhoneNumbers?.FirstOrDefault()
        //     });
        // }

        public PostEndpoint(IValidator<Request> validator)
        {
            _validator = validator;
        }

        public override async Task<IResult> HandleAsync(Request req)
        {
            await _validator.ValidateAsync(req);
            
            return Results.Ok(new Response()
            {
                Id = req.Id,
                Name = req.RequestBody.FirstName + " " + req.RequestBody.LastName,
                Age = req.RequestBody.Age,
                PhoneNumber = req.RequestBody.PhoneNumbers?.FirstOrDefault()
            });
        }
    }

    // [ApiEndpoint("/v1/benchmark/ok/{id}", "GET")]
    // public class GetEndpoint : EndpointBase<GetRequest>
    // {
    //     public GetEndpoint()
    //     {
    //     }
    //
    //     public override async Task<IResult> HandleAsync(GetRequest request)
    //     {
    //         return Results.Ok(new GetResponse()
    //         {
    //             Greeting = $"Hello World id: {request.Id}"
    //         });
    //     }
    // }

    public class GetRequest
    {
        [FromRoute] public int Id { get; set; }
    }

    public class GetResponse
    {
        public string Greeting { get; set; } = "Hello World!";
    }

    public abstract class EndpointBase<TRequest>
    {
        public HttpContext HttpContext { get; set; }
        public abstract Task<IResult> HandleAsync(TRequest request);
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiEndpointAttribute : Attribute
    {
        public string Path { get; }
        public string HttpMethod { get; }

        public ApiEndpointAttribute(string path, string httpMethod)
        {
            Path = path;
            HttpMethod = httpMethod;
        }
    }
}

public static class WebApplicationExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        var type = typeof(WebApplicationExtensions);
        var methodInfo = type.GetMethod(nameof(HandleRequest));
        
        var apiEndpoints = Assembly.GetExecutingAssembly().GetTypes()
         .Where(type => type.IsDefined(typeof(ApiEndpointAttribute)));

       foreach (var apiEndpointType in apiEndpoints)
       {
           var apiEndpointAttribute = apiEndpointType.GetCustomAttribute<ApiEndpointAttribute>();

           var path = apiEndpointAttribute.Path;
           var method = apiEndpointAttribute.HttpMethod;
           var apiEndpointBaseType = apiEndpointType.BaseType;
           var requestType = apiEndpointBaseType.GetGenericArguments().FirstOrDefault();
           
           
           var genericMethodInfoForApiEndpoint = methodInfo.MakeGenericMethod(apiEndpointType, requestType);


           var delegateTypeForApiEndpointHandler = Expression.GetDelegateType(
               requestType, 
               apiEndpointType,
               typeof(Task<IResult>));

           var apiEndpointHandlerDelegate = 
               Delegate.CreateDelegate(delegateTypeForApiEndpointHandler, genericMethodInfoForApiEndpoint);

           app.MapMethods(path, new[] { method }, apiEndpointHandlerDelegate)
               .RequireAuthorization()
               .AllowAnonymous();
       }

    }
    public static Task<IResult> HandleRequest<TEndpoint, TRequest>(
        [AsParameters] TRequest req,
        [FromServices] TEndpoint endpoint) where TEndpoint : EndpointBase<TRequest>
    {
        return endpoint.HandleAsync(req);
    }
}