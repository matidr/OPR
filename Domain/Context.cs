using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Context
    {
        private List<User> existingUsers;
        private List<User> connectedUsers;
        private Dictionary<string, Socket> usersSockets;

        public Context()
        {
            existingUsers = new List<User>();
            connectedUsers = new List<User>();
            usersSockets = new Dictionary<string, Socket>();
        }
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
