<Query Kind="Statements" />



var testMember = new Member("testName", "testEmail");

// use reflection to set DateJoined property
testMember.SetPrivateDateTimePropertyValue("DateJoined", DateTime.Today.AddYears(-1));

testMember.Dump();

public static class ReflectionHelperExtensionMethods
{
	// from https://stackoverflow.com/a/1565766/13680266
	public static void SetPrivateDateTimePropertyValue(this Member member, string propName, DateTime newValue)
	{
		PropertyInfo propertyInfo = typeof(Member).GetProperty(propName);
		if (propertyInfo == null) return;
		propertyInfo.SetValue(member, newValue);
	}
}

public class Member
{
	public string Name { get; set; }
	public string Email { get; set; }
	public DateTime DateJoined { get; private set; }

	public Member(string name, string email)
	{
		Name = name;
		Email = email;
		DateJoined = DateTime.Today;
	}

	// other code here
}