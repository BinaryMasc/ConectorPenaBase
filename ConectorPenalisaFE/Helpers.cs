using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConectorPenalisaFE
{
    public class Helpers
    {
        public static string GetExceptionDetails(Exception exception)
        {
            PropertyInfo[] properties = exception.GetType().GetProperties();
            List<string> fields = new List<string>();
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(exception, null);
                fields.Add(String.Format("{0} = {1}", property.Name, value != null ? value.ToString() : String.Empty));
            }
            return String.Join("\r\n", fields.ToArray());
        }
    }
}
