<Query Kind="Statements" />

// 示例用的基础模型，供 GettingStart.linq 通过 #load 引入。
// 这个文件只放类型声明，不要写语句。

public class Car
{
	public string Name { get; set; } = string.Empty;

	public int NumberOfSeats { get; set; }

	public CarColor Color { get; set; }

	public Manufacturer? Manufacturer { get; set; }

	public List<Tire> Tires { get; } = new List<Tire>();
}

public enum CarColor
{
	Black = 1,
	Blue = 2,
	White = 3,
}

public class Manufacturer
{
	public Manufacturer(int id, string name)
	{
		Id = id;
		Name = name;
	}

	public int Id { get; }

	public string Name { get; }
}

public class Tire
{
	public string Description { get; set; } = string.Empty;
}

public class CarDto
{
	public string Name { get; set; } = string.Empty;

	public int NumberOfSeats { get; set; }

	public CarColorDto Color { get; set; }

	public ProducerDto? Producer { get; set; }

	public List<TireDto>? Tires { get; set; }
}

// Intentionally use different numeric values for demonstration purposes
public enum CarColorDto
{
	Yellow = 1,
	Green = 2,
	Black = 3,
	Blue = 4,
}

// The manufacturer, but named differently for demonstration purposes
public class ProducerDto
{
	public ProducerDto(int id, string name)
	{
		Id = id;
		Name = name;
	}

	public int Id { get; }

	public string Name { get; }
}

public class TireDto
{
	public string Description { get; set; } = string.Empty;
}
