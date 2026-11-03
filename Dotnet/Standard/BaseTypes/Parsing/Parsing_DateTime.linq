<Query Kind="Statements">
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.ComponentModel</Namespace>
</Query>


//如果是纯日期格式，Kind为Unspecified
DateTime day = DateTime.Parse("2023-11-02");
day.ToString("O").Dump(day.Kind.ToString());
"".Dump();

//转化UTC时间
var utc = day.ToUniversalTime();
utc.ToString("O").Dump(utc.Kind.ToString());
//UTC时间格式解析，Kind是Local
var putc = DateTime.Parse("2023-11-01T16:00:00.0000000Z");
putc.Dump(putc.Kind.ToString());

"".Dump();

//UTC时间转化为Local时间
var local = utc.ToLocalTime();
local.ToString("O").Dump(local.Kind.ToString());
//如果UTC格式的解析，Kind是Local
var plocal = DateTime.Parse("2023-11-02T00:00:00.0000000+08:00");
plocal.Dump(plocal.Kind.ToString());

"".Dump();

//Local UTC格式 调整为UTC时间
var d1 =  DateTime.Parse("2023-11-02T00:00:00.0000000+08:00",styles:DateTimeStyles.AdjustToUniversal);
d1.Dump(d1.Kind.ToString());

//UTC格式 调整为UTC时间
var d2 =  DateTime.Parse("2023-11-01T16:00:00.0000000Z",styles:DateTimeStyles.AdjustToUniversal);
d2.Dump(d2.Kind.ToString());

//如果只有日期，则Kind为Unspecified
var d3 = DateTime.Parse("2023-11-02", styles: DateTimeStyles.AdjustToUniversal);
d3.Dump(d3.Kind.ToString());

//不指定使用Local Kind
var d4 = DateTime.Parse("2023-11-02T00:00:00.0000000+08:00");
d4.ToString("o").Dump(d4.Kind.ToString());
