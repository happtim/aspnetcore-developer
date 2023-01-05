<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/inheritance.html

//如果类的属性 是一个基类或者是一个基类或者接口， 可以使用 SetValidator 去指定一个子类实现的验证器。
ContactRequest request = new ContactRequest { Contact = new Person(), MessageToSend = "Hello World" };
var validator = new ContactRequestValidator();
var result = validator.Validate(request);
result.ToString().Dump();

//可以使用 SetInheritanceValidator 方法运行时，决定接口或者基类的子类实现 使用那种验证方式。
public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
	public ContactRequestValidator()
	{
		RuleFor(x => x.Contact).SetInheritanceValidator(v =>
		{
			v.Add<Organisation>(new OrganisationValidator());
			v.Add<Person>(new PersonValidator());
		});

	}
}


public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		RuleFor(x => x.Name).NotNull();
		RuleFor(x => x.Email).NotNull();
		RuleFor(x => x.DateOfBirth).GreaterThan(DateTime.MinValue);
	}
}

public class OrganisationValidator : AbstractValidator<Organisation>
{
	public OrganisationValidator()
	{
		RuleFor(x => x.Name).NotNull();
		RuleFor(x => x.Email).NotNull();
		RuleFor(x => x.HeadQuarters).SetValidator(new AddressValidator());
	}
}


public class AddressValidator : AbstractValidator<Address>
{
	public AddressValidator()
	{
		RuleFor(address => address.Postcode).NotNull();
	}
}

// We have an interface that represents a 'contact',
// for example in a CRM system. All contacts must have a name and email.
public interface IContact
{
	string Name { get; set; }
	string Email { get; set; }
}

// A Person is a type of contact, with a name and a DOB.
public class Person : IContact
{
	public string Name { get; set; }
	public string Email { get; set; }

	public DateTime DateOfBirth { get; set; }
}

// An organisation is another type of contact,
// with a name and the address of their HQ.
public class Organisation : IContact
{
	public string Name { get; set; }
	public string Email { get; set; }

	public Address HeadQuarters { get; set; }
}

// Our model class that we'll be validating.
// This might be a request to send a message to a contact.
public class ContactRequest
{
	public IContact Contact { get; set; }

	public string MessageToSend { get; set; }
}

public class Address
{
	public string Line1 { get; set; }
	public string Line2 { get; set; }
	public string Town { get; set; }
	public string County { get; set; }
	public string Postcode { get; set; }
}