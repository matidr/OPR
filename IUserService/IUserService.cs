using Domain;
using System.Collections.Generic;
using System.Net.Sockets;

namespace IUserService
{
    public interface IUserService
    {
        void AddUser(string name, string password);
        void EditUser(string name, string password);
        void DeleteUser(string username);
        List<User> ListUsers();

        void AddConnectedUser(User user);
        void RemoveConnectedUser(User user);
        List<User> ListConnectedUsers();
        bool UserAlreadyConnected(string userID);
        void AddFile(string file);
        bool fileExists(string file);
        void AddFriendRequest(User userfrom, User userTo);
        List<User> getPendingRequests(User user);
        void AddFriend(User userfrom, User userTo);
        List<User> GetFriends(User user);
        void AddMessage(User user, ChatMessage message);
        List<ChatMessage> GetMessages(User user);
        void ClearMessages(User user);
        void AddConectedTimes(User user);
        void AddConnectedTime(User user, System.DateTime date);
    }
}
