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


        //OK
        public static bool CorrectPassword(User user, string password)
        {
            return user.Password.Equals(password);
        }

        //OK pero revisar dsp por las dudas
        public static void EditPassword(string username, string password)
        {
            User result = existingUsers.Find(x => x.Username == username);
            result.Password = password;
        }
        //OK
        public static void AddNewUser(string name, string password)
        {
            User userToAdd = new User();
            userToAdd.Username = name;
            userToAdd.Password = password;
            existingUsers.Add(userToAdd);
        }
        //OK
        public static void DeleteUser(string username)
        {
            User result = existingUsers.Find(x => x.Username == username);
            existingUsers.Remove(result);
        }
        //OK
        public static void DisconnectUser(User user)
        {
            ConnectedUsers.RemoveAll(u => u.Username.Equals(user.Username));
        }

        public static void AddUserSocket(User user, Socket socket)
        {
            if (!usersSockets.ContainsKey(user.Username))
            {
                usersSockets.Add(user.Username, socket);
            }
        }





    }
}
