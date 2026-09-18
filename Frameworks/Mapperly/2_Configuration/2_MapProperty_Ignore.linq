<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <Namespace>Riok.Mapperly.Abstractions</Namespace>
</Query>

// 配置示例：忽略成员。
//   [MapperIgnoreSource] 源上的成员不参与映射；[MapperIgnoreTarget] 目标上的成员不被赋值。
// 注意忽略是单边的：忽略源 Name 后，目标 Name 会报 RMG012（找不到来源）；忽略目标 Tires 后，源 Tires 会报 RMG020（没映射出去）。
// 想彻底安静，要么两边都忽略，要么用 4_RequiredMapping_OneSideStrict 里的 RequiredMappingStrategy 只检查一边。
#load "..\Models.linq"
#load "2_MapProperty_Ignore.g.linq"

var mapper = new CarMapper();

var car = new Car
{
	Name = "Model 3",
	Color = CarColor.Blue,
	Manufacturer = new Manufacturer(1, "Tesla")
};
car.Tires.Add(new Tire { Description = "Front-Left" });

var dto = mapper.CarToCarDto(car);
// Name 保持 CarDto 的默认值 ""，Tires 保持 null，尽管源对象上两者都有值；
// Producer 为 null 是因为没配 [MapProperty]（见 1_MapProperty_Custom），与忽略无关。
dto.Dump("忽略源 Name、忽略目标 Tires");

[Mapper]
public partial class CarMapper
{
	[MapperIgnoreTarget(nameof(CarDto.Tires))]
	[MapperIgnoreSource(nameof(Car.Name))]
	public partial CarDto CarToCarDto(Car car);
}
