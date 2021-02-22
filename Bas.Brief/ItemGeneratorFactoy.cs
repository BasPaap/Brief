using Bas.Brief.ItemGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bas.Brief
{
    static class ItemGeneratorFactoy
    {
        internal static ItemGenerator GetItemGenerator(string name, IEnumerable<KeyValuePair<string, string>> parameters, string content)
        {
            var assemblies = new List<Assembly>
            {
                typeof(ItemGenerator).Assembly // The assembly containing the built in item generators.
            };
            // TODO: load any other third party assemblies 

            Type generatorType = null;

            foreach (var assembly in assemblies)
            {
                generatorType = assembly.GetTypes().FirstOrDefault(t => t.Name == $"{name}Generator");

                if (generatorType != null)
                {
                    break;
                }
            }

            if (generatorType != null)
            {
                var itemGenerator = Activator.CreateInstance(generatorType, parameters, content) as ItemGenerator;

                return itemGenerator;
            }
            else
            {
                return null;
            }
        }
    }
}
