using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DemoApp.DataAccess.Models.DataTable
{
    public class DataTableColumnSearch
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("regex")]
        public bool Regex { get; set; }

        public DataTableColumnSearch() { }

        protected DataTableColumnSearch(SerializationInfo info, StreamingContext context)
        {
            Value = info.GetString("value");
            Regex = info.GetBoolean("regex");
        }
    }
}
