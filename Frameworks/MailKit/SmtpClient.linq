<Query Kind="Statements">
  <NuGetReference>MailKit</NuGetReference>
  <Namespace>MimeKit</Namespace>
  <Namespace>MailKit.Net.Smtp</Namespace>
</Query>

string smtpServer = "gz-smtp.qcloudmail.com";
int smtpPort = 465;

string fromAddress = "no-reply@mail.deng-shen.com";
string toAddress = "526821398@qq.com";
string subject = "Hello, World!";
string body = "This is the body of the email.";

string username = "no-reply@mail.deng-shen.com";
string password = "password";


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

