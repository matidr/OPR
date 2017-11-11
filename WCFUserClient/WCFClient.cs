using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFUserClient
{
    class WCFClient
    {
        static void Main(string[] args)
        {
            WCFClientOperations wcfClientOperations = new WCFClientOperations();
            wcfClientOperations.MainMenu();
        }
    }
}
