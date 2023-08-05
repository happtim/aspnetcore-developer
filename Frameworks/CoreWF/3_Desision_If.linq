<Query Kind="Statements">
  <NuGetReference>UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities.Statements</Namespace>
  <Namespace>System.Activities</Namespace>
</Query>

#load ".\Activities"

var ageVariable = new Variable<string>();

// Declare a workflow.
var ifelse = new Sequence
{
	// Register the variable.
	Variables = { ageVariable },

	// Setup the sequence of activities to run.
	Activities =
	{
		new WriteLine
		{
			Text = "Please tell me your age:",
		},
		new ReadLine
		{
			Result = new OutArgument<string>(context => ageVariable.Get(context)),
		}, 
		// Stores user input into the provided variable.,
        new If
		{
            // If aged 18 or up, beer is provided, soda otherwise.
            Condition = new (context => Convert.ToInt32(ageVariable.Get(context)) < 18),
			Then = new Sequence
			{
				Activities =
				{
					new WriteLine
					{
						Text = new InArgument<string>( context => "Enjoy your soda!" + ageVariable.Get(context) ),
					},
				}
			},

			Else = new WriteLine
			{
				Text = "Enjoy your beer!",
			}
		},
		new WriteLine
		{
			Text = "Come again!",
		}
	}
};

WorkflowInvoker.Invoke(ifelse);

