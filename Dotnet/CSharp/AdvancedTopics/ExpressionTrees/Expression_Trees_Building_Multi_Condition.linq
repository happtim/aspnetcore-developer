<Query Kind="Statements" />

//多条件的表达式

Expression<Func<People, bool>> predicate = c => c.Id.ToString() == "10" && c.Name.Equals("张三") && c.Age > 35;
Func<People, bool> func = predicate.Compile();
bool bResult = func.Invoke(new People()
{
	Id = 10,
	Name = "张三",
	Age = 36
});

ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "c");
//c.Age > 35
ConstantExpression constant35 = Expression.Constant(35);
PropertyInfo propAge = typeof(People).GetProperty("Age");
MemberExpression ageExp = Expression.Property(parameterExpression, propAge);
BinaryExpression cagExp = Expression.GreaterThan(ageExp, constant35);

//c.Name.Equals("张三")
ConstantExpression constantrichard = Expression.Constant("张三");
PropertyInfo propName = typeof(People).GetProperty("Name");
MemberExpression nameExp = Expression.Property(parameterExpression, propName);
MethodInfo equals = typeof(string).GetMethod("Equals", new Type[] { typeof(string) });
MethodCallExpression NameExp = Expression.Call(nameExp, equals, constantrichard);

//c.Id.ToString() == "10"
ConstantExpression constantExpression10 = Expression.Constant("10", typeof(string));
var idExp = Expression.PropertyOrField(parameterExpression, "Id");
MethodInfo toString = typeof(int).GetMethod("ToString", new Type[0]);
var toStringExp = Expression.Call(idExp, toString, Array.Empty<Expression>());
var EqualExp = Expression.Equal(toStringExp, constantExpression10);

//c.Id.ToString() == "10" && c.Name.Equals("张三") && c.Age > 35
var plus = Expression.AndAlso(EqualExp, NameExp);
var exp = Expression.AndAlso(plus, cagExp);
Expression<Func<People, bool>> predicate1 = Expression.Lambda<Func<People, bool>>(exp, new ParameterExpression[1]
{
	parameterExpression
});

Func<People, bool> func1 = predicate1.Compile();
bool bResult1 = func1.Invoke(new People()
{
	Id = 10,
	Name = "张三",
	Age = 36
}).Dump("多条件表达式");


class People
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Age {get;set;}
}