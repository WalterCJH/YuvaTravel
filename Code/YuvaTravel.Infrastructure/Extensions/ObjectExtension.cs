using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Infrastructure.Extensions
{
    public static class ObjectExtension
    {
        public static T GetValue<T>(this object obj, string name)
        {
            return (T)obj.GetType().GetProperty(name)?.GetValue(obj, null);
        }

        public static void SetValue(this object obj, string name, object value)
        {
            obj.GetType().GetProperty(name)?.SetValue(obj, value);
        }

        public static IDictionary<string, object> ToDictionary(this object source)
        {
            if (source == null)
            {
                return null;
            }
            var dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                object value = property.GetValue(source);
                if (!dictionary.ContainsKey(property.Name))
                {
                    dictionary.Add(property.Name, value);
                }
            }
            return dictionary;
        }
    }
}
