<Query Kind="Statements" />

//对象字段值比较

Expression<Func<People, bool>> predicate = c => c.Id == 10;
Func<People, bool> func = predicate.Compile();
func.Invoke(new People()
{
	Id = 10
}).Dump("People.Id == 10");


//表达式生成
//参数表达式
ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "c");
//反射获取属性
PropertyInfo propertyId = typeof(People).GetProperty("Id");
//通过parameterExpression来获取调用Id
MemberExpression idExp = Expression.Property(parameterExpression, propertyId);
//常量表达式
ConstantExpression constant10 = Expression.Constant(10, typeof(int));
//二元表达式
BinaryExpression expressionExp = Expression.Equal(idExp, constant10);
//表达式树
Expression<Func<People, bool>> predicate1 = Expression.Lambda<Func<People, bool>>(expressionExp, new ParameterExpression[1]
{
	parameterExpression
});

Func<People, bool> func1 = predicate1.Compile();
bool bResult1 = func1.Invoke(new People()
{
	Id = 10
}).Dump("表达式生成方式");

class People
{
	public int Id { get; set; }
	public string Name { get; set; }
}