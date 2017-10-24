using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        private string username;
        private string password;
        private List<User> friends;
        private List<User> pendingFriendshipRequest;
        private List<ChatMessage> unreadMessages;
        private int connectedTimes;
        private DateTime connectedTime;
        public User()
        {
            friends = new List<User>();
            PendingFriendshipRequest = new List<User>();
            unreadMessages = new List<ChatMessage>();
        }

        public User(string userId)
        {
            username = userId;
            friends = new List<User>();
            pendingFriendshipRequest = new List<User>();
            unreadMessages = new List<ChatMessage>();
        }

        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
            friends = new List<User>();
            pendingFriendshipRequest = new List<User>();
            unreadMessages = new List<ChatMessage>();
        }

        public void AddFriendRequest(User theRequester)
        {
            pendingFriendshipRequest.Add(theRequester);
        }


        public void AcceptFriendRequest(User friendRequest)
        {
            lock (PendingFriendshipRequest)
            {
                pendingFriendshipRequest.Remove(friendRequest);
            }
            lock (Friends)
            {
                Friends.Add(friendRequest);
                friendRequest.Friends.Add(this);
            }
        }

        public void CancelFriendRequest(User friendRequest)
        {
            lock (friendRequest)
            {
                pendingFriendshipRequest.Remove(friendRequest);
            }
        }
        public void AddFriend(User friend)
        {
            friends.Add(friend);
        }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public List<User> Friends { get => friends; set => friends = value; }
        public List<User> PendingFriendshipRequest { get => pendingFriendshipRequest; set => pendingFriendshipRequest = value; }
        public List<ChatMessage> UnreadMessages { get => unreadMessages; set => unreadMessages = value; }

        public int ConnectedTimes { get => connectedTimes; set => connectedTimes = value; }
        public DateTime ConnectedTime { get => connectedTime; set => connectedTime = value; }
        //we need to redefine equals to be able to use contains from the user list. Equals should equal the username

        public override bool Equals(object value)
        {
            User user = value as User;

            return (user != null)
                && (username == user.username);
        }
    }
}
