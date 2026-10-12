# Razor语法

文档地址：[Razor syntax reference for ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-7.0)

Razor是一种将基于.NET的代码嵌入网页的标记语法。Razor语法由Razor标记、C#和HTML组成。包含Razor的文件通常具有 `.cshtml` 文件扩展名。Razor还可以在Razor组件文件（ `.razor` ）中找到。Razor语法类似于各种JavaScript单页应用（SPA）框架的模板引擎，如Angular、React、VueJs和Svelte。
### 渲染 HTML

默认的Razor语言是HTML。从Razor标记中渲染HTML与从HTML文件中渲染HTML没有任何区别。在Razor文件中的 `.cshtml` 标记会被服务器原样渲染。

## Razor 语法

Razor支持C#，并使用 `@` 符号从HTML转换到C#。Razor评估C#表达式并将其呈现在HTML输出中。

当一个 `@` 符号后面跟着一个Razor保留关键字（page，namespace，inherits，for，if，else，try）等时，它会转换为Razor特定的标记。否则，它会转换为普通的HTML。

要在Razor标记中转义一个 `@` 符号，请使用第二个 `@` 符号：

```cshtml
<p>@@Username</p>
```

代码以单个 `@` 符号在HTML中呈现：

```html
<p>@Username</p>
```

## 隐式Razor表达式

隐式Razor表达式以 `@` 开头，后跟C#代码：

```cshtml
<p>@DateTime.Now</p> 
<p>@DateTime.IsLeapYear(2016)</p>
```

编译之后的代码为：

```csharp
__builder.OpenElement(1, "p");  
__builder.AddContent(2, DateTime.Now);  
__builder.CloseElement();  
__builder.AddMarkupContent(3, "\r\n");  
__builder.OpenElement(4, "p");  
__builder.AddContent(5, DateTime.IsLeapYear(2016));  
__builder.CloseElement();
```

可以看到，表达式直接在带入AddContent函数中，进行toString()。

除了C#的 `await` 关键字之外，隐式表达式不能包含空格。如果C#语句有明确的结尾，可以插入空格。

隐式表达式不能包含C#泛型，因为括号（ `<>` ）中的字符会被解释为HTML标签。以下代码是无效的：

```cshtml
<p>@GenericMethod<int>()</p>
```

## 显示Razor表达式

* 带空格表达式
* 表达式与文本连结
* 范型表达式

### 带空格表达式

明确的 Razor 表达式由带有平衡括号的符号组成。要渲染上周的时间，使用以下 Razor 标记：

```cshtml
<p>Last week this time: @(DateTime.Now - TimeSpan.FromDays(7))</p>
```

任何在 `@()` 括号内的内容都会被评估并呈现到输出中。

编译之后的代码如下：

```csharp
__builder.OpenElement(7, "p");  
__builder.AddContent(8, "Last week this time: ");  
__builder.AddContent(9, DateTime.Now - TimeSpan.FromDays(7.0));  
__builder.CloseElement();
```

在前一节中描述的隐式表达式通常不能包含空格。在下面的代码中，当前时间没有减去一周。

```cshtml
<p>Last week: @DateTime.Now - TimeSpan.FromDays(7)</p>
```

编译代码之后代码如下：

```csharp
__builder.OpenElement(11, "p");  
__builder.AddContent(12, "Last week: ");  
__builder.AddContent(13, DateTime.Now);  
__builder.AddContent(14, " - TimeSpan.FromDays(7)");  
__builder.CloseElement();
```


### 达式与文本连结

可以使用明确的表达式将文本与表达式结果连接起来：

```cshtml
@{
    var joe = new Person("Joe", 33);
}

<p>Age@(joe.Age)</p>
```

没有明确的表达， `<p>Age@joe.Age</p>` 被视为电子邮件地址， `<p>Age@joe.Age</p>` 被渲染。当作为明确的表达时， `<p>Age33</p>` 被渲染。
### 范型表达式

可以使用明确的表达式来渲染 `.cshtml` 文件中通用方法的输出。以下标记显示了如何纠正之前由C#泛型括号引起的错误。代码以明确的表达式形式编写：

```cshtml
<p>@(GenericMethod<int>())</p>
```


## 表达编码

C#表达式评估为字符串时会进行HTML编码。C#表达式评估为 `IHtmlContent` 时会直接通过 `IHtmlContent.WriteTo` 进行渲染。C#表达式不评估为 `IHtmlContent` 时会先通过 `ToString` 转换为字符串并在渲染之前进行编码。

```chtml
@("<span>Hello World</span>")
```

前面的代码生成以下HTML：

```html
&lt;span&gt;Hello World&lt;/span&gt;
```


## Razor代码块

Razor代码块以 `@` 开头，并由 `{}` 包围。与表达式不同，代码块中的C#代码不会被渲染。视图中的代码块和表达式共享相同的作用域，并按顺序定义。

```cshtml
@{
    var quote = "The future depends on what you do today. - Mahatma Gandhi";
}

<p>@quote</p>

@{
    quote = "Hate cannot drive out hate, only love can do that. - Martin Luther King, Jr.";
}

<p>@quote</p>****
```

编译之后的代码如下：

```csharp
string quote = "The future depends on what you do today. - Mahatma Gandhi";  
__builder.OpenElement(0, "p");  
__builder.AddContent(1, quote);  
__builder.CloseElement();  
quote = "Hate cannot drive out hate, only love can do that. - Martin Luther King, Jr.";  
__builder.OpenElement(2, "p");  
__builder.AddContent(3, quote);  
__builder.CloseElement();
```

代码块中使用带html函数，作为函数模板。

