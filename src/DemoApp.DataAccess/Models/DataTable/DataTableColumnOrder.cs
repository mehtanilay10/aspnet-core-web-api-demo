using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DemoApp.DataAccess.Models.DataTable
{
    public class DataTableColumnOrder
    {
        [JsonProperty("column")]
        public int Column { get; set; }
        [JsonProperty("dir")]
        public string Dir { get; set; }

        public DataTableColumnOrder() { }

        protected DataTableColumnOrder(SerializationInfo info, StreamingContext context)
        {
            Column = info.GetInt16("column");
            Dir = info.GetString("dir");
        }
    }
}
