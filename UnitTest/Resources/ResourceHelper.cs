using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Gelf4NLog.UnitTest.Resources
{
    internal class ResourceHelper
    {
        internal static TextReader GetResource(string filename)
        {
            Assert.IsNotNull(filename);
            var thisAssembly = Assembly.GetExecutingAssembly();
            var resourceFullName = typeof (ResourceHelper).Namespace + "." + filename;
            var manifestResourceStream = thisAssembly.GetManifestResourceStream(resourceFullName);
            Assert.IsNotNull(manifestResourceStream, "Resource not found in this assembly: " + resourceFullName);

            return new StreamReader(manifestResourceStream);
        }
    }
}
