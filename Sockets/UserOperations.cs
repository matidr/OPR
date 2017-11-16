using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IUserService;
using Domain;
using System.Net.Sockets;

namespace Sockets
{
    public class UserOperations : MarshalByRefObject, IUserService.IUserService
    {
        void IUserService.IUserService.AddUser(string name, string password)
        {
            Context.AddNewUser(name, password);
        }

        void IUserService.IUserService.EditUser(string name, string password)
        {
            Context.EditPassword(name, password);
        }

        void IUserService.IUserService.DeleteUser(string username)
        {
            Context.DeleteUser(username);
        }

        /*public string ListUsers()
        {
            return Context.ListUsersInCSV();
        }*/
        List<User> IUserService.IUserService.ListUsers()
        {
            return Context.ExistingUsers;
        }

        void IUserService.IUserService.AddConnectedUser(User user)
        {
            Context.ConnectedUsers.Add(Context.ExistingUsers.Find(x => x.Username.Equals(user.Username)));
        }

        void IUserService.IUserService.RemoveConnectedUser(User user)
        {
            Context.ConnectedUsers.Remove(Context.ExistingUsers.Find(x => x.Username.Equals(user.Username)));
        }
        List<User> IUserService.IUserService.ListConnectedUsers()
        {
            return Context.ConnectedUsers;
        }

        bool IUserService.IUserService.UserAlreadyConnected(string userID)
        {
            return Context.ConnectedUsers.Find(x => x.Username.Equals(userID))!=null;
        }

        void IUserService.IUserService.AddFile(string file)
        {
            Context.Files.Add(file);
        }
        bool IUserService.IUserService.fileExists(string file)
        {
            string selectFile = Context.Files.Find(x => x.Equals(file));
            return selectFile != null && !selectFile.Equals("");
        }

        void IUserService.IUserService.AddFriendRequest(User userfrom, User userTo)
        {
            Context.ExistingUsers.Find(x => x.Username.Equals(userTo.Username)).PendingFriendshipRequest.Add(Context.ExistingUsers.Find(x => x.Username.Equals(userfrom.Username)));
        }

        List<User> IUserService.IUserService.getPendingRequests(User user)
        {
            return Context.ExistingUsers.Find(x => x.Username.Equals(user.Username)).PendingFriendshipRequest;
        }

        void IUserService.IUserService.AddFriend(User userfrom, User userTo)
        {
            Context.ExistingUsers.Find(x => x.Username.Equals(userTo.Username)).PendingFriendshipRequest.Remove(Context.ExistingUsers.Find(x => x.Username.Equals(userfrom.Username)));
            Context.ExistingUsers.Find(x => x.Username.Equals(userTo.Username)).Friends.Add(Context.ExistingUsers.Find(x => x.Username.Equals(userfrom.Username)));
            Context.ExistingUsers.Find(x => x.Username.Equals(userfrom.Username)).Friends.Add(Context.ExistingUsers.Find(x => x.Username.Equals(userTo.Username)));
        }

        List<User> IUserService.IUserService.GetFriends(User user)
        {
            return Context.ExistingUsers.Find(x => x.Username.Equals(user.Username)).Friends;
        }

        void IUserService.IUserService.AddMessage(User user, ChatMessage message)
        {
            Context.ExistingUsers.Find(x => x.Username.Equals(user.Username)).UnreadMessages.Add(message);
        }

        List<ChatMessage> IUserService.IUserService.GetMessages(User user)
        {
            return Context.ExistingUsers.Find(x => x.Username.Equals(user.Username)).UnreadMessages;
        }

        void IUserService.IUserService.ClearMessages(User user)
        {
            Context.ExistingUsers.Find(x => x.Username.Equals(user.Username)).UnreadMessages.Clear();
        }
        void IUserService.IUserService.AddConectedTimes(User user)
        {
            Context.ExistingUsers.Find(x => x.Username.Equals(user.Username)).ConnectedTimes++;
        }

        void IUserService.IUserService.AddConnectedTime(User user, DateTime date)
        {
            Context.ExistingUsers.Find(x => x.Username.Equals(user.Username)).ConnectedTime = date; 
        }
    }
}
