using System.Text.Json.Serialization;
using DemoApp.Models.Attributes;

namespace DemoApp.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SupportedCulture
    {
        [EnumDescription("en")]
        en = 0,

        [EnumDescription("hi")]
        hi = 1,

        [EnumDescription("fr")]
        fr = 2
    }
}
