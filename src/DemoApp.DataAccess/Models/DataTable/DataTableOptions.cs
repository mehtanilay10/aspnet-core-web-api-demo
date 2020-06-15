using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DemoApp.DataAccess.Models.DataTable
{
    public class DataTableOptions
    {
        [JsonProperty("draw")]
        public string Draw { get; set; }
        [JsonProperty("start")]
        public int Start { get; set; }
        [JsonProperty("length")]
        public int Length { get; set; }
        [JsonProperty("order")]
        public List<DataTableColumnOrder> Order { get; set; }
        [JsonProperty("columns")]
        public List<DataTableColumn> Columns { get; set; }
        [JsonProperty("search")]
        public DataTableColumnSearch Search { get; set; }
        [JsonProperty("params")]
        public List<string> Params { get; set; }

        public DataTableOptions() { }

        public DataTableOptions(SerializationInfo info, StreamingContext context)
        {
            Search = new DataTableColumnSearch();
            Params = new List<string>();

            Draw = info.GetString("draw");
            Start = info.GetInt32("start");
            Length = info.GetInt32("length");
            Order = (List<DataTableColumnOrder>)info.GetValue("order", typeof(List<DataTableColumnOrder>));
            Columns = (List<DataTableColumn>)info.GetValue("columns", typeof(List<DataTableColumn>));
            Search = (DataTableColumnSearch)info.GetValue("search", typeof(DataTableColumnSearch));
            Params = (List<string>)info.GetValue("params", typeof(List<string>));
        }

    }
}
