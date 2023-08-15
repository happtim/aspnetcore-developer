<Query Kind="Statements" />


string str = "hello world";

var strDecorator = new StringDecorator();
strDecorator.Reverse(str).Dump();

str.Reverse().Dump();
StringExtensions.Reverse(str).Dump();

public class StringDecorator
{
	public string Reverse(string input)
	{
		char[] chars = input.ToCharArray();
		Array.Reverse(chars);
		return new string(chars);
	}
}

public static class StringExtensions
{
	public static string Reverse(this string input)
	{
		char[] chars = input.ToCharArray();
		Array.Reverse(chars);
		return new string(chars);
	}
}