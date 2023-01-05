<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>


//可以使用 ILanguageManager 接口自定替换所有或者部分得系统 错误消息
ValidatorOptions.Global.LanguageManager = new CustomLanguageManager();

var cultureInfo = CultureInfo.CurrentUICulture;
cultureInfo.Dump();

var validator = new PersonValidator();
var result = validator.Validate(new Person());
result.ToString().Dump();

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		RuleFor(person => person.Surname).NotNull();
		RuleFor(person => person.Forename).NotNull();
	}
}

// 我们自定义消息管理 继承 LanguageManager，
// 构造函数中使用 AddTranslation 替换掉默认 语言和提示错误消息。 
public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
{
	public CustomLanguageManager()
	{
		AddTranslation("en", "NotNullValidator", "'{PropertyName}' is required.");
		AddTranslation("en-US", "NotNullValidator", "'{PropertyName}' is required.");
		AddTranslation("en-GB", "NotNullValidator", "'{PropertyName}' is required.");
		AddTranslation("zh-CN", "NotNullValidator", "'{PropertyName}' 不不能为空.");
	}
}

public class Person
{
	public string Surname { get; set; }
	public string Forename { get; set; }
}