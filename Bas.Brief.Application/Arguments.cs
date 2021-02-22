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
        public string RecipientName { get; init; }
        public string Recipients { get; init; }

        public Arguments(string[] commandLineArguments)
        {
            const int numExpectedArguments = 3;
            const int briefFileNameIndex = 0;
            const int recipientNameIndex = 1;
            const int recipientsIndex = 2;

            if (commandLineArguments.Length == numExpectedArguments)
            {
                BriefFileName = commandLineArguments[briefFileNameIndex];
                RecipientName = commandLineArguments[recipientNameIndex];
                Recipients = commandLineArguments[recipientsIndex];
            }
        }
    }
}
