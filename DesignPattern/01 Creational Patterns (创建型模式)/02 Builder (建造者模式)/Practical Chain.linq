<Query Kind="Statements" />


Computer computer = 
	new Builder("因特尔", "三星")
	.setDisplay("三星24寸")
	.setKeyboard("罗技")
	.setUsbCount(2)
	.build();
	
computer.Dump();

public class Computer
{
	public string Cpu { get; private set;} //必须
	public string Ram{ get; private set;}//必须
	public int UsbCount { get; set;} //可选
	public  string Keyboard { get; set;} //可选
    public  string Display { get; set;} //可选
	
	public Computer(string cpu, string ram)
	{
		this.Cpu = cpu;	
		this.Ram = ram;
	}
}

public class Builder
{
	private readonly Computer _computer;
	
	public Builder(string cpu, string ram)
	{
		_computer = new Computer(cpu, ram);
	}

	public Builder setUsbCount(int usbCount)
	{
		_computer.UsbCount = usbCount;
		return this;
	}
	public Builder setKeyboard(string keyboard)
	{
		_computer.Keyboard = keyboard;
		return this;
	}
	public Builder setDisplay(string display)
	{
		_computer.Display = display;
		return this;
	}
	public Computer build()
	{
		return _computer;
	}
}