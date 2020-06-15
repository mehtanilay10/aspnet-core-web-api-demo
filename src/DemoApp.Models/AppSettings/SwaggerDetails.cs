namespace DemoApp.Models.AppSettings
{
    public class SwaggerDetails
    {
        public string ApiVersion { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        public string RoutePrefix { get; set; }
        public Contact Contact { get; set; }
        public Endpoints Endpoints { get; set; }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }

    public class Endpoints
    {
        public API API { get; set; }
    }

    public class API
    {
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
