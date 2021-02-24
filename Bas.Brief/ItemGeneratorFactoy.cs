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
        private static Dictionary<string, Type> itemGeneratorTypes;

        internal static ItemGenerator GetItemGenerator(string name, IEnumerable<KeyValuePair<string, string>> parameters, string content, CultureInfo culture)
        {
            if (itemGeneratorTypes == null || itemGeneratorTypes.Count == 0)
            {
                itemGeneratorTypes = GetItemGeneratorTypes();
            }

            if (!itemGeneratorTypes.ContainsKey(name))  
            {
                return null;
            }

            // Instantiate the found type using the ItemGenerator constructor and return it.
            var itemGenerator = Activator.CreateInstance(itemGeneratorTypes[name], parameters, content, culture) as ItemGenerator;
            return itemGenerator;
        }

        private static Dictionary<string, Type> GetItemGeneratorTypes()
        {
            var itemGeneratorTypes = new Dictionary<string, Type>();

            var assemblies = new List<Assembly>
            {
                typeof(ItemGenerator).Assembly // The assembly containing the built in item generators.
            };
            // TODO: load any other third party assemblies 

            var types = assemblies.AsParallel()
                .SelectMany(a => a.GetExportedTypes())
                .Where(type => type.GetCustomAttributes(typeof(ItemGeneratorAttribute)).Any()).ToList();

            foreach (var itemGeneratorType in types)
            {
                var itemGeneratorAttribute = itemGeneratorType.GetCustomAttribute(typeof(ItemGeneratorAttribute)) as ItemGeneratorAttribute;

                if (!itemGeneratorTypes.ContainsKey(itemGeneratorAttribute.Name))
                {
                    itemGeneratorTypes.Add(itemGeneratorAttribute.Name, itemGeneratorType);
                }
            }

            return itemGeneratorTypes;
        }        
    }
}
