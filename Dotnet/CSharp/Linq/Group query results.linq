<Query Kind="Statements" />

//https://learn.microsoft.com/zh-cn/dotnet/csharp/linq/group-query-results

"按单个属性分组示例:".Dump();
//通过使用元素的单个属性作为分组键对源元素进行分组。
//在此示例中，键是 string，即学生的姓氏。
var groupByLastNamesQuery =
	from student in Student.students
	group student by student.LastName into newGroup
	orderby newGroup.Key
	select newGroup;
	
groupByLastNamesQuery.ToList().Dump();


"按值分组示例:".Dump();
//使用除对象属性以外的某个项作为分组键对源元素进行分组。
//在此示例中，键是学生姓氏的第一个字母。
var groupByFirstLetterQuery =
	from student in Student.students
	group student by student.LastName[0];
	

groupByFirstLetterQuery.ToList().Dump();

"按范围分组示例:".Dump();
//使用某个数值范围作为分组键对源元素进行分组。 
//然后，查询将结果投影到一个匿名类型中，该类型仅包含学生的名字和姓氏以及该学生所属的百分点范围。
int GetPercentile(Student s)
{
	double avg = s.ExamScores.Average();
	return avg > 0 ? (int)avg / 10 : 0;
}

var groupByPercentileQuery =
	from student in Student.students
	let percentile = GetPercentile(student)
	group new
	{
		student.FirstName,
		student.LastName
	} by percentile into percentGroup
	orderby percentGroup.Key
	select percentGroup;
	
groupByPercentileQuery.ToList().Dump();

"按比较分组示例:".Dump();
//过使用布尔比较表达式对源元素进行分组。 
//在此示例中，布尔表达式会测试学生的平均考试分数是否超过 75

var groupByHighAverageQuery =
	from student in Student.students
	group new
	{
		student.FirstName,
		student.LastName
	} by student.ExamScores.Average() > 75 into studentGroup
	select studentGroup;

groupByHighAverageQuery.ToList().Dump();


"按匿名类型分组:".Dump();
//使用匿名类型来封装包含多个值的键。 
//在此示例中，第一个键值是学生姓氏的第一个字母。 
//第二个键值是一个布尔值，指定该学生在第一次考试中的得分是否超过了 85。

var groupByCompoundKey =
	from student in Student.students
	group student by new
	{
		FirstLetter = student.LastName[0],
		IsScoreOver85 = student.ExamScores[0] > 85
	} into studentGroup
	orderby studentGroup.Key.FirstLetter
	select studentGroup;
	
groupByCompoundKey.ToList().Dump();
	
class Student
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public int ID { get; set; }
	public GradeLevel? Year { get; set; }
	public List<int> ExamScores { get; set; }

	public Student(string FirstName, string LastName, int ID, GradeLevel Year, List<int> ExamScores)
	{
		this.FirstName = FirstName;
		this.LastName = LastName;
		this.ID = ID;
		this.Year = Year;
		this.ExamScores = ExamScores;
	}

	public Student(string FirstName, string LastName, int StudentID, List<int>? ExamScores = null)
	{
		this.FirstName = FirstName;
		this.LastName = LastName;
		ID = StudentID;
		this.ExamScores = ExamScores ?? Enumerable.Empty<int>().ToList();
	}

	public static List<Student> students = new()
	{
		new(
			FirstName: "Terry", LastName: "Adams", ID: 120,
			Year: GradeLevel.SecondYear,
			ExamScores: new() { 99, 82, 81, 79 }
		),
		new(
			"Fadi", "Fakhouri", 116,
			GradeLevel.ThirdYear,
			new() { 99, 86, 90, 94 }
		),
		new(
			"Hanying", "Feng", 117,
			GradeLevel.FirstYear,
			new() { 93, 92, 80, 87 }
		),
		new(
			"Cesar", "Garcia", 114,
			GradeLevel.FourthYear,
			new() { 97, 89, 85, 82 }
		),
		new(
			"Debra", "Garcia", 115,
			GradeLevel.ThirdYear,
			new() { 35, 72, 91, 70 }
		),
		new(
			"Hugo", "Garcia", 118,
			GradeLevel.SecondYear,
			new() { 92, 90, 83, 78 }
		),
		new(
			"Sven", "Mortensen", 113,
			GradeLevel.FirstYear,
			new() { 88, 94, 65, 91 }
		),
		new(
			"Claire", "O'Donnell", 112,
			GradeLevel.FourthYear,
			new() { 75, 84, 91, 39 }
		),
		new(
			"Svetlana", "Omelchenko", 111,
			GradeLevel.SecondYear,
			new() { 97, 92, 81, 60 }
		),
		new(
			"Lance", "Tucker", 119,
			GradeLevel.ThirdYear,
			new() { 68, 79, 88, 92 }
		),
		new(
			"Michael", "Tucker", 122,
			GradeLevel.FirstYear,
			new() { 94, 92, 91, 91 }
		),
		new(
			"Eugene", "Zabokritski", 121,
			GradeLevel.FourthYear,
			new() { 96, 85, 91, 60 }
		)
	};
}

enum GradeLevel
{
	FirstYear = 1,
	SecondYear,
	ThirdYear,
	FourthYear
};