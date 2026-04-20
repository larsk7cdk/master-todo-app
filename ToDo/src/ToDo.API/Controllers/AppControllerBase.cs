using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace ToDo.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public abstract class AppControllerBase : ControllerBase
    {
    }
}
