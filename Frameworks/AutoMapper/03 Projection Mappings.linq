<Query Kind="Program">
  <NuGetReference>AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Projection (投影映射)
// 投影映射不是 水平的转化类的字段。如果没有额外的配置AutoMapper映射使用水平的方式类来转化字段。
// 如果要源转化成不匹配的目标中，必需指定自定义字段配置。

void Main()
{
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
		.ForMember(dest => dest.EventHour, opt => opt.MapFrom(src => src.Date.Hour))
		.ForMember(dest => dest.EventMinute, opt => opt.MapFrom(src => src.Date.Minute)));
		
		
	var mapper = configuration.CreateMapper();

	// Perform mapping
	CalendarEventForm form = mapper.Map<CalendarEvent, CalendarEventForm>(calendarEvent);
	
	form.Dump();
}

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