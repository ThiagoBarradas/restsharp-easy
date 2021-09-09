using System;
using System.Reflection;

namespace RestSharp.Easy.Helper
{
    public static class EnumHelper
    {
        public static T ConvertToEnum<T>(this object enumToConvert)
        {
            if (enumToConvert == null)
            {
                return default(T);
            }

            if (!typeof(T).GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type.");
            }

            T convertedEnum = default(T);

            try
            {
                convertedEnum = (T)Enum.Parse(typeof(T), enumToConvert.ToString(), true);
            }
            catch (Exception)
            {
                // keep default enum if cant possible cast
            }

            return convertedEnum;
        }
    }
}
