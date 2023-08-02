using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using Eurowag.Common.AspNetCore.MinimalEndpoints;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace MinimalApiReprV2.Features.PrimitiveObsession;

public class GetVehicleEndpoint : MinimalEndpoint
    .WithRequest<GetVehicleRequest>
    .WithResponse<GetVehicleResponse>
{
    public override void ConfigureEndpoint(MinimalEndpointRouteHandlerBuilder builder)
    {
        builder
            .MapPost("/vehicles")
            .MapToApiVersion(1)
            .Produces<GetVehicleResponse>();
    }

    protected override async Task<IResult> HandleAsync(GetVehicleRequest request, CancellationToken cancellationToken)
    {
        var response = new GetVehicleResponse()
        {
            VehicleId = request.RequestBody.VehicleId
        };

        return TypedResults.Ok(response);
    }
}

public class GetVehicleResponse
{
    public required VehicleId VehicleId { get; init; }
}

public class GetVehicleRequest
{
    // [FromRoute] public VehicleId VehicleId { get; init; } = null!;
    
    [FromBody] public GetVehiclesRequestBody RequestBody { get; set; } = null!;
}

public class GetVehiclesRequestBody
{
    public VehicleId? VehicleId { get; set; }
}


[TypeConverter(typeof(VehicleIdTypeConverter))]
[JsonConverter(typeof(VehicleIdJsonConverter))]
public class VehicleId : ValueObject
{
    private VehicleId(string value)
    {
        Value = value;
    }

    public static VehicleId Create(string value)
    {
        return new VehicleId(value);
    }

    public string Value { get; }
    
    public override string ToString() => Value;

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }

    public static bool TryParse(string? input, out VehicleId? output)
    {
        output = null;

        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        output = new VehicleId(input);

        return true;
    }

    private sealed class VehicleIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue)
            {
                return new VehicleId(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertTo(
            ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value is VehicleId idValue && destinationType == typeof(string))
            {
                return idValue.Value;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    
    private sealed class VehicleIdJsonConverter : JsonConverter<VehicleId>
    {
        public override VehicleId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();

            return stringValue == null 
                ? default : new VehicleId(stringValue);
        }

        public override void Write(Utf8JsonWriter writer, VehicleId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}


