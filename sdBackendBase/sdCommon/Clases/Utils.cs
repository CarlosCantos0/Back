using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace sdCommon.Clases
{
    public static class Utils
    {
        // Asignar las propiedades del objeto origen de tipo <T> sobre el objecto destino que normalmente será una instancia de una clase heredada de <T>         
        public static void AssignProperties<T>(object origen, object destino) where T: class
        {
            if (origen != null && destino != null)
            {
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(destino, property.GetValue(origen, null), null);
                    }
                }
            }
        }
        // Convert un IEnumerable<dynamic> en su representacion JSON
        public static T ConvertToType<T>(this object value) where T : class
        {
            var jsonData = JsonConvert.SerializeObject(value);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
