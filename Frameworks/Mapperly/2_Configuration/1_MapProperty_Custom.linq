<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <Namespace>Riok.Mapperly.Abstractions</Namespace>
</Query>

// 配置示例：[MapProperty] 把名称不同的属性对应起来（Car.Manufacturer → CarDto.Producer）。
// 子目录里的脚本用相对路径引用共享模型；.g.linq 由根目录的 Generator.linq 生成到本目录。
#load "..\Models.linq"
#load "1_MapProperty_Custom.g.linq"

var mapper = new CarMapper();

var car = new Car
{
	Name = "Model 3",
	Color = CarColor.Blue,
	Manufacturer = new Manufacturer(1, "Tesla")
};

var dto = mapper.CarToCarDto(car);
// Color 没做配置，走默认的按数值映射：Blue(2) → CarColorDto 里数值为 2 的 Green
dto.Dump("[MapProperty] Manufacturer → Producer");

[Mapper]
public partial class CarMapper
{
	// Car.Manufacturer 与 CarDto.Producer 名称不同，需显式指定
	[MapProperty(nameof(Car.Manufacturer), nameof(CarDto.Producer))]
	public partial CarDto CarToCarDto(Car car);
}
