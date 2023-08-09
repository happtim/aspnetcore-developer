<Query Kind="Statements">
  <NuGetReference Version="6.0.3">UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
  <Namespace>System.Activities.Statements</Namespace>
</Query>

var helloWorldActivity = new Sequence()
{
	Activities =
	{
		new WriteLine
		{
			Text = "Hello World!"
		}
	}
};

WorkflowInvoker.Invoke(helloWorldActivity);