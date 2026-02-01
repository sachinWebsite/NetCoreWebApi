namespace API.Filters
{
    using API.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ApiActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Nothing to do before execution
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Skip if exception occurred (middleware will handle it)
            if (context.Exception != null)
                return;

            // Handle ObjectResult (most common)
            if (context.Result is ObjectResult objectResult)
            {
                // Avoid double-wrapping
                if (objectResult.Value is ApiResponse<object>)
                    return;

                var apiResponse = ApiResponse<object>.Success(
                    objectResult.Value ?? new object(),
                    "Request successful"
                );

                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = objectResult.StatusCode ?? 200
                };
            }

            // Handle Empty results (e.g. NoContent)
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(
                    ApiResponse<object>.Success(new object()))
                {
                    StatusCode = 200
                };
            }
        }
    }

}
