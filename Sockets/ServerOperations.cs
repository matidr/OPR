using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Domain;
using Protocol;
using Log;
using IUserService;

namespace Sockets
{
    public class ServerOperations : MarshalByRefObject, IUserService.IUserService
    {
        private const string CASE_0 = "0";
        private const string CASE_1 = "1";
        private const string CASE_2 = "2";
        private const string CASE_3 = "3";
        private const string CASE_4 = "4";
        private const string CASE_5 = "5";
        private const string CASE_6 = "6";

        private Context myContext;
        private Socket clientSocket;
        private ClassLibrary classLibrary;
        private MessageLog theMessageLog;
        
        

        public ServerOperations(Socket socket, ClassLibrary classLibrary, MessageLog mySystemLog)
        {
            theMessageLog = mySystemLog;
            myContext = Context.Instance;
            clientSocket = socket;
            this.classLibrary = classLibrary;
        }

        public ServerOperations()
        {
            myContext = Context.Instance; 
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

        public void PrintFriends(List<User> friends, String theCase)
        {
            string returnData = "";
            foreach (User u in friends)
            {
                returnData = returnData + u.Username + ClassLibrary.LIST_SEPARATOR;
            }
            classLibrary.sendData(clientSocket, theCase + ClassLibrary.PROTOCOL_SEPARATOR + returnData);
        }

        public void PrintMessages(List<ChatMessage> messages, String theCase)
        {
            string returnData = "";
            foreach (ChatMessage m in messages)
            {
                returnData = returnData + "[" + m.TheUser.Username + "] " + m.TheMessage + ClassLibrary.LIST_SEPARATOR;

            }
            classLibrary.sendData(clientSocket, theCase + ClassLibrary.PROTOCOL_SEPARATOR + returnData);
        }

        public void MainMenu(User theUser, string menuOption)
        {
            switch (menuOption)
            {
                case CASE_1:
                    List<User> connectedFriends = GetConnectedFriends(theUser);
                    if (connectedFriends.Count > 0)
                    {
                        PrintFriends(connectedFriends, ClassLibrary.CASE_1);
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, ClassLibrary.CASE_1 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.EMPTY_STRING);
                    }
                    theMessageLog.SendMessageLog("El usuario " + theUser.Username + " ha solicitado ver sus amigos conectados.");
                    break;

                case CASE_2:
                    List<User> friendshipRequests = theUser.PendingFriendshipRequest;
                    if (friendshipRequests.Count > 0)
                    {
                        PrintFriends(friendshipRequests, ClassLibrary.CASE_2);
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, ClassLibrary.SECONDARY_MENU + ClassLibrary.PROTOCOL_SEPARATOR + "NULL");
                    }
                    theMessageLog.SendMessageLog("El usuario " + theUser.Username + " ha solicitado ver sus solicitudes de amistad pendientes.");
                    break;
                case CASE_3:
                    break;

                case CASE_4:
                    break;

                case CASE_5:
                    List<ChatMessage> unreadMessages = theUser.UnreadMessages;
                    if (unreadMessages.Count > 0)
                    {
                        PrintMessages(unreadMessages, ClassLibrary.CASE_5);
                    }
                    theMessageLog.SendMessageLog("El usuario " + theUser.Username + " ha solicitado ver sus mensajes sin leer.");
                    break;

                case CASE_6:  
                    classLibrary.sendData(clientSocket, ClassLibrary.DISCONNECT + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.DISCONNECT);
                    theMessageLog.SendMessageLog("El usuario " + theUser.Username + " se ha desconectado.");
                    DisconnectClient(theUser);
                    break;

                default:
                    classLibrary.sendData(clientSocket, ClassLibrary.PROTOCOL_ERROR_RESPONSE + ". Opcion incorrecta");
                    break;
            }
        }

        public void SendFriendRequest(User loggedInUser, User friendRequested)
        {
            if (loggedInUser != null && friendRequested != null)
            {
                friendRequested.AddFriendRequest(loggedInUser);
                classLibrary.sendData(clientSocket, ClassLibrary.CASE_3 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE);
                theMessageLog.SendMessageLog("El usuario " + loggedInUser.Username + " ha enviado una solicitud de amistad al usuario " + friendRequested.Username);
            } else
            {
                classLibrary.sendData(clientSocket, ClassLibrary.CASE_3 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_ERROR_RESPONSE);
                theMessageLog.SendMessageLog("El usuario " + loggedInUser.Username + " ha enviado una solicitud de amistad a un usuario erroneo o inexistente.");
            }
        }

        public void SecondaryMenu(User loggedInUser, User userToAccept, string accept)
        {
            if (accept.Equals(CASE_1))
            {
                loggedInUser.AcceptFriendRequest(userToAccept);
                theMessageLog.SendMessageLog("El usuario " + loggedInUser.Username + " ha aceptado la solicitud de amistad de " + userToAccept.Username);
            }
            else
            {
                if (accept.Equals(CASE_0))
                {
                    loggedInUser.CancelFriendRequest(userToAccept);
                    theMessageLog.SendMessageLog("El usuario " + loggedInUser.Username + " ha rechazado la solicitud de amistad de " + userToAccept.Username);
                }
            }
            classLibrary.sendData(clientSocket, ClassLibrary.SECONDARY_MENU + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE);
        }

