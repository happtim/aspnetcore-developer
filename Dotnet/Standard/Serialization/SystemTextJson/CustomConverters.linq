<Query Kind="Statements">
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>

var animals = new List<Animal>
{
	new Dog { Name = "Buddy", Breed = "Golden Retriever" },
	new Cat { Name = "Whiskers", LikesMilk = true }
};

var options = new JsonSerializerOptions
{
	WriteIndented = true,
	Converters = { new AnimalConverter() }
};

// 序列化
string json = JsonSerializer.Serialize(animals, options);
Console.WriteLine(json);

// 反序列化
var deserializedAnimals = JsonSerializer.Deserialize<List<Animal>>(json, options);

foreach (var animal in deserializedAnimals)
{
	if (animal is Dog dog)
	{
		Console.WriteLine($"Dog: {dog.Name}, Breed: {dog.Breed}");
	}
	else if (animal is Cat cat)
	{
		Console.WriteLine($"Cat: {cat.Name}, Likes Milk: {cat.LikesMilk}");
	}
}

public class AnimalConverter : JsonConverter<Animal>
{
	//只有在序列化的时候会调用
	public override bool CanConvert(Type typeToConvert)
	{
		return base.CanConvert(typeToConvert);
	}

	public override Animal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
		{
			if (!doc.RootElement.TryGetProperty("Type", out JsonElement typeProperty))
			{
				throw new JsonException();
			}
			string typeDiscriminator = typeProperty.GetString();
			Animal result = typeDiscriminator switch
			{
				"Dog" => JsonSerializer.Deserialize<Dog>(doc.RootElement.GetRawText(), options),
				"Cat" => JsonSerializer.Deserialize<Cat>(doc.RootElement.GetRawText(), options),
				_ => throw new JsonException($"Unknown animal type: {typeDiscriminator}"),
			};
			return result;
		}
	}

	public override void Write(Utf8JsonWriter writer, Animal value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value, value.GetType(), options);
	}
}

public abstract class Animal
{
	public string Name { get; set; }
	public virtual string Type { get; }
}

public class Dog : Animal
{
	public string Breed { get; set; }
	public override string Type => "Dog";
}

public class Cat : Animal
{
	public bool LikesMilk { get; set; }
	public override string Type => "Cat";
}