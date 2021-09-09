using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace RestSharp.Easy.Helper
{
    public static class JsonHelper
    {
        private readonly static object Lock = new object();

        public static List<JsonConverter> DefaultConverters = new List<JsonConverter>
        {
            new EnumWithContractJsonConverter(),
            new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffff"
            }
        };

        public static JsonSerializerSettings GetNewtonsoftJsonSerializerSettings(this string strategy)
        {
            strategy = strategy?.ToLowerInvariant().Trim();

            switch (strategy)
            {
                case "snake":
                case "snakecase":
                    return JsonHelper.SnakeCaseJsonSerializerSettings;
                case "camel":
                case "camelcase":
                    return JsonHelper.CamelCaseJsonSerializerSettings;
                case "lower":
                case "lowercase":
                    return JsonHelper.LowerCaseJsonSerializerSettings;
                default:
                    return JsonHelper.OriginalCaseJsonSerializerSettings;
            }
        }

        public static JsonSerializer GetNewtonsoftJsonSerializer(this string strategy)
        {
            strategy = strategy?.ToLowerInvariant().Trim();

            switch (strategy)
            {
                case "snake":
                case "snakecase":
                    return JsonHelper.SnakeCaseJsonSerializer;
                case "camel":
                case "camelcase":
                    return JsonHelper.CamelCaseJsonSerializer;
                case "lower":
                case "lowercase":
                    return JsonHelper.LowerCaseJsonSerializer;
                default:
                    return JsonHelper.OriginalCaseJsonSerializer;
            }
        }

        private static JsonSerializerSettings SnakeCaseJsonSerializerSettings
        {
            get
            {
                if (_snakeCaseJsonSerializerSettings == null)
                {
                    lock (Lock)
                    {
                        if (_snakeCaseJsonSerializerSettings == null)
                        {
                            var settings = new JsonSerializerSettings();

                            settings.ContractResolver = new SnakeCasePropertyNamesContractResolver();
                            DefaultConverters.ForEach(c => settings.Converters.Add(c));
                            settings.NullValueHandling = NullValueHandling.Ignore;

                            _snakeCaseJsonSerializerSettings = settings;
                        }
                    }
                }

                return _snakeCaseJsonSerializerSettings;
            }
        }

        private static JsonSerializerSettings _snakeCaseJsonSerializerSettings;

        public static JsonSerializerSettings CamelCaseJsonSerializerSettings
        {
            get
            {
                if (_camelCaseJsonSerializerSettings == null)
                {
                    lock (Lock)
                    {
                        if (_camelCaseJsonSerializerSettings == null)
                        {
                            var settings = new JsonSerializerSettings();

                            settings.ContractResolver = new CustomCamelCasePropertyNamesContractResolver();
                            DefaultConverters.ForEach(c => settings.Converters.Add(c));
                            settings.NullValueHandling = NullValueHandling.Ignore;

                            _camelCaseJsonSerializerSettings = settings;
                        }
                    }
                }

                return _camelCaseJsonSerializerSettings;
            }
        }

        public static JsonSerializerSettings _camelCaseJsonSerializerSettings;

        public static JsonSerializerSettings LowerCaseJsonSerializerSettings
        {
            get
            {
                if (_lowerCaseJsonSerializerSettings == null)
                {
                    lock (Lock)
                    {
                        if (_lowerCaseJsonSerializerSettings == null)
                        {
                            var settings = new JsonSerializerSettings();

                            settings.ContractResolver = new LowerCasePropertyNamesContractResolver();
                            DefaultConverters.ForEach(c => settings.Converters.Add(c));
                            settings.NullValueHandling = NullValueHandling.Ignore;

                            _lowerCaseJsonSerializerSettings = settings;
                        }
                    }
                }


                return _lowerCaseJsonSerializerSettings;
            }
        }

        private static JsonSerializerSettings _lowerCaseJsonSerializerSettings;

        public static JsonSerializerSettings OriginalCaseJsonSerializerSettings
        {
            get
            {
                if (_originalCaseJsonSerializerSettings == null)
                {
                    lock (Lock)
                    {
                        if (_originalCaseJsonSerializerSettings == null)
                        {
                            var settings = new JsonSerializerSettings();

                            settings.ContractResolver = new OriginalCasePropertyNamesContractResolver();
                            DefaultConverters.ForEach(c => settings.Converters.Add(c));
                            settings.NullValueHandling = NullValueHandling.Ignore;

                            _originalCaseJsonSerializerSettings = settings;
                        }
                    }
                }

                return _originalCaseJsonSerializerSettings;
            }
        }

        private static JsonSerializerSettings _originalCaseJsonSerializerSettings;

        public static JsonSerializer CamelCaseJsonSerializer
        {
            get
            {
                if (_camelCaseJsonSerializer == null)
                {
                    lock (Lock)
                    {

                        if (_camelCaseJsonSerializer == null)
                        {
                            var serializer = new JsonSerializer();
                            serializer.NullValueHandling = NullValueHandling.Ignore;
                            serializer.ContractResolver = new CustomCamelCasePropertyNamesContractResolver();
                            DefaultConverters.ForEach(c => serializer.Converters.Add(c));

                            _camelCaseJsonSerializer = serializer;
                        }
                    }
                }
                return _camelCaseJsonSerializer;
            }
        }

        private static JsonSerializer _camelCaseJsonSerializer;

        public static JsonSerializer SnakeCaseJsonSerializer
        {
            get
            {
                if (_snakeCaseJsonSerializer == null)
                {
                    lock (Lock)
                    {
                        if (_snakeCaseJsonSerializer == null)
                        {
                            var serializer = new JsonSerializer();
                            serializer.NullValueHandling = NullValueHandling.Ignore;
                            serializer.ContractResolver = new SnakeCasePropertyNamesContractResolver();
                            DefaultConverters.ForEach(c => serializer.Converters.Add(c));

                            _snakeCaseJsonSerializer = serializer;
                        }
                    }
                }

                return _snakeCaseJsonSerializer;
            }
        }

        private static JsonSerializer _snakeCaseJsonSerializer;

        public static JsonSerializer LowerCaseJsonSerializer
        {
            get
            {
                if (_lowerCaseJsonSerializer == null)
                {
                    lock (Lock)
                    {
                        if (_lowerCaseJsonSerializer == null)
                        {
                            var serializer = new JsonSerializer();
                            serializer.NullValueHandling = NullValueHandling.Ignore;
                            serializer.ContractResolver = new LowerCasePropertyNamesContractResolver();
                            DefaultConverters.ForEach(c => serializer.Converters.Add(c));
                            _lowerCaseJsonSerializer = serializer;
                        }
                    }
                }

                return _lowerCaseJsonSerializer;
            }
        }

        private static JsonSerializer _lowerCaseJsonSerializer;

        public static JsonSerializer OriginalCaseJsonSerializer
        {
            get
            {
                if (_originalCaseJsonSerializer == null)
                {
                    lock (Lock)
                    {
                        if (_originalCaseJsonSerializer == null)
                        {
                            var serializer = new JsonSerializer();
                            serializer.NullValueHandling = NullValueHandling.Ignore;
                            serializer.ContractResolver = new OriginalCasePropertyNamesContractResolver();
                            DefaultConverters.ForEach(c => serializer.Converters.Add(c));

                            _originalCaseJsonSerializer = serializer;
                        }
                    }
                }

                return _originalCaseJsonSerializer;
            }
        }

        private static JsonSerializer _originalCaseJsonSerializer;

        public class LowerCaseNamingResolver : NamingStrategy
        {
            public LowerCaseNamingResolver()
            {
                this.ProcessDictionaryKeys = true;
                this.OverrideSpecifiedNames = true;
            }

            protected override string ResolvePropertyName(string name)
            {
                return name.ToLowerInvariant();
            }
        }

        public class LowerCasePropertyNamesContractResolver : DefaultContractResolver
        {
            public LowerCasePropertyNamesContractResolver()
            {
                this.NamingStrategy = new LowerCaseNamingResolver
                {
                    ProcessDictionaryKeys = true,
                    OverrideSpecifiedNames = true
                };
            }
        }

        public class OriginalCaseNamingResolver : NamingStrategy
        {
            public OriginalCaseNamingResolver()
            {
                this.ProcessDictionaryKeys = true;
                this.OverrideSpecifiedNames = true;
            }

            protected override string ResolvePropertyName(string name)
            {
                return name;
            }
        }

        public class OriginalCasePropertyNamesContractResolver : DefaultContractResolver
        {
            public OriginalCasePropertyNamesContractResolver()
            {
                this.NamingStrategy = new OriginalCaseNamingResolver
                {
                    ProcessDictionaryKeys = true,
                    OverrideSpecifiedNames = true
                };
            }
        }

        public class SnakeCasePropertyNamesContractResolver : DefaultContractResolver
        {
            public SnakeCasePropertyNamesContractResolver()
            {
                this.NamingStrategy = new SnakeCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true,
                    OverrideSpecifiedNames = true
                };
            }
        }

        public class CustomCamelCasePropertyNamesContractResolver : DefaultContractResolver
        {
            public CustomCamelCasePropertyNamesContractResolver()
            {
                this.NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true,
                    OverrideSpecifiedNames = true
                };
            }
        }
    }
}