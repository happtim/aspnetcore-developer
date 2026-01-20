<Query Kind="Program">
  <NuGetReference>System.Linq.Dynamic.Core</NuGetReference>
  <NuGetReference Version="8.3.4">Volo.Abp.ObjectExtending</NuGetReference>
  <Namespace>System.Linq.Dynamic.Core</Namespace>
  <Namespace>Volo.Abp.Data</Namespace>
  <Namespace>Volo.Abp.ObjectExtending</Namespace>
</Query>

// =============================================================================
// 共享数据模型和初始化方法
// =============================================================================
// 供其他测试文件通过 #load 引用

public static List<Product> InitializeProducts()
{
	var baseTime = DateTime.Now.AddDays(-30);
	
	var products = new List<Product>
	{
		new Product
		{
			Id = Guid.NewGuid(),
			Name = "产品A",
			Active = true,
			CreationTime = baseTime,
			Tags = new List<ProductTag>
			{
				new ProductTag 
				{ 
					Id = Guid.NewGuid(), 
					Name = "标签1",
					CreationTime = baseTime
				}.SetProperty("CustomValue", 150),
				new ProductTag 
				{ 
					Id = Guid.NewGuid(), 
					Name = "标签2",
					CreationTime = baseTime
				}.SetProperty("CustomValue", 49)
			}
		}.SetProperty("CustomColor", "Red").SetProperty("CustomRating", 5),
		
		new Product
		{
			Id = Guid.NewGuid(),
			Name = "产品B",
			Active = true,
			CreationTime = baseTime.AddDays(1),
			Tags = new List<ProductTag>
			{
				new ProductTag 
				{ 
					Id = Guid.NewGuid(), 
					Name = "标签3",
					CreationTime = baseTime.AddDays(1)
				}.SetProperty("CustomValue", 121)
			}
		}.SetProperty("CustomColor", "Blue").SetProperty("CustomRating", 4),
		
		new Product
		{
			Id = Guid.NewGuid(),
			Name = "产品C",
			Active = false,
			CreationTime = baseTime.AddDays(2),
			Tags = new List<ProductTag>
			{
				new ProductTag 
				{ 
					Id = Guid.NewGuid(), 
					Name = "标签4",
					CreationTime = baseTime.AddDays(2)
				}.SetProperty("CustomValue", 201)
			}
		}.SetProperty("CustomColor", "Red").SetProperty("CustomRating", 3),
		
		new Product
		{
			Id = Guid.NewGuid(),
			Name = "产品D",
			Active = true,
			CreationTime = baseTime.AddDays(3),
			Tags = new List<ProductTag>
			{
				new ProductTag 
				{ 
					Id = Guid.NewGuid(), 
					Name = "标签5",
					CreationTime = baseTime.AddDays(3)
				}.SetProperty("CustomValue", 79)
			}
		}.SetProperty("CustomColor", "Green").SetProperty("CustomRating", 2)
	};
	
	return products;
}

public static void PrintProducts(List<Product> products)
{
	foreach (var product in products)
	{
		var color = product.GetProperty<string>("CustomColor") ?? "N/A";
		var rating = product.GetProperty<int>("CustomRating");
		
		Console.WriteLine($"产品: {product.Name,-10} | 激活: {product.Active,-5} | " +
						$"Color: {color,-8} | Rating: {rating} | 标签数: {product.Tags.Count}");
		
		foreach (var tag in product.Tags)
		{
			var value = tag.GetProperty<int>("CustomValue");
			Console.WriteLine($"  └─ {tag.Name,-10} | CustomValue: {value}");
		}
	}
	Console.WriteLine(new string('-', 80));
}

// =============================================================================
// 数据模型
// =============================================================================

/// <summary>
/// 产品实体 - 支持 ABP 扩展属性
/// </summary>
public class Product : IHasExtraProperties
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public bool Active { get; set; }
	public DateTime CreationTime { get; set; }
	
	// 导航属性
	public List<ProductTag> Tags { get; set; } = new();
	
	// ABP 扩展属性
	public ExtraPropertyDictionary ExtraProperties { get; set; } = new ExtraPropertyDictionary();
}

/// <summary>
/// 产品标签 - 支持 ABP 扩展属性
/// </summary>
public class ProductTag : IHasExtraProperties
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public DateTime CreationTime { get; set; }
	
	public Guid? ProductId { get; set; }
	
	// ABP 扩展属性
	public ExtraPropertyDictionary ExtraProperties { get; set; } = new ExtraPropertyDictionary();
}
