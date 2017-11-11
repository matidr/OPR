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

        public bool DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetUsers()
        {
            return users;
        }

        public bool ModifyUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool SaveUser(User user)
        {
            if (users != null)
            {
                if (!users.Contains(user))
                {
                    users.Add(user);
                    return true;
                }
            }
            return false;
        }
    }
}
