<Query Kind="Program">
  <NuGetReference>AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Generics （泛型）

// AutoMapper 支持对泛型的映射。
void Main()
{
	// Create the mapping
	var configuration = new MapperConfiguration(cfg => 
		cfg.CreateMap(typeof(Source<>), typeof(Destination<>))
	);
	
	var mapper = configuration.CreateMapper();

	var source = new Source<int> { Value = 10 };

	var dest = mapper.Map<Source<int>, Destination<int>>(source);
	
	dest.Dump();

}

public class Source<T>
{
	public T Value { get; set; }
}

public class Destination<T>
{
	public T Value { get; set; }
}