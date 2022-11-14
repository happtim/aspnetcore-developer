<Query Kind="Program">
  <NuGetReference>AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Mapping Inheritance （映射继承）

// Mapping Inheritance 有两个功能
//	从基类或接口配置继承映射配置
//	运行时多态映射


void Main()
{
	//运行中多态
	var configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<BaseEntity, BaseDto>()
			//包含继承类的映射。
		   .Include<DerivedEntity, DerivedDto>()
		   .ForMember(dest => dest.SomeMember, opt => opt.MapFrom(src => src.OtherMember));
		   
		  cfg.CreateMap<DerivedEntity, DerivedDto>();

		//or  使用IncludeBase 继承类配置
//		cfg. CreateMap<BaseEntity, BaseDto>()
// 			.ForMember(dest => dest.SomeMember, opt => opt.MapFrom(src => src.OtherMember));
//
//		cfg.CreateMap<DerivedEntity, DerivedDto>()
//			.IncludeBase<BaseEntity, BaseDto>();
	});
	
	var mapper = configuration.CreateMapper( );
	
	BaseEntity baseEntity = new DerivedEntity{ OtherMember = "DerivedEntity"};
	////虽然转化的类型为BaseDto，但是本身是DerivedEntity。应该使用更合适的类型
	var derivedDto = mapper.Map<BaseDto>(baseEntity);	
	derivedDto.Dump();

	//As 重定向目标类型类型
	configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Order, OnlineOrderDto>();
    	cfg.CreateMap<Order, OrderDto>().As<OnlineOrderDto>();
	});
	mapper = configuration.CreateMapper();
 	mapper.Map<OrderDto>(new Order()).Dump();


}


class BaseEntity
{
	public string OtherMember {get;set;}
}

class BaseDto
{
	public string SomeMember {get;set;}	
}
class DerivedEntity :BaseEntity{}
class DerivedDto :BaseDto{}


public class Order { }
public class OnlineOrder : Order { }
public class MailOrder : Order { }

public class OrderDto { }
public class OnlineOrderDto : OrderDto { }
public class MailOrderDto : OrderDto { }
