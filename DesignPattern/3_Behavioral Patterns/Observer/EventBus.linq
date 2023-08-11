<Query Kind="Statements">
  <NuGetReference>Autofac</NuGetReference>
  <Namespace>Autofac</Namespace>
</Query>

using System;

// Create the builder with which components/services are registered.
var builder = new ContainerBuilder();

builder.RegisterType<EventPublisher>().As<IEventPublisher>().InstancePerLifetimeScope();

//builder.RegisterType( typeof(PersonEntityAddConsumer))
//.As(typeof(IConsumer<Person>))
//.InstancePerLifetimeScope();

builder.RegisterType(typeof(PersonEntityAddConsumer))
.As(typeof(PersonEntityAddConsumer)
.FindInterfaces((type, criteria) =>
{
	var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
	return isMatch;
}, typeof(IConsumer<>)))
.InstancePerLifetimeScope();

var container = builder.Build();


using (var scope = container.BeginLifetimeScope())
{
	var publisher = scope.Resolve<IEventPublisher>();
	publisher.Publish(new Person() {Name = "Tim"});
	
}


class PersonEntityAddConsumer : IConsumer<Person>
{
	public void HandleEvent(Person eventMessage)
	{
		Console.WriteLine("person insert db");
	}
}

public class Person
{
	public string Name {get;set;}	
}


public partial class EventPublisher : IEventPublisher
{
	private readonly ILifetimeScope _lifetimeScope;

	public EventPublisher(ILifetimeScope lifetimeScope)
	{
		_lifetimeScope = lifetimeScope;
	}

	public virtual void Publish<TEvent>(TEvent @event)
	{
		//get all event consumers
		try
		{
			var consumers = _lifetimeScope.Resolve<IEnumerable<IConsumer<TEvent>>>();
			foreach (var consumer in consumers)
			{
				try
				{
					//try to handle published event
					consumer.HandleEvent(@event);
				}
				catch (Exception exception)
				{
					//log error, we put in to nested try-catch to prevent possible cyclic (if some error occurs)
					try
					{
						exception.Dump();
					}
					catch
					{
						// ignored
					}
				}
			}
		}
		catch
		{
			// ignored
		}

	}
}

public interface IConsumer<T>
{
	void HandleEvent(T eventMessage);
}

public partial interface IEventPublisher
{
	void Publish<TEvent>(TEvent @event);
}