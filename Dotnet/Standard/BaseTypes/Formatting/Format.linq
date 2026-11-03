<Query Kind="Statements" />

//Standard numeric format strings (标准的数字格式)
// [format specifier][precision specifier] 
// Format specifier 是一个单字母说明，比如 currency or percent （货币 百分比），前面可以加个一个数字空白字符数字。

// Standard format specifiers （标准格式说明）
// "C" or "c" 	Currency （货币） 123.456 ("C", en-US) -> $123.46
// "D" or "d" 	Decimal  （表示十进制浮点数）  1234 ("D")-> 1234
// "E" or "e"   Exponential （指数） 1052.0329112756 ("E", en-US)-> 1.052033E+003
// "F" or "f"   Fixed-point  （定点数）  1234.567 ("F", en-US) -> 1234.57
// "G" or "g" 	General  （固定点数，科学计数） -123.456 ("G", en-US) -> -123.456
// "N" or "n" 	Number  （整数和十进制数字，有分割符） 1234.567 ("N", en-US)-> 1,234.57
// "P" or "p" 	Percent  （百分数） 1 ("P", en-US) -> 100.00 %
// "R" or "r" 	Round-trip 
// "X" or "x" 	Hexadecimal （十六进制） 255 ("X") -> FF

decimal value = 123.456m;
value.ToString("C2").Dump(); //¥123.46
string.Format("Your account balance is {0:C2}.", value).Dump();
decimal[] amounts = { 16305.32m, 18794.16m };
Console.WriteLine("Beginning Balance           Ending Balance");
Console.WriteLine("{0,-28:C2}{1,14:C2}", amounts[0], amounts[1]); //左对齐28 右对其 14


// X or x precision specifier (精度说明) 用来指定位数。如果大于数真实位数 左补0.
var valueX = 0x2045e;
valueX.ToString("x").Dump();
// Displays 2045e
valueX.ToString("X").Dump();
// Displays 2045E
valueX.ToString("X8").Dump();
// Displays 0002045E

//Custom numeric format strings （自定义数字格式）
// 可以创建自定义数值格式字符串（由一个或多个自定义数值说明符组成），以定义如何设置数值数据的格式。
// 格式说明：
// "0"	Zero placeholder 如果有数字则替换0，否则用0填充  1234.5678 ("00000") -> 01235
// "#"	Digit placeholder 将“#”符号替换为相应的数字（如果存在） ，否则，结果字符串中不显示任何数字。 1234.5678 ("#####") -> 1235
// "."	Decimal point 确定结果字符串中小数分隔符的位置。 0.45678 ("0.00", en-US) -> 0.46
// "%"	Percentage placeholder 将数字乘以 100，并在结果字符串中插入本地化的百分比符号。 0.3697 ("%#0.00", en-US) -> %36.97

// Standard date and time format strings （标准的日期时间格式）
// 日期或者时间一个字符的标准格式输出

// "d" 	Short date pattern. （短日期）2009-06-15T13:45:30 -> 6/15/2009 (en-US)
// "D" 	Long date pattern.	（长日期）2009-06-15T13:45:30 -> Monday, June 15, 2009 (en-US)
// "g" 	General date/time pattern. （通用日期短） 2009-06-15T13:45:30 -> 6/15/2009 1:45 PM (en-US)
// "G" 	General date/time pattern. （通用日期长）2009-06-15T13:45:30 -> 6/15/2009 1:45:30 PM (en-US)
// "M", "m" 	Month/day pattern. （月日） 2009-06-15T13:45:30 -> June 15 (en-US)
// "t" 	Short time pattern. （时间短）2009-06-15T13:45:30 -> 1:45 PM (en-US)
// "T" 	Long time pattern.  （时间长）2009-06-15T13:45:30 -> 1:45:30 PM (en-US)
// "Y", "y" 	Year month pattern. （年月格式）2009-06-15T13:45:30 -> June 2009 (en-US)

//Custom date and time format strings （自定义的日期时间格式）
DateTime thisDate1 = new DateTime(2011, 6, 10);
("Today is " + thisDate1.ToString("MMMM dd, yyyy") + ".").Dump();

