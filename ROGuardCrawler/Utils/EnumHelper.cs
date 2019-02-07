using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROGuardCrawler.Utils
{
    public static class EnumHelper
    {
        public static T ParseEnum<T>(string value) where T : struct, IConvertible 
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception($"Provided type {typeof(T).Name} is not Enum!");
            }

            if (Enum.TryParse<T>(value, out var result))
            {
                return result;
            }

            Console.WriteLine($"Value {value} not found in enum {typeof(T).Name}");
            return default(T);
        }
    }
}
