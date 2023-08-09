<Query Kind="Statements">
  <NuGetReference Version="6.0.3">UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
  <Namespace>System.Activities.Statements</Namespace>
  <Namespace>System.Activities.Expressions</Namespace>
</Query>

var counterVariable = new Variable<int>("counter",1);

var @while = new Sequence
{
	Variables = { counterVariable },
	Activities =
	{
		new WriteLine{ Text = "Counting to 10:" },
        
        // Loop while counter < 100.
        new While(context => counterVariable.Get(context) <= 10)
		{
			Body = new Sequence
			{
				Activities =
				{
					new WriteLine { Text = new InArgument<string>(context => $"Current value: {counterVariable.Get(context)}")},
                    
                    // Increment counter variable.
                    new Assign<int>
					{
						To = new OutArgument<int>(context => counterVariable.Get(context)),
						Value = new InArgument<int>( context => counterVariable.Get(context) + 1),
					}
				}
			},
		},
		new WriteLine{ Text = "That was really fast!"}
	}
};

WorkflowInvoker.Invoke(@while);