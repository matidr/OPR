using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IUserService;
using Domain;

namespace Sockets
{
    public class UserOperations : MarshalByRefObject, IUserService.IUserService
    {
        public void AddUser(string name, string password)
        {
            Context.AddNewUser(name, password);
        }

        public void EditUser(string name, string password)
        {
            Context.EditPassword(name, password);
        }

        public void DeleteUser(string username)
        {
            Context.DeleteUser(username);
        }

        public string ListUsers()
        {
            return Context.ListUsersInCSV();
        }

   

    }
}
