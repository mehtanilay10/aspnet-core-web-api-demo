namespace DemoApp.Models.AppSettings
{
    public class SendgridOptions
    {
        public string ApiKey { get; set; }
        public string FromDisplayName { get; set; }
        public string FromMailId { get; set; }
        public string ReplyMailId { get; set; }
    }
}
