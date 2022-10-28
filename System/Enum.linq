<Query Kind="Program" />

void Main()
{
	Season season = (Season)9;
	season.Dump();
	(season == Season.Spring).Dump();
	
}

enum Season
{
	Spring,
	Summer,
	Autumn,
	Winter
}
