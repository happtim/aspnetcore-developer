<Query Kind="Statements">
  <NuGetReference Version="6.0.3">UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities.Statements</Namespace>
</Query>

#load ".\Activities"

var red = new State
{
	DisplayName = "Red",
	Entry = new WriteLine { Text = "Press Red" },
	Exit = new WriteLine{ Text = "Release Red"},
};

var blue = new State
{
	DisplayName = "Blue",
	Entry = new WriteLine { Text = "Press Blue" },
	Exit = new WriteLine{ Text = "Release Blue"},
};

var black = new State
{
	DisplayName = "Black",
	Entry = new WriteLine { Text = "Press Black" },
	Exit = new WriteLine{ Text = "Release Black"},
};

AddTransition(blue, "red" , red);
AddTransition(black, "red" , red);

AddTransition(red,  "blue", blue);
AddTransition(black, "blue", blue);

AddTransition(red, "black", black);
AddTransition(blue, "black", black);

var tricolorPen = new StateMachine
{
	InitialState = red,
	States =
	{
		red,
		blue,
		black
	},

};

var wfApp =  new WorkflowApplication(tricolorPen);

AutoResetEvent idleEvent = new AutoResetEvent(false);

wfApp.Idle = (e) => 
{
	if (idleEvent != null)
	{
		idleEvent.Set();
	}
};
// Start the workflow.  
wfApp.Run();

foreach (var press in new string[] { "blue","red","black"} )
{
	idleEvent.WaitOne(TimeSpan.FromSeconds(2));
	wfApp.ResumeBookmark(press,null);
}

void AddTransition(State currentState, string press, State nextState)
{
	Transition tx = new Transition
	{
		Trigger = new BookmarkActivity
		{
			BookmarkName = new InArgument<string>(press),
			Options = BookmarkOptions.None
		},
		To = nextState
	};
	currentState.Transitions.Add(tx);
}

public class BookmarkActivity : NativeActivity
{
	public InArgument<string> BookmarkName{ get; set; }

	public InArgument<BookmarkOptions> Options { get; set; }

	protected override void CacheMetadata(NativeActivityMetadata metadata)
	{
		metadata.AddArgument(new RuntimeArgument("BookmarkName", typeof(string), ArgumentDirection.In));
		metadata.AddArgument(new RuntimeArgument("Options", typeof(BookmarkOptions), ArgumentDirection.In));
	}

	protected override void Execute(NativeActivityContext context)
	{
		var bookmarkName = this.BookmarkName.Get(context);
		context.CreateBookmark(bookmarkName , new BookmarkCallback(BookmarkCallback), this.Options.Get(context));
	}

	protected override bool CanInduceIdle
	{
		get
		{
			return true;
		}
	}

	private void BookmarkCallback(NativeActivityContext context, Bookmark bookmark, object bookmarkData)
	{
		//Console.WriteLine("Bookmark {0} resumed", bookmark.Name);
		string dataString = bookmarkData as string;
		if (dataString != null)
		{
			if (string.Compare(dataString, "stop", true) == 0)
			{
				context.RemoveBookmark(bookmark.Name);
			}
		}
	}
}