```cshtml
@{
    void RenderName(string name)
    {
        <p>Name: <strong>@name</strong></p>
    }

    RenderName("Mahatma Gandhi");
    RenderName("Martin Luther King, Jr.");
}
```

编译之后的代码如下：

```csharp
RenderName("Mahatma Gandhi");  
RenderName("Martin Luther King, Jr.");  
void RenderName(string name)  
{  
	__builder.OpenElement(4, "p");  
	__builder.AddContent(5, "Name: ");  
	__builder.OpenElement(6, "strong");  
	__builder.AddContent(7, name);  
	__builder.CloseElement();  
	__builder.CloseElement();  
}
```

### 隐式转化(c# -> html)

代码块的默认语言是C#，但Razor页面可以切换回HTML

```cshtml
@{
    var inCSharp = true;
    <p>Now in HTML, was in C# @inCSharp</p>
}
```

编译之后的代码如下：

```csharp
bool inCSharp = true;  
__builder.OpenElement(8, "p");  
__builder.AddContent(9, "Now in HTML, was in C# ");  
__builder.AddContent(10, inCSharp);  
__builder.CloseElement();
```
### 显示分隔转化(c# -> html)

这样的代码会编译报错。代码块中的代码默认认为是C#。

```
@if (true)
{
    Some content
}
```

请使用Razor `<text>` 标签将呈现字符包围起来。将该文字视为html输出内容。

```cshtml
@for (var i = 0; i < people.Length; i++)
{
    var person = people[i];
    <text>Name: @person.Name</text>
}
```


### 显示行转化(c# -> html)

要在代码块中将整行的其余部分呈现为HTML，请使用 `@:` 语法：

```cshtml
@for (var i = 0; i < people.Length; i++)
{
    var person = people[i];
    @:Name: @person.Name
}
```

如下代码块中，代码C#被html标签分割可，其内部的部分当作html内容，如果需要再识别if代码块，需要加@，其里面的内容又被认定为C#代码块。

```
@{
    var personName =  new string[] { "Tim", "Bob" };
    for(var i = 0; i< personName.Count(); i++)
    {
        var item = personName[i];

        <label>

        if(item == "Tim")
        {
            Name: Tim
        }
        
        @if(@item == "Tim")
        {
            @:Name: @item (Tim)
        }

        </label>
    }

}
```


### 条件属性渲染

Razor会自动省略不需要的属性。如果传入的值是 `null` 或 `false` ，则该属性不会被渲染。

```cshtml
<div class="@false">False</div>
<div class="@null">Null</div>
<div class="@("")">Empty</div>
<div class="@("false")">False String</div>
<div class="@("active")">String</div>
<input type="checkbox" checked="@true" name="true" />
<input type="checkbox" checked="@false" name="false" />
<input type="checkbox" checked="@null" name="null" />
```

前面的Razor标记生成以下HTML：

```html
<div>False</div>
<div>Null</div>
<div class="">Empty</div>
<div class="false">False String</div>
<div class="active">String</div>
<input type="checkbox" checked="checked" name="true">
<input type="checkbox" name="false">
<input type="checkbox" name="null">
```


## 控制结构

控制结构是代码块的扩展。代码块的所有方面（转换为标记、内联C#）也适用于以下结构：

### 条件句 `@if, else if, else, and @switch`

```cshtml
@if (value % 2 == 0)
{
    <p>The value was even.</p>
}
else if (value >= 1337)
{
    <p>The value is large.</p>
}
else
{
    <p>The value is odd and small.</p>
}
```

### 循环 `@for, @foreach, @while, and @do while`

```
@for (var i = 0; i < people.Length; i++) 
{ 
	var person = people[i]; 
	<p>Name: @person.Name</p> 
	<p>Age: @person.Age</p> 
}
```


## 指令

Razor指令由隐式表达式表示，后跟 `@` 符号的保留关键字。指令通常会改变视图的解析方式或启用不同的功能。

* @attribute：`@attribute` 指令将给定的属性添加到生成的页面或视图的类中。
* @code：`@code` 块使Razor组件能够向组件添加C#成员（字段、属性和方法）。
* @funcation：`@functions` 指令允许将C#成员（字段、属性和方法）添加到生成的类中。
* @implements：`@implements` 指令实现了生成类的接口。
* @inherits：`@inherits` 指令提供了对视图继承的类的完全控制。
* @inject：`@inject` 指令使Razor页面能够将服务从服务容器注入到视图中。
* @layout：`@layout` 指令为具有 `@page` 指令的可路由Razor组件指定布局。
* @namespace：设置生成的Razor页面、MVC视图或Razor组件的类的命名空间。
* @page：指定Razor组件应直接处理请求。
* @preservewhitespace：空格的处理。
* @typeparam：`@typeparam` 指令为生成的组件类声明了一个通用类型参数。
* @using：`@using` 指令将C# `using` 指令添加到生成的视图中。

## 指令属性

Blazor指令属性由隐式表达式表示，后跟 `@` 符号的保留关键字。指令属性通常会改变元素的解析方式或启用不同的功能。

* @attribute：`@attributes` 允许组件渲染未声明的属性。
* @bind：组件中的数据绑定是通过 `@bind` 属性实现的。
* @bind:culture：为解析和格式化提供全球化本地化。
* @on{Event}：Razor为组件提供事件处理功能。
* @on{Event}:preventDefault：阻止事件的默认操作。
* @on{Event}:stopPropagation：停止事件传播。
* @key：`@key` 指令属性会导致组件的差异算法根据键的值来保留元素或组件。
* @ref：组件引用（ `@ref` ）提供了一种引用组件实例的方式，以便您可以向该实例发出命令。

## 模板化的Razor委托

Razor模板允许您使用以下格式定义UI片段：

```
@<tag>...</tag>
```
