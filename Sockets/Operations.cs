using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Domain;

namespace Sockets
{
    public class Operations
    {
        private Context myContext;


        public Operations(Context context)
        {
            myContext = context;
        }
        public string Login(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID)
        {
            string username = requestUsername(clientSocket, classLibrary, userID);
            var password = classLibrary.receiveData(clientSocket);
            requestPassword(clientSocket, classLibrary, username, password);
            return userID;
        }

        public void Register(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID)
        {
            classLibrary.sendData(clientSocket, "OK. Ingrese su contrasena");
            string password = classLibrary.receiveData(clientSocket);
            User user = new User(userID, password);
            myContext.AddNewUser(user);
            myContext.ConnectUser(user);
            classLibrary.sendData(clientSocket, "OK. Bienvenido");
        }

        public List<User> GetConnectedFriends(User theUser)
        {
            List<User> connectedFriends = new List<User>();
            foreach (User u in theUser.Friends)
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
            int i = 0;
            classLibrary.sendData(clientSocket, "Your connected friends are: ");
            foreach (User u in friends)
            {
                i++;
                classLibrary.sendData(clientSocket, i + ") " + u.Username);
            }
        }

        public void PrintMessages(Socket clientSocket, Protocol.ClassLibrary classLibrary, List<Message> messages)
        {
            foreach (Message m in messages)
            {
                classLibrary.sendData(clientSocket, "[" + m.TheUser + "] " + m.TheMessage);
            }
        }

        public void MainMenu(Socket clientSocket, Protocol.ClassLibrary classLibrary, User theUser)
        {
            var menuOption = classLibrary.receiveData(clientSocket);
            switch (menuOption)
            {
                case "1":
                    List<User> connectedFriends = GetConnectedFriends(theUser);
                    if (connectedFriends.Count > 0)
                    {
                        PrintFriends(clientSocket, classLibrary, connectedFriends);
                        classLibrary.sendData(clientSocket, "FINISH");
                        MainMenu(clientSocket, classLibrary, theUser);
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, "No tienes amigos conectados en este momento");
                        classLibrary.sendData(clientSocket, "FINISH");
                        MainMenu(clientSocket, classLibrary, theUser);
                    }
                    break;
                case "2":
                    List<User> friendshipRequests = theUser.PendingFriendshipRequest;
                    if (friendshipRequests.Count > 0)
                    {
                        PrintFriends(clientSocket, classLibrary, friendshipRequests);
                        classLibrary.sendData(clientSocket, "FINISH");
                        MainMenu(clientSocket, classLibrary, theUser);
                        //hay que hacer un sub-menú para que acepte las solicitudes o rechace
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, "No tienes solicitudes pendientes de amistad");
                        classLibrary.sendData(clientSocket, "FINISH");
                        MainMenu(clientSocket, classLibrary, theUser);
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
                        MainMenu(clientSocket, classLibrary, theUser);
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, "No tienes mensajes nuevos");
                        classLibrary.sendData(clientSocket, "FINISH");
                        MainMenu(clientSocket, classLibrary, theUser);
                    }
                    break;
                case "6":
                    DisconnectClient(clientSocket, classLibrary, theUser);
                    break;
                default:
                    classLibrary.sendData(clientSocket, "ERROR. Opcion incorrecta");
                    MainMenu(clientSocket, classLibrary, theUser);
                    break;
            }
        }

        private void DisconnectClient(Socket clientSocket, Protocol.ClassLibrary classLibrary, User theUser)
        {
            clientSocket.Disconnect(false);
            myContext.DisconnectUser(theUser);
        }
        private void requestPassword(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID, string password)
        {

            if (myContext.CorrectPassword(userID, password))
            {
                myContext.ConnectUser(new User(userID, password));
                classLibrary.sendData(clientSocket, "OK. Bienvenido");
            }
            else
            {
                classLibrary.sendData(clientSocket, "ERROR: Contrasena incorrecta");
                password = classLibrary.receiveData(clientSocket);
                requestPassword(clientSocket, classLibrary, userID, password);
            }

        }

        private string requestUsername(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID)
        {
            if (!myContext.UserAlreadyConnected(userID))
            {
                classLibrary.sendData(clientSocket, "OK. Ingrese su contraseña");
                return userID;
            }
            else
            {
                classLibrary.sendData(clientSocket, "ERROR: Usuario ya conectado, pruebe con otro usuario");
                userID = classLibrary.receiveData(clientSocket);
                return requestUsername(clientSocket, classLibrary, userID);
            }
        }
    }
}
