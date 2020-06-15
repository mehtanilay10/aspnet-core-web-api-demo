namespace DemoApp.DataAccess.Models.SearchTable
{
    public class FieldCondition
    {
        public FieldOperator Operator { get; set; } = FieldOperator.Contains;
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; } = FieldType.Text;
        public object Value { get; set; }
    }
}
