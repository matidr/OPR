using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFUserClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new UserReference.UserClient();
        }
    }
}
