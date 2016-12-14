namespace Tailspin.Web.Survey.Shared.DataExtensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ExtensibilityTypeMapper
    {
        private static IList<string> excludedProperties;
        private static IDictionary<Type, Func<string, object>> conversionDictionary;

        static ExtensibilityTypeMapper()
        {
            excludedProperties = new List<string>() { "PartitionKey", "RowKey", "Timestamp", "ParentModelKey" };
            conversionDictionary = new Dictionary<Type, Func<string, object>> 
            {
                { typeof(int), s => Convert.ToInt32(s) },
                { typeof(double), s => Convert.ToDouble(s) },
                { typeof(decimal), s => Convert.ToDecimal(s) },
                { typeof(DateTime), s => Convert.ToDateTime(s) },
                { typeof(Guid), s => Guid.Parse(s) }
            };
        }

        public static IEnumerable<ModelExtensionItem> GetModelExtensionProperties(object instance)
        {
            return instance
                .GetType()
                .GetProperties()
                .Where(p => !excludedProperties.Contains(p.Name))
                .Select(p => new ModelExtensionItem() { PropertyName = p.Name, PropertyValue = Convert.ToString(p.GetValue(instance, null)) });
        }

        public static void SetModelExtensionProperties(object instance, IEnumerable<ModelExtensionItem> values)
        {
            instance
                .GetType()
                .GetProperties()
                .Where(p => !excludedProperties.Contains(p.Name))
                .ToList()
                .ForEach(p => p.SetValue(instance, GetTypedValue(p, values), null));
        }

        private static object GetTypedValue(PropertyInfo property, IEnumerable<ModelExtensionItem> values)
        {
            var value = values.Where(em => em.PropertyName.Equals(property.Name)).FirstOrDefault();

            if (value == null)
            {
                return null;
            }

            Func<string, object> converter;
            if (conversionDictionary.TryGetValue(property.PropertyType, out converter))
            {
                return converter(value.PropertyValue);
            }

            return value.PropertyValue;
        }
    }
}
