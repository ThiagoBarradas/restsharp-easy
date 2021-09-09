using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static RestSharp.Easy.Helper.JsonHelper;

namespace RestSharp.Easy.Helper
{
    public class EnumWithContractJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            Type type = IsNullableType(objectType)
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            return type.GetTypeInfo().IsEnum;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = IsNullableType(objectType);
            Type enumType = isNullable
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            string[] names = Enum.GetNames(enumType);

            if (reader.TokenType == JsonToken.String)
            {
                string enumText = reader.Value.ToString().ToLowerCase();

                if (!string.IsNullOrEmpty(enumText))
                {
                    string match = names
                        .Where(n => string.Equals(n.ToLowerCase(), enumText, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                    if (match != null)
                    {
                        return Enum.Parse(enumType, match);
                    }
                }
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                int enumVal = Convert.ToInt32(reader.Value);
                int[] values = (int[])Enum.GetValues(enumType);
                if (values.Contains(enumVal))
                {
                    return Enum.Parse(enumType, enumVal.ToString());
                }
            }

            if (!isNullable)
            {
                string defaultName = names
                    .Where(n => string.Equals(n, "Undefined", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (defaultName == null)
                {
                    defaultName = names.First();
                }

                return Enum.Parse(enumType, defaultName);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var finalValue = value.ToString();

            if (serializer.ContractResolver is CamelCasePropertyNamesContractResolver ||
                serializer.ContractResolver is CustomCamelCasePropertyNamesContractResolver)
            {
                finalValue = finalValue.ToCamelCase();
            }
            else if (serializer.ContractResolver is SnakeCasePropertyNamesContractResolver)
            {
                finalValue = finalValue.ToSnakeCase();
            }
            else if (serializer.ContractResolver is LowerCasePropertyNamesContractResolver)
            {
                finalValue = finalValue.ToLowerCase();
            }

            writer.WriteValue(finalValue);
        }

        private bool IsNullableType(Type t)
        {
            return (t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
