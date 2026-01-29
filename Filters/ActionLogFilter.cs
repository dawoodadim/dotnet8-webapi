using Microsoft.AspNetCore.Mvc.Filters;

namespace MiddlewareFilterDI.Filters
{
    public class ActionLogFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("Filter: Before action execution");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("Filter: After action execution");
        }

    }
}
