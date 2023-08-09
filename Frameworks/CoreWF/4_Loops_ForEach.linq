<Query Kind="Statements">
  <NuGetReference Version="6.0.3">UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
  <Namespace>System.Activities.Statements</Namespace>
  <Namespace>System.Activities.Expressions</Namespace>
  <RemoveNamespace>System.Diagnostics</RemoveNamespace>
</Query>


var shoppingList = new List<string> { "Apples", "Bananas", "Potatoes", "Coffee", "Honey", "Rice" };
DelegateInArgument<string> current = new DelegateInArgument<string>();
var @foreach = new Sequence
{
	Activities = 
	{
		new WriteLine { Text = "Going through the shopping list..."},
		new ForEach<string>
		{
			Values = new LambdaValue<IEnumerable<string>>(c => shoppingList),
			Body = new ActivityAction<string>
			{
				Argument = current,
				Handler = new WriteLine {  Text = new InArgument<string>( context => $"- [ ] {current.Get(context)}")},
			},

		},
		new WriteLine { Text = "Let's not forget anything!\n"},
	}
};

// Run the workflow.
WorkflowInvoker.Invoke(@foreach);