// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IdentitySample.Mvc.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitySample.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly ILogger _logger;

        public AuthMessageSender(
            IOptions<AuthMessageSenderOptions> optionsAccessor,
            ILogger<AuthMessageSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

        public Task SendEmailAsync(string email, string subject, string body)
        {
            if (string.IsNullOrEmpty(Options.SMTPPassword) || string.IsNullOrEmpty(Options.SMTPUsername) )
            {
                throw new Exception("Null SMTP Password Or Username");
            }

            string smtpServer = "gz-smtp.qcloudmail.com";
            int smtpPort = 465;

            string fromAddress = "no-reply@mail.deng-shen.com";

            // 创建邮件对象
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("deng-shen", fromAddress));
            message.To.Add(new MailboxAddress("收件人", email));
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
                    client.Authenticate(Options.SMTPUsername, Options.SMTPPassword);

                    // 发送邮件
                    client.Send(message);

                    // 断开连接
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send email: " + ex.Message);
            }

            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            _logger.LogDebug($"SMS: {message} sent to {number}");
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
