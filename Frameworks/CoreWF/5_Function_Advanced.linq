<Query Kind="Statements">
  <NuGetReference Version="6.0.3">UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
  <Namespace>System.Activities.Statements</Namespace>
  <RemoveNamespace>System.Diagnostics</RemoveNamespace>
</Query>


#load ".\Activities"

var askDetails = new AskDetails();

WorkflowInvoker.Invoke(askDetails).Dump();

public class AskDetails : Activity<Person>
{
	public InArgument<string> NamePrompt { get; set; } = new("Please tell me your name:");
	public InArgument<string> AgePrompt { get; set; } = new("Please tell me your age:");

	Variable<string> _name = new Variable<string>
	{
		Name = "name"
	};

	public AskDetails()
	{
		Variable<int> _age = new("age");

		Implementation = () => new Sequence
		{
			Variables = { _name, _age },
			Activities =
			{
				new AskName
				{
					Prompt = new InArgument<string>( context => NamePrompt.Get(context)),
					Result = new OutArgument<string>( context => _name.Get(context)) 
				},
				new AskAge
				{
					Prompt =  new InArgument<string>( context =>  AgePrompt.Get(context)),
					Result = new OutArgument<int>( context => _age.Get(context)) 
				},
				new Assign<Person>
				{
					To = new OutArgument<Person> (context => this.Result.Get(context) ) ,
					Value = new InArgument<Person>(context => new Person( _name.Get(context),_age.Get(context) ) ) ,
				}
			},
		};
	}
}

public class AskName : Activity<string>
{
	public InArgument<string> Prompt { get; set; } = new("Please tell me your name:");

	public AskName()
	{
		Implementation = () => new Sequence
		{
			Activities =
			{
				new WriteLine { Text = new InArgument<string>(context => Prompt.Get(context))},
				new ReadLine
				{
					Result = new OutArgument<string>(contex =>  this.Result.Get(contex)),
				},
			}
		};
	}
}

public class AskAge : Activity<int>
{
	public InArgument<string> Prompt { get; set; } = new("Please tell me your age:");

	public AskAge()
	{
		Variable<string> _age = new ();
		
		Implementation = () => new Sequence
		{
			Variables = { _age },
			Activities =
			{
				new WriteLine { Text = new InArgument<string>(context => Prompt.Get(context))},
				new ReadLine
				{
					Result = new OutArgument<string>(contex => _age.Get(contex)),
				},
				new Assign<int>
				{
					To = new OutArgument<int>((env) => this.Result.Get(env)),
					Value = new InArgument<int>((env) => Convert.ToInt32( _age.Get(env)))
				}
			}
		};
	}
}


public record Person(string Name, int Age);