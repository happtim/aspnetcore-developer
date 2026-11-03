<Query Kind="Statements">
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//https://www.cnblogs.com/sdBob/p/12151013.html

var button = new System.Windows.Controls.Button { Content = "Hello, world!" };
button.Click += (sender, args) => 
{ 
	var html =  GetHtml();
	button.Content = "I've just been gethtml()";
	html =  GetHtml2().Result;
	button.Content = "I've just been clicked!";
};

PanelManager.DisplayWpfElement(button, "My Button");

"123".ToUpper();

string GetHtml()
{
	HttpClient httpClient = new HttpClient();
	httpClient.BaseAddress = new Uri("https://www.baidu.com/");

	string html = httpClient.GetStringAsync("/").Result;

	html = "【" + html + "】";

	return html;
}

async Task<string> GetHtml2()
{
	HttpClient httpClient = new HttpClient();
	httpClient.BaseAddress = new Uri("https://www.baidu.com/");
	string html = await httpClient.GetStringAsync("/");
	html = "【" + html + "】";
	return html;
}