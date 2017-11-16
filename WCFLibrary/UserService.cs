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
        IUserService.IUserService userClient;

        public void Inicialization()
        {
            userClient = (IUserService.IUserService)Activator.GetObject
                (typeof(IUserService.IUserService),
                "tcp://localhost:5000/UserOperations");

        }

        [STAThread]
        public bool DeleteUser(User user)
        {
            Inicialization();
            if (user != null)
            {
                string username = user.Username;
                userClient.DeleteUser(username);
                return true;
            }
            else
            {
                return false;
            }
        }

        /*[STAThread]
        public IEnumerable<User> GetUsers()
        {
            Inicialization();
            List<User> listOfUsers = new List<User>();
            string usersCSV = userClient.ListUsers();
            string commaSeparator = ",";
            string[] usersArray = usersCSV.Split(commaSeparator.ToArray());
            for (int i = 0; i < usersArray.Length - 1; i++)
            {
                string username = usersArray[i];
                User theUser = new User();
                theUser.Username = username;
                listOfUsers.Add(theUser);
            }
            return listOfUsers;
        }*/

        [STAThread]
        public IEnumerable<User> GetUsers()
        {
            Inicialization();
            List<User> listOfUsers = userClient.ListUsers();
            return listOfUsers;
        }


        public bool ModifyUser(User user)
        {
            Inicialization();
            if (user != null)
            {
                string username = user.Username;
                string password = user.Password;
                userClient.EditUser(username, password);
                return true;
            }
            else
            {
                return false;
            }
        }

        [STAThread]
        public bool SaveUser(User user)
        {
            Inicialization();
            if (user != null)
            {
                string name = user.Username;
                string password = user.Password;
                userClient.AddUser(name, password);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
