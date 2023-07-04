<Query Kind="Statements">
  <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
</Query>

// https://docs.fluentvalidation.net/en/latest/error-codes.html

var validator = new PersonValidator();
var result = validator.Validate(new Person());
result.ToString().Dump();
foreach (var failure in result.Errors)
{
	Console.WriteLine($"Property: {failure.PropertyName} Error Code: {failure.ErrorCode}");
}


//可以通过WithErrorCode 方法设置自定义的错误码。ErrorCode 和 ErrorMessage关系
//* ErrorMessage 可以通过 ErrorCode 查找找到指定语言的说明。如：NotNull() 错误码为：NotNullValidator，通过LanguageManager查找对应错误码的文字说明。
//* 如果提供了ErrorCode 你最好也提供对应的ErrorMessage 来创建自定义消息。
//* 如果提供了ErrorCode 没有提供Custom Message，则报错消息使用验证器的默认消息。
public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		RuleFor(person => person.Surname).NotNull().WithErrorCode("ERR1234");
		RuleFor(person => person.Forename).NotNull();
	}
}


public class Person
{ 
	public string Surname {get;set;}
	public string Forename {get;set;}
}