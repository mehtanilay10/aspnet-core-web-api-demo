using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DemoApp.DataAccess.Models.SearchTable
{
    public class SearchTableResponse<T> : ISerializable
    {
        public int TotalRecords { get; set; }

        public List<T> Data { get; set; }

        public bool Status { get; set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("recordsTotal", TotalRecords);
            info.AddValue("Status", Status);
            info.AddValue("data", Data);
        }
    }
}
