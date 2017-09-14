using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Context
    {
        private List<User> existingUsers;

        public List<User> ExistingUsers { get => existingUsers; set => existingUsers = value; }

        public bool userExist(string user)
        {
            foreach (User u in existingUsers)
            {
                if (u.Username == user)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        public bool correctPassword(string user, string password)
        {
            foreach (User u in existingUsers)
            {
                if (u.Username == user && u.Password == password)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        //agregar usuario


    }
}
