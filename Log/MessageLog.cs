using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    public class MessageLog
    {
        private string queueName;
        private MessageQueue myQ;

        public MessageLog()
        {
            queueName = @".\private$\logqueue";
        }
       
        public void SendMessageLog(string messageToSend)
        {
            if (MessageQueue.Exists(queueName))
            {
                using (myQ = new MessageQueue(queueName))
                {
                    Message msg = new Message(messageToSend);
                    //myQ.Send(msg);
                    myQ.Send(msg, "Logged Action");
                }
            }
            else
            {
                using (myQ = MessageQueue.Create(queueName))
                {
                    Message msg = new Message(messageToSend);
                    //myQ.Send(msg);
                    myQ.Send(msg, "Logged Action");
                }
            }
        }


        public List<string> ReceiveMessages()
        {
            List<string> lstMessages = new List<string>();
            using (MessageQueue messageQueue = new MessageQueue(queueName))
            {
                System.Messaging.Message[] messages = messageQueue.GetAllMessages();
                foreach (System.Messaging.Message message in messages)
                {
                    message.Formatter = new XmlMessageFormatter(
                    new String[] { "System.String, mscorlib" });
                    string msg = message.Body.ToString();
                    lstMessages.Add(msg);
                }
            }
            return lstMessages;
        }
    }
}
