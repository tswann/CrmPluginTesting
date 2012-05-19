using Microsoft.Xrm.Sdk;

namespace ExtensionMethods
{
    public static class Extensions
    {
        /// <summary>
        /// Extension of the CRM Entity type to set a specified attribute value
        /// </summary>
        public static void SetAttribute(this Entity entity, string key, object value)
        {
            if (entity.Attributes.Contains(key))
            {
                entity.Attributes[key] = value;
            }
            else
            {
                entity.Attributes.Add(key, value);
            }
        }
    }
}
