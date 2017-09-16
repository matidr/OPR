using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Message
    {
        User theUser;
        String theMessage;

        public User TheUser { get => theUser; set => theUser = value; }
        public string TheMessage { get => theMessage; set => theMessage = value; }
    }
}
