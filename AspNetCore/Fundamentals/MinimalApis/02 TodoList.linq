<Query Kind="Statements">
  <NuGetReference Version="6.0.16">Microsoft.EntityFrameworkCore.InMemory</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


var builder = WebApplication.CreateBuilder();

//出入数据库
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

//注入IApiDescriptionProvider，这个服务允许应用程序生成API的元数据 ，这些元数据可以用于生成API文档或Swagger UI等工具
builder.Services.AddEndpointsApiExplorer(); 

//添加Swagger 该方法会扫描应用程序中的API控制器，并使用反射机制来生成API文档的规范描述
builder.Services.AddSwaggerGen();
var app = builder.Build();

//启用Swagger文档生成器
app.UseSwagger();

//方法用于启用Swagger UI
app.UseSwaggerUI();

app.MapGet("/",async context => context.Response.Redirect("swagger") );

app.MapGet("/todoitems", async (TodoDb db) =>
	await db.Todos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
	await db.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
	await db.Todos.FindAsync(id)
		is Todo todo
			? Results.Ok(todo)
			: Results.NotFound());

app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
	db.Todos.Add(todo);
	await db.SaveChangesAsync();

	return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
	var todo = await db.Todos.FindAsync(id);

	if (todo is null) return Results.NotFound();

	todo.Name = inputTodo.Name;
	todo.IsComplete = inputTodo.IsComplete;

	await db.SaveChangesAsync();

	return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
	if (await db.Todos.FindAsync(id) is Todo todo)
	{
	db.Todos.Remove(todo);
	await db.SaveChangesAsync();
	return Results.Ok(todo);
	}

	return Results.NotFound();
});


Process.Start(new ProcessStartInfo
{
	FileName = "http://localhost:5000/",
	UseShellExecute = true,
});
app.Run();

class TodoDb : DbContext
{
	public TodoDb(DbContextOptions<TodoDb> options)
		: base(options) { }

	public DbSet<Todo> Todos => Set<Todo>();
}

public class Todo
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public bool IsComplete { get; set; }
}