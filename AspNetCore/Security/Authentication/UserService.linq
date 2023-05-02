<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
</Query>

public interface IUserService
{
	Task<User> Authenticate(string username, string password);
	Task<IEnumerable<User>> GetAll();
	Task<User> FindAsync(int id);
}

public class UserService : IUserService
{
	// users hardcoded for simplicity, store in a db with hashed passwords in production applications
	private List<User> _users = new List<User>
	{
		new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" },
		new User { Id = 2, FirstName = "Tim", LastName = "Ge", Username = "tim", Password = "123456" }
	};

	public async Task<User> Authenticate(string username, string password)
	{
		var user = await Task.Run(() => _users.SingleOrDefault(x => x.Username == username && x.Password == password));

		// return null if user not found
		if (user == null)
			return null;

		// authentication successful so return user details without password
		return user.WithoutPassword();
	}

	public async Task<User> FindAsync(int id)
	{
		return await Task.Run (() => _users.FirstOrDefault(u =>u.Id == id));
	}

	public async Task<IEnumerable<User>> GetAll()
	{
		return await Task.Run(() => _users.WithoutPasswords());
	}
}
public static class ExtensionMethods
{
	public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users)
	{
		return users.Select(x => x.WithoutPassword());
	}

	public static User WithoutPassword(this User user)
	{
		user.Password = null;
		return user;
	}
}

public class User
{
	public int Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
}

public class LoginRequest
{
	[Required]
	public string UserName { get; set; }

	[Required]
	public string Password { get; set; }
}