using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [DataContract]
    public class ChatMessage
    {
        User theUser;
        String theMessage;
        String display;
        public ChatMessage(String username, String msg, Context myContext)
        {
            Display = "";
            theUser = myContext.ExistingUsers.Find(x => x.Username.Equals(username));
            theMessage = msg;
        }
        public ChatMessage(String disp)
        {
            theUser = new User();
            theMessage = "";
            Display = disp;
        }
        [DataMember]
        public User TheUser { get => theUser; set => theUser = value; }
        [DataMember]
        public string TheMessage { get => theMessage; set => theMessage = value; }
        [DataMember]
        public string Display { get => display; set => display = value; }

        public override bool Equals(object value)
        {
            ChatMessage msg = value as ChatMessage;
            if (msg.Display == "")
            {
                return (msg != null) && (msg.theMessage == theMessage) && (msg.theUser == theUser);
            }
            else
            {
                return (msg != null) && (msg.Display == Display);
            }
        }
    }
}
