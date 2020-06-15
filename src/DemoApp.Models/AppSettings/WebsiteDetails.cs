using System.Collections.Generic;

namespace DemoApp.Models.AppSettings
{
    public class WebsiteDetails
    {
        public List<string> AdminMailIds { get; set; }
        public string Title { get; set; }
        public string DashboadHome { get; set; }
        public string WebApiHome { get; set; }
    }
}
