namespace DemoApp.Models.AppSettings
{
    public class AppSettings
    {
        public Logging Logging { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public JwtIssuerOptions JwtIssuerOptions { get; set; }
        public SendgridOptions SendGridOptions { get; set; }
        public SwaggerDetails SwaggerDetails { get; set; }
        public WebsiteDetails WebsiteDetails { get; set; }
    }
}
