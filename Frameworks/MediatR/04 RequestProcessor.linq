<Query Kind="Statements">
  <NuGetReference>MediatR</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>MediatR</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>MediatR.Pipeline</Namespace>
</Query>

//为了简化开发，内置了两种行为：
//RequestPreProcessorBehavior会在调用任何处理程序之前执行IRequestPreProcessor实现
//RequestPostProcessorBehavior会在所有处理程序调用后执行IRequestPostProcessor实现

var services = new ServiceCollection();

services.AddMediatR(cfg =>
{
	cfg.RegisterServicesFromAssemblies(typeof(Ping).Assembly);
});


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


public class GenericRequestPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
{

	public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
	{
		"- All Done".Dump();
		return Task.CompletedTask;
		
	}
}

public class ConstrainedRequestPostProcessor<TRequest, TResponse>
	: IRequestPostProcessor<TRequest, TResponse>
	where TRequest : Ping
{

	public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
	{
		"- All Done with Ping".Dump();
		return Task.CompletedTask;
	}
}

public class GenericRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
{

	public Task Process(TRequest request, CancellationToken cancellationToken)
	{
		"- Starting Up".Dump();
		return Task.CompletedTask;
	}
}