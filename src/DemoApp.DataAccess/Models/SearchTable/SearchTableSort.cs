using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DemoApp.DataAccess.Models.SearchTable
{
    public class SearchTableSort
    {
        [JsonProperty("fieldName")]
        public string FieldName { get; set; }
        [JsonProperty("direction")]
        public string Direction { get; set; }

        public SearchTableSort() { }

        protected SearchTableSort(SerializationInfo info, StreamingContext context)
        {
            FieldName = info.GetString("fieldName");
            Direction = info.GetString("direction");
        }
    }
}
