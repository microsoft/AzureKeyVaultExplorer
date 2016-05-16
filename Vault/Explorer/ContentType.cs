using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// See here:
    /// http://www.freeformatter.com/mime-types-list.html
    /// http://pki-tutorial.readthedocs.io/en/latest/mime.html
    /// </summary>
    public enum ContentType
    {
        [Description("(none)")]
        None = 0,
        [Description("text/plain")]
        Text,
        [Description("text/csv")]
        Csv,
        [Description("text/tab-separated-values")]
        Tsv,
        [Description("application/xml")]
        Xml,
        [Description("application/json")]
        Json,
        [Description("application/pkix-cert")]
        Certificate,
        [Description("application/x-pkcs12")]
        Pkcs12,
        [Description("application/x-pkcs12b64")]
        Pkcs12Base64,
        [Description("application/x-base64")]
        Base64,
        [Description("application/x-json-gzb64")]
        JsonGZipBase64
    }

    public class ContentTypeEnumConverter : CustomEnumTypeConverter<ContentType> { }

    public class CustomEnumTypeConverter<T> : EnumConverter where T : struct
    {
        private static readonly Dictionary<T, string> s_toString = new Dictionary<T, string>();

        private static readonly Dictionary<string, T> s_toValue = new Dictionary<string, T>(StringComparer.CurrentCultureIgnoreCase);

        static CustomEnumTypeConverter()
        {
            Debug.Assert(typeof(T).IsEnum, "The custom enum class must be used with an enum type.");
            Initialize();
        }

        public CustomEnumTypeConverter() : base(typeof(T)) { }

        internal static void Initialize()
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                string description = GetDescription(item);
                s_toString[item] = description;
                s_toValue[description] = item;
            }
        }

        public static string GetDescription(T optionValue)
        {
            var optionDescription = optionValue.ToString();
            var optionInfo = typeof(T).GetField(optionDescription);
            if (Attribute.IsDefined(optionInfo, typeof(DescriptionAttribute)))
            {
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(optionInfo, typeof(DescriptionAttribute));
                return attribute.Description;
            }
            return optionDescription;
        }

        public static T GetValue(string description)
        {
            if (!string.IsNullOrEmpty(description) && s_toValue.ContainsKey(description))
            {
                return s_toValue[description];
            }
            return default(T);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var optionValue = (T)value;

            if (destinationType == typeof(string) && s_toString.ContainsKey(optionValue))
            {
                return s_toString[optionValue];
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue) && s_toValue.ContainsKey(stringValue))
            {
                return s_toValue[stringValue];
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    public static class ContentTypeUtils
    {
        public static string FromRawValue(this ContentType contentType, string rawValue)
        {
            if (rawValue == null) return null;

            switch (contentType)
            {
                case ContentType.None:
                case ContentType.Text:
                case ContentType.Csv:
                case ContentType.Tsv:
                case ContentType.Xml:
                case ContentType.Json:
                case ContentType.Certificate:
                case ContentType.Pkcs12:
                    return rawValue;
                case ContentType.Pkcs12Base64:
                case ContentType.Base64:
                    return Encoding.UTF8.GetString(Convert.FromBase64String(rawValue));
                case ContentType.JsonGZipBase64:
                    // Decode (base64) and decompress the secret raw value
                    var decoded = Convert.FromBase64String(rawValue);
                    using (var input = new MemoryStream(decoded))
                    {
                        using (var output = new MemoryStream())
                        {
                            using (var gz = new GZipStream(input, CompressionMode.Decompress, true))
                            {
                                gz.CopyTo(output);
                            }
                            return Encoding.UTF8.GetString(output.ToArray());
                        }
                    }
                default:
                    throw new ArgumentException($"Invalid ContentType {contentType}");
            }
        }

        public static string ToRawValue(this ContentType contentType, string value)
        {
            if (value == null) return null;

            switch (contentType)
            {
                case ContentType.None:
                case ContentType.Text:
                case ContentType.Csv:
                case ContentType.Tsv:
                case ContentType.Xml:
                case ContentType.Json:
                case ContentType.Certificate:
                case ContentType.Pkcs12:
                    return value;
                case ContentType.Pkcs12Base64:
                case ContentType.Base64:
                    return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                case ContentType.JsonGZipBase64:
                    // Compress and Encode (base64) the secret value
                    using (var input = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                    {
                        using (var output = new MemoryStream())
                        {
                            using (var gz = new GZipStream(output, CompressionMode.Compress, true))
                            {
                                input.CopyTo(gz);
                            }
                            return Convert.ToBase64String(output.ToArray());
                        }
                    }
                default:
                    throw new ArgumentException($"Invalid ContentType {contentType}");
            }
        }

        public static ContentType FromExtension(string extension)
        {
            switch (extension)
            {
                case ".txt":
                    return ContentType.Text;
                case ".csv":
                    return ContentType.Csv;
                case ".tsv":
                    return ContentType.Tsv;
                case ".xml":
                    return ContentType.Xml;
                case ".json":
                    return ContentType.Json;
                case ".cer":
                case ".crt":
                    return ContentType.Certificate;
                case ".pfx":
                case ".p12":
                    return ContentType.Pkcs12;
                case ".pfxb64":
                case ".p12b64":
                    return ContentType.Pkcs12Base64;
                case ".base64":
                    return ContentType.Base64;
                case ".gzb64":
                    return ContentType.JsonGZipBase64;
                default:
                    return ContentType.None;                
            }
        }

        public static string ToSyntaxHighlightingMode(this ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.None:
                case ContentType.Text:
                case ContentType.Csv:
                case ContentType.Tsv:
                    return "HTML";
                case ContentType.Xml:
                    return "XML";
                case ContentType.Json:
                case ContentType.Certificate:
                case ContentType.Pkcs12:
                case ContentType.Pkcs12Base64:
                case ContentType.JsonGZipBase64:
                    return "JavaScript";
                case ContentType.Base64:
                    return "HTML";
                default:
                    throw new ArgumentException($"Invalid ContentType {contentType}");
            }
        }
    }
}
