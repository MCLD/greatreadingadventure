using System;
using System.Collections.Generic;
using System.Linq;
using GRA.Domain.Model;

namespace GRA
{
    public static class ObjectExtensions
    {
        public static ICollection<ObjectDifference> DetailedCompare<T>(this T object1, T object2)
        {
            return DetailedCompare(object1, object2, null);
        }

        public static ICollection<ObjectDifference> DetailedCompare<T>(this T object1,
            T object2,
            string[] properties)
        {
            var differences = new List<ObjectDifference>();
            var fieldInfos = typeof(T).GetProperties();
            var propsSpecified = properties?.Length > 0;
            foreach (var fieldInfo in fieldInfos)
            {
                if (!propsSpecified || properties.Contains(fieldInfo.Name))
                {
                    var diff = new ObjectDifference
                    {
                        Property = fieldInfo.Name,
                        Value1 = fieldInfo.GetValue(object1),
                        Value2 = fieldInfo.GetValue(object2)
                    };
                    if (!Equals(diff.Value1, diff.Value2))
                    {
                        differences.Add(diff);
                    }
                }
            }
            return differences;
        }
    }
}
