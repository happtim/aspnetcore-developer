<Query Kind="Statements" />

public class Blog
{
	public int Id { get; set; }

	public string Name { get; set; }
	
	public string Url { get; set; }

	public IList<Post> Posts { get; } = new List<Post>();
}

public class Post
{
	public int Id { get; set; }

	public string Title { get; set; }
	public string Content { get; set; }

	public int BlogId { get; set; }
	public Blog Blog { get; set; }
}