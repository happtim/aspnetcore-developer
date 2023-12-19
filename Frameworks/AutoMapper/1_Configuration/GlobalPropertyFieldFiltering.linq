<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>


//Global property/field filtering （全局属性/字段筛选）
// 默认情况下，自动映射器会尝试映射每个公共属性/字段。您可以使用属性/字段筛选器筛选出属性/字段：
var config = new MapperConfiguration(cfg =>
{
	// 直接设置配置。
	cfg.CreateMap<Foo, FooDto>();
	// 禁止所有Field 字段映射
	cfg.ShouldMapField = fi => false;

	// 映射有公用get方法或者私有get方法的属性。
	cfg.ShouldMapProperty = pi =>
		pi.GetMethod != null && (pi.GetMethod.IsPublic || pi.GetMethod.IsPrivate);
});

var foo = new Foo {};
foo.Name = "Name";
foo.FieldName = "ShouldMapField=>false";
foo.PrivateProperty = "ShouldMapProperty => pi.GetMethod.IsPrivate";

var mapper = config.CreateMapper();
mapper.Map<FooDto>(foo).Dump();


class Foo
{
	public string Name { get; set; }
	public string FieldName;
	public string PrivateProperty { private get; set; }
}

class FooDto : Foo
{
	public new string PrivateProperty { get; set; }
}