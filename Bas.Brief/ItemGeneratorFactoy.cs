using Bas.Brief.ItemGenerators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bas.Brief
{
    static class ItemGeneratorFactoy
    {
        internal static ItemGenerator GetItemGenerator(string name, IEnumerable<KeyValuePair<string, string>> parameters, string content, CultureInfo culture)
        {
            Type generatorType = GetGeneratorType(name);

            if (generatorType == null)
            {
                return null;
            }

            // Instantiate the found type using the ItemGenerator constructor and return it.
            var itemGenerator = Activator.CreateInstance(generatorType, parameters, content, culture) as ItemGenerator;
            return itemGenerator;
        }

        private static Type GetGeneratorType(string name)
        {
            var assemblies = new List<Assembly>
            {
                typeof(ItemGenerator).Assembly // The assembly containing the built in item generators.
            };
            // TODO: load any other third party assemblies 

            // Go through all assemblies and try to find a type with the right name. As soon as one it found, return it.
            Type generatorType = null;
            
            foreach (var assembly in assemblies)
            {
                generatorType = assembly.GetTypes().FirstOrDefault(t => t.Name == $"{name}Generator");

                if (generatorType != null)
                {
                    break;
                }
            }

            return generatorType;
        }
    }
}
