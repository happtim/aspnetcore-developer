<Query Kind="Statements">
  <NuGetReference Version="12.0.1">MediatR</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>MediatR</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>MediatR.Pipeline</Namespace>
</Query>

//pipeline behaviors 只支持 IRequestHandler<TRequest,TResponse> 不支持 INotificationHandler<TRequest>
var services = new ServiceCollection();

services.AddMediatR(cfg =>
{
	
	cfg.RegisterServicesFromAssemblies(typeof(Ping).Assembly);
	//一种注册方式
	//cfg.AddOpenBehavior(typeof(GenericPipelineBehavior<,>));
	//cfg.AddBehavior<IPipelineBehavior<Ping, string>, LoggingBehavior>();
});

//第二种注册方式
//services.AddScoped(typeof(IPipelineBehavior<,>), typeof(GenericPipelineBehavior<,>));
//services.AddScoped(typeof(IPipelineBehavior<Ping, string>), typeof(LoggingBehavior));

var provider = services.BuildServiceProvider();

var mediator = provider.GetRequiredService<IMediator>();

mediator.Send(new Ping()).Dump();


public class Ping : IRequest<string> { }

public class PingHandler : IRequestHandler<Ping, string>
{
	public Task<string> Handle(Ping request, CancellationToken cancellationToken)
	{
		return Task.FromResult("Pong");
	}
}

public class LoggingBehavior : IPipelineBehavior<Ping, string>
{
	public async Task<string> Handle(Ping request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
	{
		$"Handling {typeof(Ping).Name}".Dump();
		var response = await next();
		$"Handled {typeof(string).Name}".Dump();

		return response;
	}
}

public class GenericPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		"-- Generic Handling Request".Dump();
		var response = await next();
		"-- Generic Finished Request".Dump();
		return response;
	}
}
