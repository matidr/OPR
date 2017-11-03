using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFLibrary;

namespace WCFConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(UserService)))
            {
                Console.WriteLine("Service started");
                host.Open();
                Console.ReadLine();
            }
        }
    }
}
