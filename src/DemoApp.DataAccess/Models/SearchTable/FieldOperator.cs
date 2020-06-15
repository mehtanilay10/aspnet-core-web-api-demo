using Newtonsoft.Json;

namespace DemoApp.DataAccess.Models.SearchTable
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum FieldOperator
    {
        Contains,
        In,
        NotIn,
        StartsWith,
        EndsWith,
        Equals,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }
}
