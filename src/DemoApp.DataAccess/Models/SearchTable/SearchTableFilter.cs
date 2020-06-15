using DemoApp.Models.Helpers;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DemoApp.DataAccess.Models.SearchTable
{
    public class SearchTableFilter
    {
        [JsonProperty("fieldName")]
        public string FieldName { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("fieldType")]
        public FieldType FieldType { get; set; } = FieldType.Text;
        [JsonProperty("operator")]
        public FieldOperator Operator { get; set; } = FieldOperator.Contains;
        [JsonProperty("lookupName")]
        public string LookupName { get; set; } = string.Empty;

        public SearchTableFilter() { }

        protected SearchTableFilter(SerializationInfo info, StreamingContext context)
        {
            FieldName = info.GetString("fieldName");
            Value = info.GetString("value");
            FieldType = EnumHelpers<FieldType>.Parse(info.GetString("fieldType"));
            Operator = EnumHelpers<FieldOperator>.Parse(info.GetString("operator"));
            LookupName = info.GetString("lookupName");
        }
    }
}
