<Query Kind="Statements" />

EmployeeList list = new EmployeeList();

var fte1 = new FulltimeEmployee("张无忌", 3200.00, 45);
var fte2 = new FulltimeEmployee("杨过", 2000.00, 40);
var fte3 = new FulltimeEmployee("段誉", 2400.00, 38);
var pte1 = new ParttimeEmployee("洪七公", 80.00, 20);
var pte2 = new ParttimeEmployee("郭靖", 60.00, 18);

list.addEmployee(fte1,fte2,fte3,pte1,pte2);

Department dep = new FADepartment();
list.accept(dep);

//员工列表类：对象结构    
public class EmployeeList
{
	//定义一个集合用于存储员工对象    
	private List<Employee> list = new List<Employee>();

	public void addEmployee(params  Employee[] employees)
	{
		list.AddRange(employees);
	}

	//遍历访问员工集合中的每一个员工对象    
	public void accept(Department handler)
	{
		foreach (var employee in list)
		{
			employee.accept(handler);
		}
	}
}

//财务部类：具体访问者类    
class FADepartment : Department
{
	//实现财务部对全职员工的访问    
	public override void visit(FulltimeEmployee employee)
	{

		int workTime = employee.getWorkTime();
		double weekWage = employee.getWeeklyWage();
		if (workTime > 40)
		{
			weekWage = weekWage + (workTime - 40) * 100;
		}
		else if (workTime < 40)
		{
			weekWage = weekWage - (40 - workTime) * 80;
			if (weekWage < 0)
			{
				weekWage = 0;
			}
		}
		Console.WriteLine("正式员工" + employee.getName() + "实际工资为：" + weekWage + "元。");
	}

	//实现财务部对兼职员工的访问    
	public override void visit(ParttimeEmployee employee)
	{
		int workTime = employee.getWorkTime();
		double hourWage = employee.getHourWage();
		Console.WriteLine("临时工" + employee.getName() + "实际工资为：" + workTime * hourWage + "元。");
	}
}

//人力资源部类：具体访问者类    
class HRDepartment : Department
{
	//实现人力资源部对全职员工的访问    
	public override void visit(FulltimeEmployee employee)
	{
		int workTime = employee.getWorkTime();
		Console.WriteLine("正式员工" + employee.getName() + "实际工作时间为：" + workTime + "小时。");
		if (workTime > 40)
		{
			Console.WriteLine("正式员工" + employee.getName() + "加班时间为：" + (workTime - 40) + "小时。");
		}
		else if (workTime < 40)
		{
			Console.WriteLine("正式员工" + employee.getName() + "请假时间为：" + (40 - workTime) + "小时。");
		}
	}

	//实现人力资源部对兼职员工的访问    
	public override void visit(ParttimeEmployee employee)
	{
		int workTime = employee.getWorkTime();
		Console.WriteLine("临时工" + employee.getName() + "实际工作时间为：" + workTime + "小时。");
	}
}

//兼职员工类：具体元素类    
public class ParttimeEmployee : Employee
{

	private String name;
	private double hourWage;
	private int workTime;

	public ParttimeEmployee(String name, double hourWage, int workTime)
	{
		this.name = name;
		this.hourWage = hourWage;
		this.workTime = workTime;
	}

	public void setName(String name)
	{
		this.name = name;
	}

	public void setHourWage(double hourWage)
	{
		this.hourWage = hourWage;
	}

	public void setWorkTime(int workTime)
	{
		this.workTime = workTime;
	}

	public String getName()
	{
		return (this.name);
	}

	public double getHourWage()
	{
		return (this.hourWage);
	}

	public int getWorkTime()
	{
		return (this.workTime);
	}

	public void accept(Department handler)
	{
		handler.visit(this); //调用访问者的访问方法    
	}
}

//全职员工类：具体元素类    
public class FulltimeEmployee : Employee
{

	private String name;
	private double weeklyWage;
	private int workTime;

	public FulltimeEmployee(String name, double weeklyWage, int workTime)
	{
		this.name = name;
		this.weeklyWage = weeklyWage;
		this.workTime = workTime;
	}

	public void setName(String name)
	{
		this.name = name;
	}

	public void setWeeklyWage(double weeklyWage)
	{
		this.weeklyWage = weeklyWage;
	}

	public void setWorkTime(int workTime)
	{
		this.workTime = workTime;
	}

	public String getName()
	{
		return (this.name);
	}

	public double getWeeklyWage()
	{
		return (this.weeklyWage);
	}

	public int getWorkTime()
	{
		return (this.workTime);
	}

	public void accept(Department handler)
	{
		handler.visit(this); //调用访问者的访问方法    
	}

}



public interface Employee
{
	void accept(Department handler);

}

//访问者抽象类
public abstract class Department
{
	//声明一组重载的访问方法，用于访问不同类型的具体元素    
	public abstract void visit(FulltimeEmployee employee);
	public abstract void visit(ParttimeEmployee employee);
}