// "d" 	The day of the month, from 1 through 31.  2009-06-01T13:45:30 -> 1
// "dd" 	The day of the month, from 01 through 31. 2009-06-01T13:45:30 -> 01
// "ddd" 	The abbreviated name of the day of the week. 2009-06-15T13:45:30 -> Mon (en-US)
// "dddd" 	The full name of the day of the week. 2009-06-15T13:45:30 -> Monday (en-US)
// "h" 	The hour, using a 12-hour clock from 1 to 12. 2009-06-15T01:45:30 -> 1
// "hh" 	The hour, using a 12-hour clock from 01 to 12. 2009-06-15T01:45:30 -> 01
// "H" 	The hour, using a 24-hour clock from 0 to 23. 2009-06-15T01:45:30 -> 1
// "HH" 	The hour, using a 24-hour clock from 00 to 23. 2009-06-15T01:45:30 -> 01
// "m" 	The minute, from 0 through 59. 2009-06-15T01:09:30 -> 9
// "mm" 	The minute, from 00 through 59.  2009-06-15T01:09:30 -> 09
// "M" 	The month, from 1 through 12. 2009-06-15T13:45:30 -> 6
//"MM" 	The month, from 01 through 12. 2009-06-15T13:45:30 -> 06
//"MMM" 	The abbreviated name of the month. 2009-06-15T13:45:30 -> Jun (en-US)
//"MMMM" 	The full name of the month. 2009-06-15T13:45:30 -> June (en-US)
// "s" 	The second, from 0 through 59. 2009-06-15T13:45:09 -> 9
// "ss" 	The second, from 00 through 59. 2009-06-15T13:45:09 -> 09
// "t" 	The first character of the AM/PM designator. 2009-06-15T13:45:30 -> P (en-US)
// "tt" 	The AM/PM designator. 2009-06-15T13:45:30 -> PM (en-US)
// "y" 	The year, from 0 to 99. 0001-01-01T00:00:00 -> 1
// "yy" 	The year, from 00 to 99.  0001-01-01T00:00:00 -> 01
//"yyy" 	The year, with a minimum of three digits. 0001-01-01T00:00:00 -> 001
// "yyyy" 	The year as a four-digit number. 0001-01-01T00:00:00 -> 0001


//Composite formatting  （符合格式）
// 复合格式字符串由固定文本与索引占位符混合而成，称为格式项。
// 格式化操作生成一个结果字符串，该字符串由原始固定文本与列表中对象的字符串表示形式混合而成。

// Composite format string （符合格式字符串）
string name = "Fred";
String.Format("Name = {0}, hours = {1:hh}", name, DateTime.Now).Dump();

//Format item syntax （设置项目语法的格式）
// {index[,alignment][:formatString]}

// Index component （索引组件）
// 索引（参数说明）是必需的，是从 0 开始的数字，用于标识对象列表中的相应项。
String.Format("小于 10 的素数: {0}, {1}, {2}, {3}", 2, 3, 5, 7).Dump();

String.Format("0x{0:X} {0:E} {0:N}",Int64.MaxValue).Dump(); //通过指定相同的参数说明符，多个格式项可以引用对象列表中的同一元素。

// Alignment component （对齐组件）
// 对齐组件是一个可选的参数，指示格式化字段宽度。
// 如果对齐值小于格式化字符串的长度，则忽略对齐方式，并以格式化字符串的长度作为字段宽度。
// 如果对齐方式为正，则字段中的格式化数据为右对齐，
// 如果对齐方式为负，则为左对齐。

string[] names = { "Adam", "Bridgette", "Carla", "Daniel", "Ebenezer", "Francine", "George" };
decimal[] hours = { 40, 6.667m, 40.39m, 82, 40.333m, 80, 16.75m };

var ss =  string.Format("{0,-20} {1,5}\n", "Name", "Hours");
for (int ctr = 0; ctr < names.Length; ctr++)
	Console.WriteLine("{0,-20} {1,5:N1}", names[ctr], hours[ctr]);

//Format string component （格式字符串组件）
// 用于要格式化的对象类型的格式字符串 ，标准或自定义数字，日期格式字符串。
