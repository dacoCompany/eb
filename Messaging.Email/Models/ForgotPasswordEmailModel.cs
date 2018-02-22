namespace Messaging.Email.Models
{
    public class ForgotPasswordEmailModel
    {
        public string Name { get; set; }
        public string ResetLink { get; set; }
    }
}
