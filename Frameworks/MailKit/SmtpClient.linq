<Query Kind="Statements">
  <NuGetReference Version="4.1.0">MailKit</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Configuration.UserSecrets</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>MailKit.Net.Smtp</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>MimeKit</Namespace>
</Query>


var config  = new ConfigurationBuilder()
	.AddUserSecrets("linqpad-9013DB32-D1F9-400A-9057-4CFF2B283D2D")
	.Build();

string smtpServer = "gz-smtp.qcloudmail.com";
int smtpPort = 465;

string fromAddress = "no-reply@mail.deng-shen.com";
string toAddress = "526821398@qq.com";
string subject = "Hello, World!";
string body = "This is the body of the email.";

string username = "no-reply@mail.deng-shen.com";
string password = config["SMTPPassword"];

// 创建邮件对象
var message = new MimeMessage();
message.From.Add(new MailboxAddress("deng-shen", fromAddress));
message.To.Add(new MailboxAddress("用户名称", toAddress));
message.Subject = subject;

// 创建邮件正文
var bodyBuilder = new BodyBuilder();
bodyBuilder.TextBody = body;

// 将正文添加到邮件中
message.Body = bodyBuilder.ToMessageBody();

try
{
	// 创建 SmtpClient 对象
	using (var client = new SmtpClient())
	{
		// 连接到 SMTP 服务器
		client.Connect(smtpServer, smtpPort, true);

		// 使用 SMTP 服务器的身份验证凭据进行身份验证
		client.Authenticate(username, password);

		// 发送邮件
		client.Send(message);

		// 断开连接
		client.Disconnect(true);
	}
}
catch (Exception ex)
{
	Console.WriteLine("Failed to send email: " + ex.Message);
}

