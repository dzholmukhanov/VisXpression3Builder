using Newtonsoft.Json;
using System;
using System.Linq;

namespace VisXpression3Builder.Lib.Constants
{
    public static class DataTypes
    {
        public const string Number = "Number";
        public const string Boolean = "Boolean";
        public const string NumberArray = "NumberArray";

        public static Type GetType(string type)
        {
            switch (type)
            {
                case Boolean:
                    return typeof(bool?);
                case Number:
                    return typeof(double?);
                case NumberArray:
                    return typeof(double?[]);
            }
            return null;
        }

        public static string GetSystemType(Type type)
        {
            if (type == typeof(bool?)) return Boolean;
            if (type == typeof(double?)) return Number;
            if (type == typeof(double?[])) return NumberArray;

            return null;
        }

        public static bool IsValidType(string type)
        {
            return typeof(DataTypes).GetFields().Any(f => f.Name == type);
        }

        public static dynamic Parse(string value, string type)
        {
            if (!IsValidType(type)) throw new ArgumentException("Cannot parse because of invalid type.");

            switch (type)
            {
                case Boolean:
                    return (bool?)Convert.ToBoolean(value);
                case Number:
                    return (double?)Convert.ToDouble(value);
                case NumberArray:
                    return JsonConvert.DeserializeObject<double?[]>(value);
            }
            return null;
        }
    }
}
