<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>

// Projection (投影映射)
// 投影映射不是 水平的转化类的字段。如果没有额外的配置AutoMapper映射使用水平的方式类来转化字段。
// 如果要源转化成不匹配的目标中，必需指定自定义字段配置。

// Model
var calendarEvent = new CalendarEvent
{
	Date = new DateTime(2008, 12, 15, 20, 30, 0),
	Title = "Company Holiday Party"
};

// Configure AutoMapper
var configuration = new MapperConfiguration(cfg =>
  cfg.CreateMap<CalendarEvent, CalendarEventForm>()
	.ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.Date.Date))
	//使用两个参数的重载 而不是使用一个参数的解决 n expression tree lambda may not 问题
	.ForMember(dest => dest.Title, opt => opt.MapFrom( (src,dest) =>  JsonSerializer.Serialize(src.Date)))
	.ForMember(dest => dest.EventHour, opt => opt.MapFrom(src => src.Date.Hour))
	.ForMember(dest => dest.EventMinute, opt => opt.MapFrom(src => src.Date.Minute)));
	
	
var mapper = configuration.CreateMapper();

// Perform mapping
CalendarEventForm form = mapper.Map<CalendarEvent, CalendarEventForm>(calendarEvent);

form.Dump();


public class CalendarEvent
{
	public DateTime Date { get; set; }
	public string Title { get; set; }
}

public class CalendarEventForm
{
	public DateTime EventDate { get; set; }
	public int EventHour { get; set; }
	public int EventMinute { get; set; }
	public string Title { get; set; }
}