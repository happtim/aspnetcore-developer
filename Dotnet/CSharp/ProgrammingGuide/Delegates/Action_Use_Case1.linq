<Query Kind="Statements" />

//根据Action的方式从类中提取参数值。

GetValuesDictionary<MudItem>(true,i => i.xs = 12 ,i => i.sm = 6).Dump();

GetValuesDictionary<MudItem>(false,i => i.xs = 12,i => i.sm = 6).Dump();

IDictionary<string,object> GetValuesDictionary<T>(bool removeDefaults, params Action<T>[] options)
	where T : new()
{
	// 创建一个字典来存储字段名和值
	var dictionary = new Dictionary<string, object>();

	var instance = new T();
	
	foreach (var option in options)
		option(instance);
		
	var newInstance = new T();

	// 获取对象的字段信息
	PropertyInfo[] properties = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
	
	// 遍历字段并添加到字典中
	foreach (PropertyInfo property in properties)
	{
		object value = property.GetValue(instance);

		if (value == null) 
		{
			continue;
		}

		if (!removeDefaults || !value.Equals(property.GetValue(newInstance)))
		{
			dictionary.Add(property.Name, value);
		}
		
	}

	return dictionary;
}



public class MudItem
{
	public int xs { get; set; }
	public int sm { get; set; }
	public int md { get; set; }
	public int lg { get; set; }
	public int xl { get; set; }
	public int xxl { get; set; }
}