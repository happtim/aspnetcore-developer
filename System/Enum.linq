<Query Kind="Statements" />


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

//类中的枚举类型 默认未0
Direction dir = new Direction();
dir.DirectionEnum.Dump("Default");


enum Season
{
	Spring,
	Summer,
	Autumn,
	Winter
}

enum DirectionEnum
{
	East = -1,
	West = 1,
	South,
	North
}


class Direction
{
	public DirectionEnum DirectionEnum {get;set;}
}