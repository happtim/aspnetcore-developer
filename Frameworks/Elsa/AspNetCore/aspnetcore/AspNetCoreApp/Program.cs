using Elsa.Extensions;
using Elsa.Http;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Contracts;

var builder = WebApplication.CreateBuilder();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddElsa(config => { 
    config.UseHttp();
    config.UseWorkflowRuntime(runtime => runtime.AddWorkflow<HelloWorldHttpWorkflow>());
});

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("run-workflow", async (IWorkflowRunner workflowRunner) => {
    await workflowRunner.RunAsync(new WriteLine("Hello ASP.NET world!"));
} );

app.MapGet("run-workflow2", async (IWorkflowRunner workflowRunner) =>
{
    await workflowRunner.RunAsync(new WriteHttpResponse
    {
        Content = new("Hello ASP.NET world!")
    });

});
app.UseWorkflows();
app.Run();


public class HelloWorldHttpWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("/run-workflow3"),
                    CanStartWorkflow = true
                },
                new WriteHttpResponse
                {
                    Content = new("Hello world of HTTP workflows!")
                }
            }
        };
    }
}