<Query Kind="Statements" />

Account acc = new CurrentAccount();
acc.Handle("user", "123456");


public class SavingAccount : Account
{
	public override void calculateInterest()
	{
		Console.WriteLine("按照定期计算利息");
	}
}

class CurrentAccount : Account
{
	public override void calculateInterest()
	{
		Console.WriteLine("按照活期计算利息");
	}
}

public abstract class Account
{

	// 基本方法 具体方法
	public bool validate(string account, string password)
	{
		Console.WriteLine(String.Format("Account:{0}", account));
		Console.WriteLine(String.Format("Password:{0}", password));
		// 模拟登录    

		if (account.Equals("user") && password.Equals("123456"))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	// 基本方法——抽象方法 需要子类重写的方法 
	public abstract void calculateInterest();

	// 基本方法——具体方法    
	public void display()
	{
		Console.WriteLine("显示利息！");
	}

	// 模板方法    
	public void Handle(String account, String password)
	{

		if (!validate(account, password))
		{
			Console.WriteLine("账户或密码错误！");
			return;
		}
		calculateInterest();
		display();
	}

}