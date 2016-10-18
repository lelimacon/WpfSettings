using System.IO;
using System.Reflection;

namespace WpfSettings.Utils.Reflection
{
    internal class ResourceUtils
    {
        internal static Stream FromParentAssembly(string resourceName)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            string name = assembly.GetName().Name;
            string resourcePath = $"{name}.{resourceName}";
            Stream resourceStream = assembly.GetManifestResourceStream(resourcePath);
            return resourceStream;
        }
    }
}
