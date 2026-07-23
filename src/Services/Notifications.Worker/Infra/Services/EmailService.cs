using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notifications.Worker.Domain.Interfaces;

namespace Notifications.Worker.Infra.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var smtpSection = _configuration.GetSection("Smtp");
        var saveToFileOnly = smtpSection.GetValue<bool>("SaveToFileOnly");

        if (saveToFileOnly)
        {
            await SaveEmailToFileAsync(toEmail, subject, htmlBody);
            return;
        }

        try
        {
            var host = smtpSection.GetValue<string>("Host") ?? "localhost";
            var port = smtpSection.GetValue<int>("Port", 25);
            var username = smtpSection.GetValue<string>("Username");
            var password = smtpSection.GetValue<string>("Password");
            var enableSsl = smtpSection.GetValue<bool>("EnableSsl");
            var fromAddress = smtpSection.GetValue<string>("FromAddress") ?? "no-reply@financeflow.com";
            var fromName = smtpSection.GetValue<string>("FromName") ?? "FinanceFlow";

            _logger.LogInformation("SMTP Config: Host={Host}, Port={Port}, Username={Username}, PasswordLength={PasswordLength}, EnableSsl={EnableSsl}, StartsWithQuote={StartsWithQuote}, EndsWithQuote={EndsWithQuote}", 
                host, port, username, password?.Length ?? 0, enableSsl, password?.StartsWith("\"") ?? false, password?.EndsWith("\"") ?? false);

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromAddress, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(toEmail));

            using var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = string.IsNullOrEmpty(username)
            };

            if (!string.IsNullOrEmpty(username))
            {
                smtpClient.Credentials = new NetworkCredential(username, password);
            }

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail por SMTP para {ToEmail}. Salvando em arquivo fallback.", toEmail);
            await SaveEmailToFileAsync(toEmail, subject, htmlBody);
        }
    }

    private async Task SaveEmailToFileAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var folderPath = Path.Combine(AppContext.BaseDirectory, "SentEmails");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{toEmail.Replace("@", "_at_")}.html";
            var filePath = Path.Combine(folderPath, fileName);

            var fileContent = $"<!-- PARA: {toEmail}\n     ASSUNTO: {subject}\n     DATA: {DateTime.Now} -->\n{htmlBody}";
            await File.WriteAllTextAsync(filePath, fileContent);
        }
        catch
        {
        }
    }
}
