using System.Diagnostics;

namespace MarketDZ.Services
{
    /// <summary>
    /// Mock implementation of email service for development purposes
    /// </summary>
    public class MockEmailService : IEmailService
    {
        public Task<bool> SendEmailVerificationAsync(string toEmail, string verificationLink)
        {
            Debug.WriteLine($"[MOCK EMAIL] Sending verification email to {toEmail}");
            Debug.WriteLine($"[MOCK EMAIL] Verification link: {verificationLink}");
            return Task.FromResult(true);
        }

        public Task<bool> SendPasswordResetAsync(string toEmail, string resetLink)
        {
            Debug.WriteLine($"[MOCK EMAIL] Sending password reset email to {toEmail}");
            Debug.WriteLine($"[MOCK EMAIL] Reset link: {resetLink}");
            return Task.FromResult(true);
        }

        public Task<bool> SendGenericEmailAsync(string toEmail, string subject, string body)
        {
            Debug.WriteLine($"[MOCK EMAIL] Sending email to {toEmail}");
            Debug.WriteLine($"[MOCK EMAIL] Subject: {subject}");
            Debug.WriteLine($"[MOCK EMAIL] Body: {body}");
            return Task.FromResult(true);
        }
    }
}