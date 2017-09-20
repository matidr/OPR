using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Domain;
using Protocol;

namespace Sockets
{
    public class Operations
    {
        private Context myContext;
        

        public Operations(Context context)
        {
            myContext = context;
        }

        public List<User> GetConnectedFriends(User theUser)
        {
            User user = myContext.ExistingUsers.Find(x => x.Username.Equals(theUser.Username));
            List<User> connectedFriends = new List<User>();
            foreach (User u in user.Friends)
            {
                if (myContext.ConnectedUsers.Contains(u))
                {
                    connectedFriends.Add(u);
                }
            }
            return connectedFriends;
        }

        public void PrintFriends(Socket clientSocket, Protocol.ClassLibrary classLibrary, List<User> friends)
        {
            string returnData = "";
            foreach (User u in friends)
            {
                returnData = returnData + u.Username + ClassLibrary.LIST_SEPARATOR;
            }
            classLibrary.sendData(clientSocket, ClassLibrary.CASE_1 + ClassLibrary.PROTOCOL_SEPARATOR + returnData);
        }

        public void PrintMessages(Socket clientSocket, Protocol.ClassLibrary classLibrary, List<Message> messages)
        {
            foreach (Message m in messages)
            {
                classLibrary.sendData(clientSocket, "[" + m.TheUser + "] " + m.TheMessage);
            }
        }

        public void MainMenu(Socket clientSocket, Protocol.ClassLibrary classLibrary, User theUser, string menuOption)
        {
            switch (menuOption)
            {
                case "1":
                    List<User> connectedFriends = GetConnectedFriends(theUser);
                    if (connectedFriends.Count > 0)
                    {
                        PrintFriends(clientSocket, classLibrary, connectedFriends);
                    }
                    break;
                case "2":
                    List<User> friendshipRequests = theUser.PendingFriendshipRequest;
                    if (friendshipRequests.Count > 0)
                    {
                        PrintFriends(clientSocket, classLibrary, friendshipRequests);
                        classLibrary.sendData(clientSocket, "FINISH");
                        //hay que hacer un sub-menú para que acepte las solicitudes o rechace
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, "No tienes solicitudes pendientes de amistad");
                        classLibrary.sendData(clientSocket, "FINISH");
                    }
                    break;
                case "3":
                    //3. Enviar solicitud de amistad
                    break;
                case "4":
                    //4. Enviar mensaje a un amigo
                    break;
                case "5":
                    List<Message> unreadMessages = theUser.UnreadMessages;
                    if (unreadMessages.Count > 0)
                    {
                        PrintMessages(clientSocket, classLibrary, unreadMessages);
                        classLibrary.sendData(clientSocket, "FINISH");
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, "No tienes mensajes nuevos");
                        classLibrary.sendData(clientSocket, "FINISH");
                    }
                    break;
                case "6":
                    DisconnectClient(clientSocket, classLibrary, theUser);
                    break;
                default:
                    classLibrary.sendData(clientSocket, "ERROR. Opcion incorrecta");
                    break;
            }
        }

        private void DisconnectClient(Socket clientSocket, Protocol.ClassLibrary classLibrary, User theUser)
        {
            clientSocket.Disconnect(false);
            myContext.DisconnectUser(theUser);
        }

        public void login(Socket clientSocket, Protocol.ClassLibrary classLibrary, string loginInfo)
        {
            string[] loginInfoArray = loginInfo.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
            string userID = loginInfoArray[0];
            string password = loginInfoArray[1];

            if (!myContext.UserExist(userID))
            {
                // REGISTER

                User user = new User(userID, password);
                user.AddFriend(new User("Denu"));
                user.AddFriend(new User("Leslie"));
                myContext.AddNewUser(user);
                myContext.ConnectUser(user);
                classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + "OK. Bienvenido");
            }
            else
            {
                // LOGIN

                if (!myContext.UserAlreadyConnected(userID))
                {
                    if (myContext.CorrectPassword(userID, password))
                    {
                        myContext.ConnectUser(new User(userID, password));
                        classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + "OK. Bienvenido");
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + "ERROR: Contrasena incorrecta");
                    }
                }
                else
                {
                    classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + "ERROR: Usuario ya conectado");
                }
            }
        }
    }
}
