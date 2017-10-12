using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace Log
{
    class Program
    {
        static void Main(string[] args)
        {
            string queueName = @".\private$\logqueue";
            MessageQueue myQ;
            if (MessageQueue.Exists(queueName))
            {
                using (myQ = new MessageQueue(queueName))
                {
                    Message msg = new Message("Hola, MSMQ!");
                    myQ.Send(msg);
                    myQ.Send(msg, "Saludos");

                    myQ.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                    // recibe el msg sin sacarlo de la cola
                    //Message msgR = myQ.Peek();
                    Message msg2 = myQ.Receive();
                    Console.WriteLine((string)(msg2.Body));
                    Console.ReadLine();

                }
            }
            else
            {
                using (myQ = MessageQueue.Create(queueName))
                {

                }
            }
        }
    }
}
