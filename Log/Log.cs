using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    class Log
    {
        static void Main(string[] args)
        {
            while (true)
            {
                MessageLog theMessageLog = new MessageLog();
                List<String> logs = theMessageLog.ReceiveMessages();
                if (logs.Count > 0)
                {
                    foreach (String log in logs)
                    {
                        Console.WriteLine(log);
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
