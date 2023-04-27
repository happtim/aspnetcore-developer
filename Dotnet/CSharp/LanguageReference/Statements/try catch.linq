<Query Kind="Statements" />


try
{
	try
	{
		throw new Exception("异常");
	}
	finally
	{
		"finally".Dump();
	}
}
catch (Exception ex) 
{
	"我接到了异常".Dump();
}

