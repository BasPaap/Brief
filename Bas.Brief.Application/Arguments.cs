using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief.Application
{
    record Arguments
    {
        public string BriefFileName { get; init; }
        public string Recipients { get; init; }

        public Arguments(string[] commandLineArguments)
        {
            const int numExpectedArguments = 2;
            const int briefFileNameIndex = 0;
            const int recipientsIndex = 1;

            if (commandLineArguments.Length == numExpectedArguments)
            {
                BriefFileName = commandLineArguments[briefFileNameIndex];
                Recipients = commandLineArguments[recipientsIndex];
            }
        }
    }
}
