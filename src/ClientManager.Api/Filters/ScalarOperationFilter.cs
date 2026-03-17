using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ClientManager.Api.Filters
{
    public class ScalarOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var path = context.ApiDescription.RelativePath;

            if (string.IsNullOrEmpty(operation.Description) || operation.Description == operation.Summary)
            {
                operation.Description = operation.Summary;
            }

            operation.Summary = $"/{path}";
        }
    }
}
