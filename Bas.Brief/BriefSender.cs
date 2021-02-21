using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief
{
    public static class BriefSender
    {
        public static void Send(string briefFileName, string recipients)
        {
            var configuration = new BriefConfiguration();
            configuration.Load(briefFileName);

            throw new NotImplementedException();
        }
    }
}
