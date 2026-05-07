using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ToDo.API.DTO;
using ToDo.API.Mappers;
using ToDo.Application;
using ToDo.Application.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.API.Controllers;

public class ToDoController : AppControllerBase
{
    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> CreateAsync(
        [FromServices] IValidator<ToDoCreateRequest> validator,
        [FromBody] ToDoCreateRequest request,
        [FromKeyedServices(KeyedServices.ToDoCreateRequestServiceKey)]
        IRequestHandler<ToDoModel, int> requestService,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var model = request.ToModel();

        var result = await requestService.InvokeAsync(model, cancellationToken);

        return Created($"/todo/id", new { id = result });
    }

    [HttpPut]
    [ProducesResponseType(200)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] IValidator<ToDoUpdateRequest> validator,
        [FromBody] ToDoUpdateRequest request,
        [FromKeyedServices(KeyedServices.ToDoUpdateRequestServiceKey)]
        IRequestHandler<ToDoModel, int> requestService,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var model = request.ToModel();

        var result = await requestService.InvokeAsync(model, cancellationToken);

        return Ok(new { id = result });
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] IValidator<ToDoDeleteRequest> validator,
        [FromRoute] int id,
        [FromKeyedServices(KeyedServices.ToDoDeleteRequestServiceKey)]
        IRequestHandler<int> requestService,
        CancellationToken cancellationToken)
    {
        await requestService.InvokeAsync(id, cancellationToken);

        return Ok();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ToDoResponse), 200)]
    public async Task<IActionResult> GetDetailsAsync(
        [FromServices] IValidator<ToDoReadDetailsRequest> validator,
        [FromRoute] int id,
        [FromKeyedServices(KeyedServices.ToDoReadDetailsRequestServiceKey)]
        IRequestHandler<int, ToDoModel> requestService,
        CancellationToken cancellationToken)
    {
        var result = await requestService.InvokeAsync(id, cancellationToken);

        var response = result.ToResponse();

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ToDoResponse>), 200)]
    public async Task<IActionResult> GetAllAsync(
        [FromKeyedServices(KeyedServices.ToDoReadAllRequestServiceKey)]
        IQueryHandler<IList<ToDoModel>> requestService,
        CancellationToken cancellationToken)
    {
        var result = await requestService.InvokeAsync(cancellationToken);

        return Ok(result);
    }
}
