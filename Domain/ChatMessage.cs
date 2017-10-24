using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
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
        public User TheUser { get => theUser; set => theUser = value; }
        public string TheMessage { get => theMessage; set => theMessage = value; }
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
