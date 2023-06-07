<Query Kind="Statements">
  <NuGetReference>MediatR</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>MediatR</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//https://www.mdnice.com/writing/0e53512ae968473ab47be6e09029a24b
var services = new ServiceCollection();

services.AddMediatR(cfg =>
{
   cfg.RegisterServicesFromAssemblies(typeof(Ping).Assembly);
});

var provider = services.BuildServiceProvider();

var mediator = provider.GetRequiredService<IMediator>();


//1 Request/response 模式
var response = mediator.Send(new Ping());
response.Dump();

//1.1 无参是返回
var notReturn = mediator.Send(new OneWay());
notReturn.Dump();

//IRequest<T> 返回参数T
public class Ping : IRequest<string> { }

public class PingHandler : IRequestHandler<Ping, string>
{
	public Task<string> Handle(Ping request, CancellationToken cancellationToken)
	{
		return Task.FromResult("Pong");
	}
}

//IRequest 没有参数返回
public class OneWay : IRequest { }
public class OneWayHandler : IRequestHandler<OneWay>
{
	public Task Handle(OneWay request, CancellationToken cancellationToken)
	{
		// do work
		return Task.CompletedTask;
	}
}
