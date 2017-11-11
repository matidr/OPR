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
            //TODO change this method to call the delete on the old server through remoting
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetUsers()
        {
            //TODO change this method to call the getusers on the old server through remoting
            return users;
        }

        public bool ModifyUser(User user)
        {
            //TODO change this method to call the modify on the old server through remoting
            throw new NotImplementedException();
        }

        public bool SaveUser(User user)
        {
            //TODO change this method to call the add on the old server through remoting
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
