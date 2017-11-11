using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Domain;

namespace WCFLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IUser
    {
        [OperationContract]
        bool SaveUser(User user);

        [OperationContract]
        IEnumerable<User> GetUsers();

        [OperationContract]
        bool ModifyUser(User user);

        [OperationContract]
        bool DeleteUser(User user);

        // TODO: Add your service operations here
    }
}
