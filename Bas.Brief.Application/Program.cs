using System;

namespace Bas.Brief.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new Arguments(args);
            BriefSender.Send(arguments.BriefFileName, arguments.Recipients);
        }
    }
}
