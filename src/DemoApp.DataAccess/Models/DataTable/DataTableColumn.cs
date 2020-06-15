using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DemoApp.DataAccess.Models.DataTable
{
    public class DataTableColumn
    {
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("searchable")]
        public bool Searchable { get; set; }
        [JsonProperty("orderable")]
        public bool Orderable { get; set; }
        [JsonProperty("search")]
        public DataTableColumnSearch Search { get; set; }

        public DataTableColumn() { }

        protected DataTableColumn(SerializationInfo info, StreamingContext context)
        {
            Data = info.GetString("data");
            Name = info.GetString("name");
            Searchable = info.GetBoolean("searchable");
            Orderable = info.GetBoolean("orderable");
            Search = (DataTableColumnSearch)info.GetValue("search", typeof(DataTableColumnSearch));
        }
    }
}
