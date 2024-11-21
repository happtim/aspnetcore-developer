<Query Kind="Statements">
  <Namespace>System.Net.Mail</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

//不支持一些现代的email协议 建议使用 MailKit

//但是按照配置好地址发送邮件时，总是抛出异常。**"Unable to read data from the transport connection: The connection was closed."**  调用堆栈如下：

//at System.Net.Mail.SmtpReplyReaderFactory.ProcessRead(Byte[] buffer, Int32 offset, Int32 read, Boolean readLine)
//at System.Net.Mail.SmtpReplyReaderFactory.ReadLines(SmtpReplyReader caller, Boolean oneLine)
//at System.Net.Mail.SmtpReplyReaderFactory.ReadLine(SmtpReplyReader caller)
//at System.Net.Mail.SmtpConnection.GetConnection(String host, Int32 port)
//at System.Net.Mail.SmtpTransport.GetConnection(String host, Int32 port)
//at System.Net.Mail.SmtpClient.GetConnection()
//at System.Net.Mail.SmtpClient.Send(MailMessage message)

string smtpServer = "gz-smtp.qcloudmail.com";
int smtpPort = 465;

string fromAddress = "no-reply@mail.deng-shen.com";
string toAddress = "526821398@qq.com";
string subject = "Hello, World!";
string body = "This is the body of the email.";

string username = "no-reply@mail.deng-shen.com";
string password = "password";

MailMessage mailMessage = new MailMessage(fromAddress, toAddress)
{
	Subject = subject,
	Body = body,
};
mailMessage.Headers.Add("Content-Type","text/html; charset=UTF-8");

SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
{
	UseDefaultCredentials  = false,
	Credentials = new NetworkCredential(username, password),
	EnableSsl = true,
}

try
{
	smtpClient.Send(mailMessage);
	Console.WriteLine("Email sent successfully!");
}
catch (Exception ex)
{
	Console.WriteLine("Failed to send email: " + ex.Message);
}