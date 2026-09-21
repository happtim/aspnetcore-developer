<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <Namespace>Riok.Mapperly.Abstractions</Namespace>
</Query>

// 基础模型在 Models.linq；LINQPad 7 不运行源生成器，[Mapper] 的 partial 方法实现
// 由 Generator.linq 生成到同名的 .g.linq。Mapper 有改动时先运行 Generator.linq，再运行本脚本。
#load "Models.linq"
#load "07_Enum.g.linq"

var mapper = new CarMapper();
var car = new Car
{
	Name = "Model 3",
	NumberOfSeats = 5,
	Color = CarColor.Black,
	Manufacturer = new Manufacturer(1, "Tesla"),
};
car.Tires.Add(new Tire { Description = "Front-Left" });
car.Tires.Add(new Tire { Description = "Front-Right" });

var dto = mapper.CarToCarDto(car);
// Black 在 CarColor 里是 1、在 CarColorDto 里是 3：按名称映射得到 Black(3)；若按数值会得到 Yellow(1)
dto.Dump("使用名称映射，而不是值");

car.Color = CarColor.White;
dto = mapper.CarToCarDto(car);
dto.Dump("失败转化使用默认Black");


//[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName, EnumMappingIgnoreCase = true)]
[Mapper]
public partial class CarMapper
{
	public partial CarDto CarToCarDto(Car car);

	// 单独声明一个 CarColor → CarColorDto 的方法，Mapperly 在 CarToCarDto 里映射 Color 时会自动调用它
	[MapEnum(EnumMappingStrategy.ByName, IgnoreCase = true,FallbackValue = CarColorDto.Black)]
	private partial CarColorDto MapColor(CarColor color);
}
