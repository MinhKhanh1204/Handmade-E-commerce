using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Services
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendPasswordResetEmailAsync(string toEmail, string resetCode)
		{
			try
			{
				var smtpHost = _configuration["EmailSettings:SmtpHost"];
				var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
				var senderEmail = _configuration["EmailSettings:SenderEmail"];
				var senderPassword = _configuration["EmailSettings:SenderPassword"];
				var senderName = _configuration["EmailSettings:SenderName"];

				using (var client = new SmtpClient(smtpHost, smtpPort))
				{
					client.EnableSsl = true;
					client.Credentials = new NetworkCredential(senderEmail, senderPassword);

					var mailMessage = new MailMessage
					{
						From = new MailAddress(senderEmail, senderName),
						Subject = "Password Reset Code - Handicraft Shop",
						Body = $@"
							<html>
							<body style='font-family: Arial, sans-serif;'>
								<div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
									<h2 style='color: #333;'>Password Reset Request</h2>
									<p>Hello,</p>
									<p>You have requested to reset your password. Please use the following code to reset your password:</p>
									<div style='background-color: #f4f4f4; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; margin: 20px 0;'>
										{resetCode}
									</div>
									<p>This code will expire in 15 minutes.</p>
									<p>If you did not request this password reset, please ignore this email.</p>
									<br>
									<p>Best regards,</p>
									<p><strong>Handicraft Shop Team</strong></p>
								</div>
							</body>
							</html>",
						IsBodyHtml = true
					};

					mailMessage.To.Add(toEmail);

					await client.SendMailAsync(mailMessage);
				}
			}
			catch (Exception ex)
			{
				// Log the error
				Console.WriteLine($"Error sending email: {ex.Message}");
				throw new Exception("Failed to send email. Please try again later.");
			}
		}
	}
}

