<Query Kind="Statements">
  <NuGetReference>UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities.Statements</Namespace>
  <Namespace>System.Activities</Namespace>
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