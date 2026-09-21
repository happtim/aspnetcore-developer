<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <Namespace>Riok.Mapperly.Abstractions</Namespace>
</Query>

// 配置示例：User-implemented mapping methods —— Mapperly 生成不了、或者你想自己控制的部分，直接在 Mapper 里手写方法。
//   1) 按类型对自动发现：Mapper 里任何 “TSource → TTarget” 签名的普通方法，Mapperly 遇到这对类型就会调用它
//   2) 只给某个属性用：[MapProperty(..., Use = nameof(方法))]
//   3) [UserMapping(Default = false)]：方法仍可被 Use 引用，但不会成为该类型对的默认映射
#load "Models.linq"
#load "09_UserImplementedMapping.g.linq"

var mapper = new CarMapper();

var car = new Car
{
	Name = "Model 3",
	NumberOfSeats = 5,
	Color = CarColor.White,
	Manufacturer = new Manufacturer(1, "Tesla"),
};
car.Tires.Add(new Tire { Description = "Front-Left" });

var dto = mapper.CarToCarDto(car);
// Name 被 FormatName 转成大写；Tire.Description 同样是 string → string，但没受影响（Default = false 的作用）
// Color：White 在 CarColorDto 里不存在，手写规则把它归到 Yellow
// Producer.Name：手写方法加了 Id 前缀
dto.Dump("手写映射：Name / Color / Producer");

[Mapper]
public partial class CarMapper
{
	[MapProperty(nameof(Car.Manufacturer), nameof(CarDto.Producer))]
	[MapProperty(nameof(Car.Name), nameof(CarDto.Name), Use = nameof(FormatName))]
	public partial CarDto CarToCarDto(Car car);

	// 1) 自动发现：凡是需要 CarColor → CarColorDto 的地方都用它，取代 Mapperly 生成的枚举映射
	private CarColorDto MapColor(CarColor color) => color switch
	{
		CarColor.Black => CarColorDto.Black,
		CarColor.Blue => CarColorDto.Blue,
		_ => CarColorDto.Yellow,
	};

	// 1) 自动发现：Manufacturer → ProducerDto
	private ProducerDto MapProducer(Manufacturer manufacturer)
		=> new ProducerDto(manufacturer.Id, $"#{manufacturer.Id} {manufacturer.Name}");

	// 2) + 3) 只通过 Use 给 Car.Name 用。不加 Default = false 的话，它会变成所有 string → string 的默认映射，
	//    Tire.Description 也会被转成大写。
	[UserMapping(Default = false)]
	private string FormatName(string name) => name.ToUpperInvariant();
}
