<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


var pipeline = new Pipeline();
pipeline.UseMiddleware<LoggingMiddleware>();
pipeline.UseMiddleware<AuthenticationMiddleware>();
pipeline.UseMiddleware<RoutingMiddleware>();

var context = new HttpContext();
pipeline.Run(context).GetAwaiter().GetResult();

public class Pipeline
{
	private readonly IList<Func<RequestDelegate, RequestDelegate>> _middlewares;

	public Pipeline()
	{
		_middlewares = new List<Func<RequestDelegate, RequestDelegate>>();
	}

	public RequestDelegate Build()
	{
		RequestDelegate app = context => Task.CompletedTask;

		//组装pipeline
		
		foreach (var middle in _middlewares.Reverse())
		{
			app = middle(app);
		}

		//LoggingMiddleware
			//AuthenticationMiddleware
				//RoutingMiddleware
					//context => Task.CompletedTask;
		return app;
	}

	public void UseMiddleware<TMiddleware>() where TMiddleware : IMiddleware
	{
		_middlewares.Add(next => context =>
		{
			var middleware = Activator.CreateInstance<TMiddleware>();
			return middleware.Invoke(context, next);
		});
	}
	
	public Pipeline Use(Func<RequestDelegate, RequestDelegate> middleware)
	{
		_middlewares.Add(middleware);
		return this;
	}

	public async Task Run(HttpContext context)
	{
		var app = Build();
		await app(context);
	}
}

//public static class PipelineExtensions 
//{
//	public static Pipeline UseMiddleware<TMiddleware>(this Pipeline pipeline) where TMiddleware : IMiddleware 
//	{
//		return pipeline.Use(next => context =>
//		{
//			var middleware = Activator.CreateInstance<TMiddleware>();
//			return middleware.Invoke(context, next);
//		});
//	}
//}

public interface IMiddleware
{
	Task Invoke(HttpContext context, RequestDelegate next);
}

public class LoggingMiddleware : IMiddleware
{
	public async Task Invoke(HttpContext context, RequestDelegate next)
	{
		Console.WriteLine("Logging Middleware: Request received");

		await next(context);

		Console.WriteLine("Logging Middleware: Response sent");
	}
}

public class AuthenticationMiddleware : IMiddleware
{
	public async Task Invoke(HttpContext context, RequestDelegate next)
	{
		Console.WriteLine("Authentication Middleware: Request authenticated");

		await next(context);

		Console.WriteLine("Authentication Middleware: Response authenticated");
	}
}

public class RoutingMiddleware : IMiddleware
{
	public async Task Invoke(HttpContext context, RequestDelegate next)
	{
		Console.WriteLine("Routing Middleware: Request routed");

		await next(context);

		Console.WriteLine("Routing Middleware: Response routed");
	}
}

public class HttpContext
{
	public string Response { get; set; }
}

public delegate Task RequestDelegate(HttpContext context);