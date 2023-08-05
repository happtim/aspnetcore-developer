<Query Kind="Statements">
  <NuGetReference>UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities.Statements</Namespace>
  <Namespace>System.Activities</Namespace>
</Query>



var variableInt = new Variable<int>("int");

var workflow = new Sequence()
{
	Variables = { variableInt },
	Activities =
	{
		new WriteLine
		{
			Text = new InArgument<string>( context => $"Current value: { variableInt.Get(context)}")
		},
		new Assign<int>
		{
			To = new OutArgument<int>( context => variableInt.Get(context)),
			Value = new InArgument<int>( context => variableInt.Get(context) + 1),
		},
		new WriteLine
		{
			Text = new InArgument<string>( context => $"Current value: { variableInt.Get(context)}")
		}
	}
};

WorkflowInvoker.Invoke(workflow);
