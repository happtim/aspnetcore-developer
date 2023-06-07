<Query Kind="Statements">
  <NuGetReference>MediatR</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>MediatR</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var services = new ServiceCollection();

services.AddMediatR(cfg =>
{
   cfg.RegisterServicesFromAssemblies(typeof(Pinged).Assembly);
});

var provider = services.BuildServiceProvider();

var mediator = provider.GetRequiredService<IMediator>();


//2 Notifications 通知
mediator.Publish(new Pinged());


public class Pinged : INotification{}

public class PingedHandler : INotificationHandler<Pinged>
{
	public Task Handle(Pinged notification, CancellationToken cancellationToken)
	{
		"Got pinged async.".Dump();
		return Task.CompletedTask;
	}
}

public class PingedAlsoHandler : INotificationHandler<Pinged>
{
	public Task Handle(Pinged notification, CancellationToken cancellationToken)
	{
		"Got pinged also async.".Dump();
		return Task.CompletedTask;
	}
}

public class ConstrainedPingedHandler<TNotification> : INotificationHandler<TNotification>
 where TNotification : Pinged
{
	public Task Handle(TNotification notification, CancellationToken cancellationToken)
	{
		"Got pinged constrained async.".Dump();
		return Task.CompletedTask;
	}
}

public class GenericHandler : INotificationHandler<INotification>
{

	public Task Handle(INotification notification, CancellationToken cancellationToken)
	{
		"Got notified.".Dump();
		return Task.CompletedTask;
	}
}