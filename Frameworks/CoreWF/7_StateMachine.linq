<Query Kind="Statements">
  <NuGetReference>UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
  <Namespace>System.Activities.Statements</Namespace>
  <Namespace>System.Activities.Expressions</Namespace>
</Query>

#load ".\Activities"

Variable<string> input = new Variable<string>("input");

var on = new State
{
	DisplayName = "on",
	Entry = new WriteLine { Text = "Entry On" },
	Exit = new WriteLine { Text = "Exit On" },
};

var off = new State 
{
	DisplayName = "off",
	Entry = new WriteLine { Text = "Turn Off" },
	Exit = new WriteLine { Text = "Exit Off" },
};

on.Transitions.Add(new Transition
{
	To = off,
	Action = new WriteLine { Text = "Once executed, it will become Off." },
	Condition = new Equal<string,string,bool> 
	{
		Left = new InArgument<string>( context => input.Get(context)),
		Right = new InArgument<string>( " " ),
	},
	Trigger = new PressedSpace
	{
		Result = new OutArgument<string>( context =>  input.Get(context)),
	}
});

off.Transitions.Add(new Transition
{
	To = on,
	Action = new WriteLine { Text = "Once executed, it will become On." },
	Condition = new Equal<string, string, bool>
	{
		Left = new InArgument<string>(context => input.Get(context)),
		Right = new InArgument<string>(" "),
	},
	Trigger = new PressedSpace
	{
		Result = new OutArgument<string>(context => input.Get(context)),
	}
});

var workflow = new StateMachine 
{
	Variables = { input },
	InitialState = on,
	States =
	{
		on,
		off
	},
	
};

WorkflowInvoker.Invoke(workflow);

class PressedSpace: Activity<string>
{
	
	public PressedSpace()
	{
		Implementation = () => new Sequence
		{
			Activities =
			{
				new ReadLine
				{
					Result = new OutArgument<string>(context => this.Result.Get(context))
				},
				new WriteLine
				{
					Text = new InArgument<string>( context => "you input:'" + this.Result.Get(context) + "' , input ' ' change state")
				},

			}	
		};
	}
} 