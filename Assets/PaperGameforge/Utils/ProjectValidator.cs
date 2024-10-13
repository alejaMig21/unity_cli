using System;
using System.Linq;
using System.Reflection;

namespace Assets.PaperGameforge.Utils
{
    public static class ProjectValidator
    {
        public static bool NamespaceExists(string namespaceName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => t.Namespace != null && t.Namespace.StartsWith(namespaceName));
                if (types.Any())
                {
                    return true;
                }
            }

            return false;
        }
        public static bool ClassExists(string className)
        {
            var type = Type.GetType(className);
            return type != null;
        }
        public static bool MethodExists(string className, string methodName)
        {
            var type = Type.GetType(className);
            if (type == null)
            {
                return false;
            }

            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return method != null;
        }
        public static bool PropertyExists(string className, string propertyName)
        {
            var type = Type.GetType(className);
            if (type == null)
            {
                return false;
            }

            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return property != null;
        }
        public static bool FieldExists(string className, string fieldName)
        {
            var type = Type.GetType(className);
            if (type == null)
            {
                return false;
            }

            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return field != null;

        }
    }
}