        private void DisconnectClient(User theUser)
        {
            clientSocket.Disconnect(false);
            myContext.DisconnectUser(theUser);
        }

        public void login(string loginInfo)
        {
            string[] loginInfoArray = loginInfo.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
            string userID = loginInfoArray[0];
            string password = loginInfoArray[1];

            if (!myContext.UserExist(userID))
            {
                // REGISTER

                User user = new User(userID, password);
                myContext.AddNewUser(user);
                myContext.ConnectUser(user);
                myContext.AddUserSocket(user.Username, clientSocket);
                theMessageLog.SendMessageLog("Nuevo Cliente Conectado: "+ user.Username); 
                user.ConnectedTimes++;
                user.ConnectedTime = DateTime.Now;
                classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE + ". Bienvenido");
            }
            else
            {
                // LOGIN

                if (!myContext.UserAlreadyConnected(userID))
                {
                    if (myContext.CorrectPassword(userID, password))
                    {
                        myContext.ConnectUser(new User(userID, password));
                        myContext.AddUserSocket(userID, clientSocket);
                        User user = myContext.ExistingUsers.Find(x => x.Username.Equals(userID));
                        user.ConnectedTimes++;
                        user.ConnectedTime = DateTime.Now;
                        if (user.UnreadMessages.Count > 0)
                        {
                            string unreadMessages = "";
                            foreach (ChatMessage m in user.UnreadMessages)
                            {
                                unreadMessages = unreadMessages + m.TheUser.Username + ": " + m.TheMessage + ClassLibrary.LIST_SEPARATOR;
                            }
                            classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_MSJS_RESPONSE + ClassLibrary.LIST_SEPARATOR + unreadMessages);
                        }
                        else
                        {
                            classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE + ". Bienvenido");
                        }
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_ERROR_RESPONSE + ": Contrasena incorrecta");
                    }
                }
                else
                {
                    classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_ERROR_RESPONSE + ": Usuario ya conectado");
                }
            }
        }

        public void Case4(string fromUsername, string toUsername, string message)
        {
            User userFrom = myContext.ExistingUsers.Find(x => x.Username.Equals(fromUsername));
            User userTo = userFrom.Friends.Find(x => x.Username.Equals(toUsername));
            if (userTo != null)
            {
                if (myContext.UserAlreadyConnected(toUsername))
                {
                    Socket toSocket = myContext.UsersSockets[toUsername];
                    classLibrary.sendData(toSocket, ClassLibrary.NEW_MESSAGE + ClassLibrary.PROTOCOL_SEPARATOR + fromUsername + ClassLibrary.LIST_SEPARATOR + message);
                    classLibrary.sendData(clientSocket, ClassLibrary.CASE_4 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE);
                }
                else
                {
                    User user = myContext.ExistingUsers.Find(x => x.Username.Equals(toUsername));
                    user.UnreadMessages.Add(new ChatMessage(fromUsername, message));
                }
                theMessageLog.SendMessageLog("El usuario " + fromUsername + " ha enviado un mensaje al usuario " + toUsername);
            }
            else
            {
                classLibrary.sendData(clientSocket, ClassLibrary.CASE_4 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_ERROR_RESPONSE);
            }
        }

        public void ClearUnreadMessages(string username)
        {
            User user = myContext.ExistingUsers.Find(x => x.Username.Equals(username));
            user.UnreadMessages.Clear();
        }

        private void PrintListInConsole(List<User> users, string message)
        {
            if (users.Count > 0)
            {
                foreach (User user in users)
                {
                    if (message.Equals("usuarios conectados "))
                    {
                        DateTime nowDate = DateTime.Now;
                        TimeSpan timespan = nowDate - user.ConnectedTime;
                        Console.WriteLine(user.Username + " Amigos: " + user.Friends.Count + " Veces conectado: " + user.ConnectedTimes + " Tiempo conectado: " + Math.Round(timespan.TotalMinutes) + " minutos " + Math.Round(timespan.TotalSeconds) + " segundos");
                    }
                    else
                    {
                        Console.WriteLine(user.Username + " Amigos: " + user.Friends.Count + " Veces conectado: " + user.ConnectedTimes);
                    }
                    
                }
            }
            else
            {
                Console.WriteLine("No hay " + message + "en el momento");
            }
            Console.WriteLine();
        }

        public void ServerMenu()
        {
            Console.WriteLine("1) Mostrar todos los usuarios del sistema");
            Console.WriteLine("2) Mostrar los usuarios conectados");
            string option = Console.ReadLine();
            switch (option)
            {
                case CASE_1:
                    PrintListInConsole(myContext.ExistingUsers, "usuarios registrados ");
                    ServerMenu();
                    break;

                case CASE_2:
                    PrintListInConsole(myContext.ConnectedUsers, "usuarios conectados ");
                    ServerMenu();
                    break;

                default:
                    Console.WriteLine("Opcion incorrecta. Intentelo de nuevo.");
                    ServerMenu();
                    break;
            }
        }

        public void AddUser(string name, string password)
        {
            myContext.AddNewUser(name, password);
        }

        public void EditUser(string name, string password)
        {
            myContext.EditPassword(name, password);
        }

        public void DeleteUser(string username)
        {
            myContext.DeleteUser(username);
        }

        public string ListUsers()
        {
            return myContext.ListUsersInCSV();
        }

    }
}
