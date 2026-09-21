<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <Namespace>Riok.Mapperly.Abstractions</Namespace>
</Query>

// 配置示例：One side strict property mappings（RequiredMappingStrategy）。
// 默认 Both：源上有成员没被用到（RMG020）、目标上有成员找不到来源（RMG012）都会报警告。
//   Source：只要求“源的每个成员都必须映射出去”，目标多出来的成员不管
//   Target：只要求“目标的每个成员都必须有来源”，源多出来的成员不管
//   None  ：两边都不检查
// 注意：这是【生成期的诊断】，不是运行时异常 —— 三个 Mapper 运行结果完全一样，
// 区别要看 Generator.linq 输出的 Diagnostics 表（Mapper 列区分是哪个类报的）。
//
// 所以 Mapperly 的 “strict” 只是警告，本身并不强制；正式项目里要强制，得在 .editorconfig 里把 RMG012 / RMG020 提成 error。
// 对比 AutoMapper：Map() 时静默跳过未映射成员，只有主动调用 AssertConfigurationIsValid() 才在运行期抛异常，
// 且默认只检查目标一边（MemberList.Destination ≈ Target，MemberList.Source ≈ Source，MemberList.None ≈ None）。
#load "06_RequiredMapping_OneSideStrict.g.linq"

var car = new Car { Name = "Model 3", NumberOfSeats = 5, InternalCode = "X-001" };

new BothMapper().ToDto(car).Dump("Both（默认）：RMG020 InternalCode + RMG012 DisplayLabel");
new SourceStrictMapper().ToDto(car).Dump("Source：只报 RMG020 InternalCode 没映射出去");
new TargetStrictMapper().ToDto(car).Dump("Target：只报 RMG012 DisplayLabel 找不到来源");
new MethodLevelMapper().ToDto(car).Dump("方法级 [MapperRequiredMapping(None)]：不报");

[Mapper]
public partial class BothMapper
{
	public partial CarDto ToDto(Car car);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public partial class SourceStrictMapper
{
	public partial CarDto ToDto(Car car);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TargetStrictMapper
{
	public partial CarDto ToDto(Car car);
}

[Mapper]
public partial class MethodLevelMapper
{
	// 也可以只对某个方法覆盖 Mapper 级别的设置
	[MapperRequiredMapping(RequiredMappingStrategy.None)]
	public partial CarDto ToDto(Car car);
}

public class Car
{
	public string Name { get; set; } = string.Empty;

	public int NumberOfSeats { get; set; }

	// 只有源上有
	public string InternalCode { get; set; } = string.Empty;
}

public class CarDto
{
	public string Name { get; set; } = string.Empty;

	public int NumberOfSeats { get; set; }

	// 只有目标上有
	public string DisplayLabel { get; set; } = string.Empty;
}
