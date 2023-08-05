<Query Kind="Statements">
  <NuGetReference>UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
  <Namespace>System.Activities.Statements</Namespace>
</Query>

Variable<string> name = new Variable<string>();

var workflow = new Sequence
{
	Variables = { name },
	Activities =
	{
		new WriteLine()
		{
			Text = "What is your name?"
		},
		new ReadLine()  
        {  
            BookmarkName = "UserName",  
            Result = name
		},
		new WriteLine()
		{
			Text = new InArgument<string>((env) => "Hello, " + name.Get(env))
		}
	}
};

AutoResetEvent syncEvent = new AutoResetEvent(false);

// Create the WorkflowApplication using the desired  
// workflow definition.  
WorkflowApplication wfApp = new WorkflowApplication(workflow);

// Handle the desired lifecycle events.  
wfApp.Completed = delegate (WorkflowApplicationCompletedEventArgs e)
{
	// Signal the host that the workflow is complete.  
	syncEvent.Set();
};

// Start the workflow.  
wfApp.Run();

// Collect the user's name and resume the bookmark.  
// Bookmark resumption only occurs when the workflow  
// is idle. If a call to ResumeBookmark is made and the workflow  
// is not idle, ResumeBookmark blocks until the workflow becomes  
// idle before resuming the bookmark.  
wfApp.ResumeBookmark("UserName", Console.ReadLine());

// Wait for Completed to arrive and signal that  
// the workflow is complete.  
syncEvent.WaitOne();

public class ReadLine : NativeActivity<string>
{
	public InArgument<string> BookmarkName { get; set; }
	
	protected override void Execute(NativeActivityContext context)
	{
		context.CreateBookmark(BookmarkName.Get(context), OnResumeBookmark);
	}

	private void OnResumeBookmark(NativeActivityContext context, Bookmark bookmark, object value)
	{
		string userInput = (string)value;
		context.SetValue(Result, userInput);
	}

	protected override bool CanInduceIdle => true;
}