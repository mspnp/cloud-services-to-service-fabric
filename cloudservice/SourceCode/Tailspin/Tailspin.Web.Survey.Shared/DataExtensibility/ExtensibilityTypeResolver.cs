namespace Tailspin.Web.Survey.Shared.DataExtensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Tailspin.Web.Survey.Extensibility;
    using Tailspin.Web.Survey.Shared.Helpers;

    public static class ExtensibilityTypeResolver
    {
        private static IDictionary<string, Assembly> loadedAssemblies;

        static ExtensibilityTypeResolver()
        {
            loadedAssemblies = new Dictionary<string, Assembly>();
        }

        public static Type GetTypeFrom(string assemblyFileName, string @namespace, Type mainType)
        {
            try
            {
                Assembly assembly;
                if (!loadedAssemblies.TryGetValue(assemblyFileName, out assembly))
                {
                    assembly = Assembly.LoadFrom(assemblyFileName);
                    loadedAssemblies[assemblyFileName] = assembly;
                }

                var result = assembly.GetType(string.Format("{0}.{1}Extension", @namespace, mainType.Name.Split('.').Last()));

                if (!result.GetInterfaces().Contains(typeof(IModelExtension)))
                {
                    throw new NotSupportedException(string.Format("Extension type '{0}' should implement IModelExtension.", result.Name));
                }

                return result;
            }
            catch (Exception e)
            {
                TraceHelper.TraceError("Error getting extension for type {0} - Details {1}", mainType.Name, e.Message);
                throw;
            }
        }

        public static IModelExtension GetInstanceFrom(string assemblyFileName, string @namespace, Type mainType)
        {
            try
            {
                return Activator.CreateInstance(GetTypeFrom(assemblyFileName, @namespace, mainType)) as IModelExtension;
            }
            catch (Exception e)
            {
                TraceHelper.TraceError("Error instancing extension for type {0} - Details {1}", mainType.Name, e.Message);
                throw;
            }
        }
    }
}
