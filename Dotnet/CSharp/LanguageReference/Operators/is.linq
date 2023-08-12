<Query Kind="Statements" />


var person = new Person(){ Name =  "Tim"};

if(person is Person p)
{
	p.Dump();
}

person = null;

if (person is Person pp)
{
	pp.Dump();
}

public class Person 
{
	public string Name {get;set;}
}