<Query Kind="Statements" />

//https://zhuanlan.zhihu.com/p/58093669
//传统模式下的构造函数

HtmlBuilder textBuilder = new HtmlBuilder();
Director director = new Director(textBuilder);
director.Construct();

Util.RawHtml(textBuilder.getResult()).Dump();

public class Director
{
	private Builder _builder;

	public Director(Builder builder)
	{
		this._builder = builder;
	}

	// 编写文档
	public void Construct()
	{
		_builder.makeTitle("Greeting");
		_builder.makeString("从早上到中午");
		_builder.makeItems(new[] { "早上好.", "下午好." });
		_builder.close();
	}
}


public class TextBuilder : Builder
{

	private StringBuilder buffer = new StringBuilder();

	public override void close()
	{
		buffer.Append("===============================\r\n");
	}

	public override void makeItems(string[] items)
	{

		for (int i = 0; i < items.Length; i++)
		{
			buffer.Append("  ." + items[i] + "\r\n");
		}
	}

	public override void makeString(string str)
	{
		buffer.Append("." + str + "\r\n");
		buffer.Append("\r\n");
	}

	public override void makeTitle(string title)
	{
		buffer.Append("===============================\r\n");
		buffer.Append("[" + title + "]");
		buffer.Append("\r\n");
	}

	public string getResult()
	{
		return buffer.ToString();
	}

}

public class HtmlBuilder : Builder
{
	private string filename;
	private StringWriter writer;

	public override void close()
	{
		writer.WriteLine("</body></html>");
		writer.Close();
	}

	public override void makeItems(string[] items)
	{
		writer.WriteLine("<ul>");
		for (int i = 0; i < items.Length; i++)
		{
			writer.WriteLine("<li>" + items[i] + "</li>");
		}
		writer.WriteLine("</ul>");
	}

	public override void makeString(string str)
	{
		writer.WriteLine("<p>" + str + "</p>");
	}

	public override void makeTitle(string title)
	{
		filename = title + ".html";
		writer = new StringWriter();
		writer.WriteLine("<html><head><title>" + title + "</title></head><body>");
		writer.WriteLine("<h1>" + title + "</h1>");
	}

	public string getResult()
	{
		return writer.ToString();
	}
}

public abstract class Builder
{
	public abstract void makeTitle(string title);
	public abstract void makeString(string str);
	public abstract void makeItems(string[] items);
	public abstract void close();
}