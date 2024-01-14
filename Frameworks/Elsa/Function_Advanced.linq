<Query Kind="Statements">
  <NuGetReference Version="3.0.0">Elsa</NuGetReference>
  <Namespace>Elsa.Expressions.Models</Namespace>
  <Namespace>Elsa.Extensions</Namespace>
  <Namespace>Elsa.Workflows</Namespace>
  <Namespace>Elsa.Workflows.Activities</Namespace>
  <Namespace>Elsa.Workflows.Contracts</Namespace>
  <Namespace>Elsa.Workflows.Memory</Namespace>
  <Namespace>Elsa.Workflows.Models</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();

// Resolve a workflow runner to run the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();


// Declare a workflow variable for use in the workflow.
var personVariable = new Variable<Person>();

// Declare a workflow.
var workflow = new Sequence
{
	Variables = { personVariable },
	Activities =
	{
		new AskDetails(new(personVariable))
		{
			NamePrompt = new ("What's your name?"),
			AgePrompt = new ("What's your age?")
		},
		new WriteLine(context =>
		{
			var person = personVariable.Get(context)!;
			return $"Your name is {person.Name} and you are {person.Age} years old.";
		})
	}
};

// Run the workflow.
await workflowRunner.RunAsync(workflow);


public class AskDetails : Composite<Person>
{
	private readonly Variable<string> _name = new();
	private readonly Variable<int> _age = new();

	public Input<string> NamePrompt { get; set; } = new("Please tell me your name:");
	public Input<string> AgePrompt { get; set; } = new("Please tell me your age:");

	public AskDetails(Output<Person> person)
	{
		Result = person;
		Variables = new List<Variable> { _name, _age };
		Root = new Sequence
		{
			//作用域问题，在OnCompleted的时候拿不到Root里面声明的变量。
			//Variables = new List<Variable> { _name, _age },
			Activities =
			{
				new AskName
				{
					Prompt = new (context => NamePrompt.Get(context)),
					Result = new (_name)
				},
				new AskAge
				{
					Prompt = new (context => AgePrompt.Get(context)),
					Result = new (_age)
				},
				////替换OnCompleted的方式返回返回值。
				//new SetResult<Person>
				//{
				//	Result = Result,
				//	Value = new Input<Person>(context => new Person(_name.Get<string>(context),_age.Get<int>(context)))
				//}
			}
		};
	}

	protected override void OnCompleted(ActivityCompletedContext context)
	{
		//var name = _name.Get<string>(childContext)!;
		//var age = _age.Get<int>(childContext);

		var name = _name.Get<string>(context.TargetContext)!;
		var age = _age.Get<int>(context.TargetContext);
		var person = new Person(name, age);

		context.TargetContext.Set(Result, person);
	}
}

public class AskAge : Composite<int>
{
	private readonly Variable<string> _age = new();

	public Input<string> Prompt { get; set; } = new("Please tell me your age:");

	public AskAge()
	{
		Variables = new List<Variable> { _age };
		Root = new Sequence
		{
			Activities =
			{
				new WriteLine(context => Prompt.Get(context)),
				new ReadLine(_age)
			}
		};
	}


	protected override void OnCompleted(ActivityCompletedContext context)
	{
		var age = _age.Get<int>(context.TargetContext);
		context.TargetContext.Set(Result, age);
	}
}

public class AskName : Composite<string>
{
	private readonly Variable<string> _name = new();

	public Input<string> Prompt { get; set; } = new("Please tell me your name:");

	public AskName()
	{
		Variables = new List<Variable> { _name };
		Root = new Sequence
		{
			Activities = new List<IActivity>
			{
				new WriteLine(context => Prompt.Get(context)),
				new ReadLine(_name)
			}
		};
	}

	protected override void OnCompleted(ActivityCompletedContext context)
	{
		var name = _name.Get<string>(context.TargetContext);
		context.TargetContext.Set(Result, name);
	}
}

public class SetResult<T> : CodeActivity<T>
{
	/// <summary>
	/// The value to assign.
	/// </summary>
	[Elsa.Workflows.Attributes.Input(Description = "The value to assign.")]
	public Input<T> Value { get; set; } = new(new Literal<T>());

	protected override void Execute(ActivityExecutionContext context)
	{
		var value = context.Get<T>(Value);
		context.Set(Result, value);
	}
}

public record Person(string Name, int Age);