using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DemoApp.DataAccess.Models.SearchTable
{
    public class SearchTableRequest
    {
        #region Page details

        [JsonProperty("pageNo")]
        public int PageNo { get; set; }

        [JsonProperty("pageLength")]
        public int PageLength { get; set; }

        [JsonIgnore]
        public int PageStart
        {
            get
            {
                return (PageNo > 1)
                    ? (PageLength * (PageNo - 1))
                    : 0;
            }
        }

        #endregion

        #region Parameters

        [JsonProperty("filterParas")]
        public List<SearchTableFilter> FilterParas { get; set; }

        [JsonProperty("sortParas")]
        public SearchTableSort SortParas { get; set; }

        [JsonProperty("params")]
        public Dictionary<string, string> ExtraParams { get; set; }

        #endregion

        #region Constructor

        public SearchTableRequest() { }

        public SearchTableRequest(SerializationInfo info, StreamingContext context)
        {
            ExtraParams = new Dictionary<string, string>();
            PageNo = info.GetInt32("start");
            PageLength = info.GetInt32("length");
            FilterParas = (List<SearchTableFilter>)info.GetValue("filterParas", typeof(List<SearchTableFilter>));
            SortParas = (SearchTableSort)info.GetValue("sortParas", typeof(SearchTableSort));
            ExtraParams = (Dictionary<string, string>)info.GetValue("params", typeof(Dictionary<string, string>));
        }

        #endregion
    }
}
