using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Domain;

namespace WCFLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class UserService : IUser
    {
        private static List<User> users = new List<User>();

        public IEnumerable<User> GetUsers()
        {
            return users;
        }

        public void SaveUser(User user)
        {
            if (users != null)
            {
                users.Add(user);
            }
        }
    }
}
