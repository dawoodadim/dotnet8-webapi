using Microsoft.AspNetCore.Mvc;
using MiddlewareFilterDI.Filters;
using MiddlewareFilterDI.Services;

namespace MiddlewareFilterDI.Controllers
{
    [ApiController]
    [Route("api/demo")]
    [ServiceFilter(typeof(ActionLogFilter))]
    public class DomeController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public DomeController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(_messageService.GetMessage());
        }
    }
}
