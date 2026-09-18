<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <Namespace>Riok.Mapperly.Abstractions</Namespace>
</Query>

// 配置示例：PropertyNameMappingStrategy。
// Mapperly 默认按“区分大小写”匹配属性名，ModelName 与 modelName 被当成两个不相干的成员；
// 设为 CaseInsensitive 后忽略大小写匹配。本例模型只在大小写上不同，所以写在脚本里，不用共享的 Models.linq。
#load "3_PropertyName_CaseInsensitive.g.linq"

var car = new Car { ModelName = "Model 3", NumberOfSeats = 5 };

// 默认策略：一个属性都对不上，dto 全是默认值；Generator 会为每个成员报 RMG012 / RMG020 警告
new CaseSensitiveMapper().ToDto(car).Dump("默认 CaseSensitive：全部没映射上");

new CaseInsensitiveMapper().ToDto(car).Dump("CaseInsensitive：ModelName → modelName，NumberOfSeats → numberOfSeats");

[Mapper]
public partial class CaseSensitiveMapper
{
	public partial CarDto ToDto(Car car);
}

[Mapper(PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive)]
public partial class CaseInsensitiveMapper
{
	public partial CarDto ToDto(Car car);
}

public class Car
{
	public string ModelName { get; set; } = string.Empty;

	public int NumberOfSeats { get; set; }
}

// 故意用 camelCase 命名，模拟对接 JSON / 旧系统的 DTO
public class CarDto
{
	public string modelName { get; set; } = string.Empty;

	public int numberOfSeats { get; set; }
}
