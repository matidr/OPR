using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public static class Context
    {
        // private static Context instance = null;
        private static readonly object padlock = new object();
        private static List<User> existingUsers = new List<User>();
        private static List<User> connectedUsers = new List<User>();
        private static List<string> files = new List<string>();
        private static Dictionary<string, Socket> usersSockets = new Dictionary<string, Socket>();

     


        public static List<User> ExistingUsers { get => existingUsers; set => existingUsers = value; }
        public static List<User> ConnectedUsers { get => connectedUsers; set => connectedUsers = value; }
        public static List<string> Files { get => files; set => files = value; }
        public static Dictionary<string, Socket> UsersSockets { get => usersSockets; set => usersSockets = value; }

        public static bool UserAlreadyConnected(string userID)
        {
            return ConnectedUsers.Contains(new User(userID));
        }

       /* public static bool CorrectPassword(User user, string password)
        {
            return user.Password.Equals(password);
        }*/

        public static void AddNewUser(User user)
        {
            existingUsers.Add(user);
        }
        public static void EditPassword(string username, string password)
        {
            User result = existingUsers.Find(x => x.Username == username);
            result.Password = password;
        }

        public static void AddNewUser(string name, string password)
        {
            User userToAdd = new User();
            userToAdd.Username = name;
            userToAdd.Password = password;
            existingUsers.Add(userToAdd);
        }

        public static string ListUsersInCSV()
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

        public static void DeleteUser(string username)
        {
            User result = existingUsers.Find(x => x.Username == username);
            existingUsers.Remove(result);
        }

        public static void ConnectUser(User user)
        {
            ConnectedUsers.Add(user);
        }

        public static void AddUserSocket(string username, Socket socket)
        {
            usersSockets.Add(username, socket);
        }

        public static void DisconnectUser(User user)
        {
            ConnectedUsers.RemoveAll(u => u.Username.Equals(user.Username));
            usersSockets.Remove(user.Username);
        }

        public static void addFile(string file)
        {
            files.Add(file);
        }

        public static bool fileExists(string file)
        {
            string selectFile = files.Find(x => x.Equals(file));
            return selectFile != null && !selectFile.Equals("");
        }

    }
}
