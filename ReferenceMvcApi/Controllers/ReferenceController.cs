using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ReferenceMvcApi.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
[Route("api")]
public class ReferenceController : ControllerBase
{
    [HttpPost("/exception")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]

    public async Task<IActionResult> Test(
        RequestDto request)
    {

        return Ok();
        // throw new NotImplementedException();
        //
        //
        // var validationProblemDetails = new ValidationProblemDetails();
    }
}

public class ResponseDto
{
    [MinLength(3)]
    public string Prop1 { get; set; }
}

public class RequestDto
{
    [MinLength(3)]
    [Required]
    public string Prop1 { get; set; }
}