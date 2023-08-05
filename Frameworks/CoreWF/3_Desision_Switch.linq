<Query Kind="Statements">
  <NuGetReference>UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
  <Namespace>System.Activities.Statements</Namespace>
  <Namespace>System.Activities.Expressions</Namespace>
  <RemoveNamespace>System.Diagnostics</RemoveNamespace>
</Query>

#load ".\Activities"

var ageVariable = new Variable<string>();

var switchcase = new Sequence
{
	Variables = { ageVariable },

	Activities =
	{
		new WriteLine
		{
			Text = "Please tell me your age:",
		},
		// Stores user input into the provided variable.,
		new ReadLine
		{
			Result = new OutArgument<string>(context => ageVariable.Get(context)),
		},
		new Switch<string>
		{
			Expression = new InArgument<string>(context => ageVariable.Get(context)),
			//Expression = ExpressionServices.Convert<string>( context => ageVariable.Get(context) ),
			Cases =
			{
				{"19", new WriteLine { Text = "Enjoy your beer!"} },
				{"18", new WriteLine { Text = "Enjoy your beer!"} },
				{"17", new WriteLine { Text = "Enjoy your soda!"} }
			},
			Default = new WriteLine
			{
				Text = "Enjoy your soda!",
			}
		}
	}
};

// Run the workflow.
WorkflowInvoker.Invoke(switchcase);
