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
        internal static ItemGenerator GetItemGenerator(string name, IEnumerable<KeyValuePair<string, string>> parameters, string content, bool isFirst, bool isLast)
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(typeof(ItemGenerator).Assembly); // The assembly containing the built in item generators.
            // TODO: load any other third party assemblies 

            Type generatorType = null;

            foreach (var assembly in assemblies)
            {
                generatorType = assembly.GetType($"{name}Generator", false);

                if (generatorType != null)
                {
                    break;
                }
            }

            var itemGenerator = Activator.CreateInstance(generatorType, parameters, content, isFirst, isLast) as ItemGenerator;

            return itemGenerator;
        }
    }
}
