<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <Namespace>Riok.Mapperly.Abstractions</Namespace>
</Query>

// 基础模型在 Models.linq；LINQPad 7 不运行源生成器，[Mapper] 的 partial 方法实现
// 由 Generator.linq 生成到同名的 .g.linq。Mapper 有改动时先运行 Generator.linq，再运行本脚本。
#load "Models.linq"
#load "1_GettingStart.g.linq"

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
dto.Dump();

[Mapper]
public partial class CarMapper
{
	public partial CarDto CarToCarDto(Car car);
}
