using System;

namespace DemoApp.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumDescriptionAttribute : Attribute, IAttribute<string>
    {
        public EnumDescriptionAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; }
    }
}
