<Query Kind="Statements" />

Power12V powner = new Adapter();
powner.GetPower().Dump();

public class Adapter : Power12V
{

	private Power220V power220V = new Power220V();

	public override int GetPower()
	{

		int power = power220V.GetPower();
		return power - 208; //降压
	}
}
public class Power220V
{

	public int GetPower()
	{
		return 220;
	}
}

public abstract class Power12V
{

	public abstract int GetPower();

}