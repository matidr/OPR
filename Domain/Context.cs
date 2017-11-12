using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public sealed class Context 
    {
        private static Context instance = null;
        private static readonly object padlock = new object();

        private static List<User> existingUsers;
        private static List<User> connectedUsers;
        private static Dictionary<string, Socket> usersSockets;

        Context() { }

        public static Context Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Context();
                        existingUsers = new List<User>();
                        connectedUsers = new List<User>();
                        usersSockets = new Dictionary<string, Socket>();
                    }
                    return instance;
                }
            }

        }

       /* public Context()
        {
            existingUsers = new List<User>();
            connectedUsers = new List<User>();
            usersSockets = new Dictionary<string, Socket>();
        } */
        public List<User> ExistingUsers { get => existingUsers; set => existingUsers = value; }
        public List<User> ConnectedUsers { get => connectedUsers; set => connectedUsers = value; }
        public Dictionary<string, Socket> UsersSockets { get => usersSockets; set => usersSockets = value; }

        public bool UserExist(string userId)
        {
            return existingUsers.Contains(new User(userId));
        }

        public bool UserAlreadyConnected(string userID)
        {
            return ConnectedUsers.Contains(new User(userID));
        }

        public bool CorrectPassword(string user, string password)
        {
            User result = existingUsers.Find(x => x.Username == user);
            return result.Password.Equals(password);
        }

        public void AddNewUser(User user)
        {
            existingUsers.Add(user);
        }
        public void EditPassword(string username, string password)
        {
            User result = existingUsers.Find(x => x.Username == username);
            result.Password = password;
        }

        public void AddNewUser(string name, string password)
        {
            User userToAdd = new User();
            userToAdd.Username = name;
            userToAdd.Password = password;
            existingUsers.Add(userToAdd);
        }

        public string ListUsersInCSV()
        {
            string CSVlist = "";
            if (existingUsers.Count > 0)
            {
                foreach (User u in existingUsers)
                {
                    CSVlist = CSVlist + u.Username + ",";
                }
            }
            return CSVlist;
        }

        public void DeleteUser(string username)
        {
            User result = existingUsers.Find(x => x.Username == username);
            existingUsers.Remove(result);
        }

        public void ConnectUser(User user)
        {
            ConnectedUsers.Add(user);
        }

        public void AddUserSocket(string username, Socket socket)
        {
            usersSockets.Add(username, socket);
        }

        public void DisconnectUser(User user)
        {
            ConnectedUsers.RemoveAll(u => u.Username.Equals(user.Username));
            usersSockets.Remove(user.Username);
        }

        
    }
}
