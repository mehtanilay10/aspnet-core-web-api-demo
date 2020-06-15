using DemoApp.DataAccess.ExtensionMethods;

namespace DemoApp.DataAccess.Models
{
    public class Condition
    {
        public OperatorComparer Operator { get; set; }
        public string FieldName { get; set; }
        public object Value { get; set; }
    }
}
