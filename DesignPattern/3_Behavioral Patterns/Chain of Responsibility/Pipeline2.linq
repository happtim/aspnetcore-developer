<Query Kind="Statements" />


var pipeline = new PipelineExecution();

pipeline.Execute("hello world").Dump();

public class PipelineExecution
{
	private MiddlewareDelegate _pipeline;
	
	public PipelineExecution()
	{
		_pipeline = CreateDefaultPipeline();
	}
	
	public string Execute(string input) => _pipeline(input);

	private MiddlewareDelegate CreateDefaultPipeline() => Setup(x =>
		x.UseMiddleware<UpperCaseMiddleware>()
		.UseMiddleware<ReverseMiddleware>()
   );

	private MiddlewareDelegate Setup(Action<PipelineBuilder> setup)
	{
		var builder = new PipelineBuilder();
		setup(builder);
		_pipeline = builder.Build();
		return _pipeline;
	}
}

public class PipelineBuilder
{
	private readonly IList<Func<MiddlewareDelegate, MiddlewareDelegate>> _middlewares;

	public PipelineBuilder()
	{
		_middlewares = new List<Func<MiddlewareDelegate, MiddlewareDelegate>>();
	}

	public MiddlewareDelegate Build()
	{
		MiddlewareDelegate app = str => str;

		//组装pipeline
		foreach (var middle in _middlewares.Reverse())
		{
			app = middle(app);
		}

		return app;
	}

	public PipelineBuilder Use(Func<MiddlewareDelegate, MiddlewareDelegate> middleware)
	{
		_middlewares.Add(middleware);
		return this;
	}

	public string Run(string input)
	{
		var app = Build();
		return app(input);
	}
}

public static class PipelineExtensions 
{
	public static PipelineBuilder UseMiddleware<TMiddleware>(this PipelineBuilder pipeline) where TMiddleware : IMiddleware 
	{
		return pipeline.Use(next => str =>
		{
			var middleware = Activator.CreateInstance<TMiddleware>();
			return middleware.Invoke(str, next);
		});
	}
}

public class UpperCaseMiddleware : IMiddleware
{
	public string Invoke(string input, MiddlewareDelegate next)
	{
		return next(input.ToUpper()); 
	}
}

public class ReverseMiddleware : IMiddleware
{
	public string Invoke(string input, MiddlewareDelegate next)
	{
		return next(new string(input.Reverse().ToArray())); 
	}
}

public interface IMiddleware
{
	string Invoke(string input, MiddlewareDelegate next);
}

public delegate string  MiddlewareDelegate(string input);