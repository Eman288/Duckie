//using MailKit.Net.Smtp;
//using MimeKit;

//namespace WebApplication1.Services
//{
//    public interface IEmailService
//    {
//        Task SendEmailAsync(string toEmail, string subject, string body);
//    }

//    public class EmailService : IEmailService
//    {
//        private readonly IConfiguration _configuration;

//        public EmailService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task SendEmailAsync(string toEmail, string subject, string body)
//        {
//            var email = new MimeMessage();
//            email.From.Add(new MailboxAddress("Ducki 🦆", _configuration["EmailSettings:FromEmail"]));
//            email.To.Add(new MailboxAddress("", toEmail));
//            email.Subject = subject;

//            var builder = new BodyBuilder { HtmlBody = body };
//            email.Body = builder.ToMessageBody();

//            using var smtp = new SmtpClient();
//            await smtp.ConnectAsync(
//                _configuration["EmailSettings:SmtpServer"],
//                int.Parse(_configuration["EmailSettings:Port"]),
//                MailKit.Security.SecureSocketOptions.StartTls
//            );
//            await smtp.AuthenticateAsync(
//                _configuration["EmailSettings:FromEmail"],
//                _configuration["EmailSettings:Password"]
//            );
//            await smtp.SendAsync(email);
//            await smtp.DisconnectAsync(true);
//        }
//    }
//}