using System;
using System.Linq;
using System.Reflection;
using DemoApp.Models.Attributes;

namespace DemoApp.Models.ExtensionMethods
{
    public static class EnumDescriptionExtensionMethod
    {
        public static string GetEnumDescription(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            string description = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(description);
            EnumDescriptionAttribute[] attributes = (EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                description = attributes.FirstOrDefault().Value;

            return description;
        }
    }
}
