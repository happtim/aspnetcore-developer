<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
  <Namespace>FluentValidation.Validators</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/custom-validators.html#

//有三种方式可以自定义验证器，其中Predicate Validator和 Custom 方法 较为常用。 Property Validators 应用复杂场景。

Person person = new Person();
PersonValidator validator = new PersonValidator();

ValidationResult result = validator.Validate(person);

result.ToString().Dump();

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		//使用Predicate方式自定义
		RuleFor(x => x.Pets).Must(list => list.Count < 10)
		  .WithMessage("The list must contain fewer than 10 items");
		
		//也可以使用扩展方法简化
		RuleFor(x => x.Pets).ListMustContainFewerThan(10);
		
		//提示信息如下：{MaxElements} 为我们自定义添加占位符。
		//Pets’ must contain fewer than 10 items.
		RuleFor(x => x.Pets).ListMustContainFewerThanWithCustomMessagePlaceholders(10);

		//Custom方法自定义验证器，我们需要手动添加 ValidationFailure  实例到 context。
		RuleFor(x => x.Pets).Custom((list, context) =>
		{
			if (list.Count > 10)
			{
				//如果不加则不回报错。
				context.AddFailure("The list must contain 10 items or fewer");
			}
		});
		
		//也可以定义扩展类方式化简
		RuleFor(x => x.Pets).ListMustContainFewerThanCustom(10);
		
		//Property Validators 适用于复杂场景，可以控制各种逻辑在一个类中。通过继承PropertyValidator<T,TProperty>。builtin所有验证器都是继承这个类。
		RuleFor(person => person.Pets).SetValidator(new ListCountValidator<Person, Pet>(10));
		
		//扩展方式化简
		RuleFor(x => x.Pets).ListMustContainFewerThanPropertyValidators(10);
	}
}

public static class MyCustomValidators
{
	public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num)
	{
		return ruleBuilder.Must(list => list.Count < num).WithMessage("The list contains too many items");
	}

	public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThanWithCustomMessagePlaceholders<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num)
	{

		return ruleBuilder.Must((rootObject, list, context) =>
		{
			context.MessageFormatter.AppendArgument("MaxElements", num);
			return list.Count < num;
		})
		.WithMessage("{PropertyName} must contain fewer than {MaxElements} items.");
	}

	public static IRuleBuilderOptionsConditions<T, IList<TElement>> ListMustContainFewerThanCustom<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num)
	{

		return ruleBuilder.Custom((list, context) =>
		{
			if (list.Count > 10)
			{
				context.AddFailure("The list must contain 10 items or fewer");
			}
		});
	}

	public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThanPropertyValidators<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num)
	{
		return ruleBuilder.SetValidator(new ListCountValidator<T, TElement>(num));
	}
}

public class ListCountValidator<T, TCollectionElement> : PropertyValidator<T, IList<TCollectionElement>>
{
	private int _max;

	public ListCountValidator(int max)
	{
		_max = max;
	}

	//context 和 验证属性
	public override bool IsValid(ValidationContext<T> context, IList<TCollectionElement> list)
	{
		if (list != null && list.Count >= _max)
		{
			context.MessageFormatter.AppendArgument("MaxElements", _max);
			return false;
		}

		return true;
	}

	public override string Name => "ListCountValidator";
	//获取模板
	protected override string GetDefaultMessageTemplate(string errorCode)
		=> "{PropertyName} must contain fewer than {MaxElements} items.";
}



public class Pet
{
}

public class Person
{
	public IList<Pet> Pets { get; set; } = new List<Pet>();
}