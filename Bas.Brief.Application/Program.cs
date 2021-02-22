using System;
using System.Threading.Tasks;

namespace Bas.Brief.Application
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var arguments = new Arguments(args);
            await BriefSender.SendAsync(arguments.BriefFileName, arguments.Recipients);
        }
    }
}
