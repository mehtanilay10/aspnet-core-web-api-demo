using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoApp.Models.ExtensionMethods
{
    public static class DataTableExtensionMethods
    {
        public static Dictionary<string, dynamic> GetDictionaryForFirstRow(this DataTable dataTable)
        {
            Dictionary<string, dynamic> returnData = new Dictionary<string, dynamic>();

            if (dataTable?.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                returnData = dataTable.Columns
                                .Cast<DataColumn>()
                                .ToDictionary(c => c.ColumnName.ToCamelCase(), c => row[c]);
            }

            return returnData;
        }
    }
}
