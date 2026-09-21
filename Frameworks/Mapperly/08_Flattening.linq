<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <Namespace>Riok.Mapperly.Abstractions</Namespace>
</Query>

// Flattening（展平）/ Unflattening（反展平）：嵌套对象的成员 ↔ 目标上的平铺属性。
//   1) 自动展平：目标属性名 = 嵌套路径按 PascalCase 拼接（Car.Manufacturer.Id → CarDto.ManufacturerId），无需配置
//   2) 手动展平：名字对不上时用 [MapProperty] 指定路径，三种写法等价
//   3) [MapNestedProperties]：把某个嵌套对象的所有成员一次性“提”到顶层参与匹配
//   4) 反展平：Mapperly 不会自动做，必须用 [MapProperty] 手动指定目标路径
// 基础模型在 Models.linq；Mapper 有改动时先运行 Generator.linq，再运行本脚本。
#load "Models.linq"
#load "08_Flattening.g.linq"

var mapper = new CarMapper();
var car = new Car
{
	Name = "Model 3",
	NumberOfSeats = 5,
	Color = CarColor.Black,
	Manufacturer = new Manufacturer(1, "Tesla"),
	Engine = new Engine { Horsepower = 283, FuelType = "Electric" },
};
car.Tires.Add(new Tire { Description = "Front-Left" });
car.Tires.Add(new Tire { Description = "Front-Right" });

mapper.CarToCarDto(car).Dump("1) 自动展平：ManufacturerId / ManufacturerName / EngineHorsepower");

// Manufacturer 可空：为 null 时展平属性保持默认值，不会抛 NullReferenceException
mapper.CarToCarDto(new Car { Name = "No maker" }).Dump("1) Manufacturer 为 null");

var summary = mapper.CarToSummary(car);
summary.Dump("2) 手动展平：Brand ← Manufacturer.Name，Hp ← Engine.Horsepower，Fuel ← Engine.FuelType");

mapper.CarToEngineInfo(car).Dump("3) MapNestedProperties：Engine 的成员直接对到顶层 Horsepower / FuelType");

mapper.SummaryToCar(summary).Dump("4) 反展平：Hp → Engine.Horsepower，Fuel → Engine.FuelType");

[Mapper]
public partial class CarMapper
{
	// 1) 什么都不用写，展平属性靠名字自动匹配
	[MapProperty(nameof(Car.Manufacturer), nameof(CarDto.Producer))]
	public partial CarDto CarToCarDto(Car car);

	// 2) 三种等价的路径写法（文档里的 [a, b] 集合表达式是 C# 12 语法，LINQPad 7 是 C# 10，用 new[] { }）
	[MapProperty(new[] { nameof(Car.Manufacturer), nameof(Manufacturer.Name)}, nameof(CarSummaryDto.Brand))]   // 数组
	//new[] { "Manufacturer", "Name" }   // 两跳：Car → .Manufacturer → .Name
	
	[MapProperty("Engine.Horsepower", nameof(CarSummaryDto.Hp))]                                                
	// 点号分隔的字符串
	
	[MapProperty(nameof(@Car.Engine.FuelType), nameof(CarSummaryDto.Fuel))]                              
	// “full nameof”：@ 前缀让 Mapperly 取完整路径 
	//检查 nameof( 后面第一个字符是不是 @。是的话，它就自己沿着语法树把 Car.Engine.FuelType 整条路径取出来，
	
	// 只要求目标成员都有来源；Car 上用不到的成员不报警（见 06_RequiredMapping_OneSideStrict）
	[MapperRequiredMapping(RequiredMappingStrategy.Target)]
	public partial CarSummaryDto CarToSummary(Car car);

	// 3) Engine.Horsepower → Horsepower、Engine.FuelType → FuelType；Name 照常从 Car.Name 来
	[MapNestedProperties(nameof(Car.Engine))]
	[MapperRequiredMapping(RequiredMappingStrategy.Target)]
	public partial CarEngineInfoDto CarToEngineInfo(Car car);

	// 4) 反展平要手动指定目标路径；Engine 为 null 时 Mapperly 会先 new 一个。
	//    Brand 回不去：Manufacturer 只有构造函数、属性只读，Mapperly 无法往它的成员上赋值，所以忽略。
	[MapProperty(nameof(CarSummaryDto.Hp), "Engine.Horsepower")]
	[MapProperty(nameof(CarSummaryDto.Fuel), nameof(@Car.Engine.FuelType))]
	[MapperIgnoreSource(nameof(CarSummaryDto.Brand))]
	[MapperRequiredMapping(RequiredMappingStrategy.Source)]
	public partial Car SummaryToCar(CarSummaryDto summary);
}

// 属性名和嵌套路径对不上（Brand / Hp / Fuel），只能手动配置
public class CarSummaryDto
{
	public string Name { get; set; } = string.Empty;

	public string Brand { get; set; } = string.Empty;

	public int Hp { get; set; }

	public string Fuel { get; set; } = string.Empty;
}

// 属性名和 Engine 的成员同名，配合 MapNestedProperties
public class CarEngineInfoDto
{
	public string Name { get; set; } = string.Empty;

	public int Horsepower { get; set; }

	public string FuelType { get; set; } = string.Empty;
}
