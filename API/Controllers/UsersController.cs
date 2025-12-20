using Application.UseCases;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [ResponseCache(Duration = 300)] // Cache for 5 minutes
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserQuery { Id = id };
        var result = await _mediator.Send(query);
        if (result.IsSuccess)
            return Ok(result.Data);
        return NotFound(new { error = result.Error });
    }
}