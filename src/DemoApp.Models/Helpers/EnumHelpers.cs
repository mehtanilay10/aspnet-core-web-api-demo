using System;
using System.Collections.Generic;
using System.Linq;
using DemoApp.Models.ApiModels.Common;
using DemoApp.Models.ExtensionMethods;

namespace DemoApp.Models.Helpers
{
    public static class EnumHelpers<T> where T : Enum
    {
        public class NotAnEnumException : Exception
        {
            public NotAnEnumException() : base(string.Format(@"Type ""{0}"" is not an Enum type.", typeof(T))) { }
        }

        static EnumHelpers()
        {
            if (typeof(T).BaseType != typeof(Enum)) throw new NotAnEnumException();
        }

        public static IEnumerable<T> GetValues()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static IEnumerable<EnumDetails> GetEnumDetails(bool useEnumDescription = true)
        {
            if (useEnumDescription)
            {
                return Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Select(x => new EnumDetails { Id = x.ToString(), Name = x.GetEnumDescription() });
            }
            else
            {
                return Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Select(x => new EnumDetails { Id = Convert.ToInt32(x).ToString(), Name = x.ToString() });
            }
        }
    }
}
