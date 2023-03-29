<Query Kind="Statements" />

int x = 5;
switch (x)
{
	case 5:
		break;
	case > 5:
	default:
		break;
}


int y = x switch 
{
	< -4 => -5,
	>= 10 and <= 20 => 15,
	5 => 5,
	6 => 6,
	_ => 0,
}


string str  = "123" switch
{
	var st when st.Length == 3 => "123",
	string { Length: >= 5 } s => s.Substring(0, 5),
	"123" => "123",
	"456" => "456",
	_ => "",
} ;
