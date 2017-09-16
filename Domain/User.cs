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

        public User()
        {
            friends = new List<User>();
        }

        public User(string userId)
        {
            username = userId;
            friends = new List<User>();
        }

        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
            friends = new List<User>();
        }

        public void AddFriend(User friend)
        {
            friends.Add(friend);
        }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        internal List<User> Friends { get => friends; set => friends = value; }

        //we need to redefine equals to be able to use contains from the user list. Equals should equal the username

        public override bool Equals(object value)
        {
            User user = value as User;

            return (user != null)
                && (username == user.username);
        }
    }
}
