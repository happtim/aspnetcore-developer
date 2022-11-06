<Query Kind="Program" />

void Main()
{
	//enum -> string
	Season.Spring.ToString().Dump();
	
	//string -> enum
	((Season)Enum.Parse(typeof(Season),"Summer")).Dump();
	
	//enum -> int
	((int)Season.Spring).Dump();

	//1 int -> enum
	Season season = (Season)2;
	season.Dump();
	
	Season notfound = (Season)9;
	notfound.Dump();

}

enum Season
{
	Spring,
	Summer,
	Autumn,
	Winter
}
