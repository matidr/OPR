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
using System.IO;
using IUserService;

namespace Sockets
{
    public class ServerOperations
    {
        private const string CASE_0 = "0";
        private const string CASE_1 = "1";
        private const string CASE_2 = "2";
        private const string CASE_3 = "3";
        private const string CASE_4 = "4";
        private const string CASE_5 = "5";
        private const string CASE_6 = "6";
        private const string CASE_7 = "7";

        private Socket clientSocket;
        private ClassLibrary classLibrary;
        private MessageLog theMessageLog;
        private static List<User> users = new List<User>();
        IUserService.IUserService userClient;

        public void Inicialization()
        {
            userClient = (IUserService.IUserService)Activator.GetObject
                (typeof(IUserService.IUserService),
                "tcp://localhost:5000/UserOperations");
        }

        public ServerOperations(Socket socket, ClassLibrary classLibrary, MessageLog mySystemLog)
        {
            theMessageLog = mySystemLog;
            clientSocket = socket;
            this.classLibrary = classLibrary;
            Inicialization();
        }

        public ServerOperations() { }

        [STAThread]
        public List<User> GetConnectedFriends(User theUser)
        {
            User user = getUsersRemoting().Find(x => x.Username.Equals(theUser.Username));
            List<User> connectedFriends = new List<User>();
            foreach (User u in userClient.GetFriends(user))
            {
                if (userClient.ListConnectedUsers().Contains(u))
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

        [STAThread]
        public void MainMenu(User theUser, string menuOption)
        {
            User userByReflection = getUsersRemoting().Find(x => x.Username.Equals(theUser.Username));
            switch (menuOption)
            {
                case CASE_1:
                    List<User> connectedFriends = GetConnectedFriends(userByReflection);
                    if (connectedFriends.Count > 0)
                    {
                        PrintFriends(connectedFriends, ClassLibrary.CASE_1);
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, ClassLibrary.CASE_1 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.EMPTY_STRING);
                    }
                    theMessageLog.SendMessageLog("El usuario " + userByReflection.Username + " ha solicitado ver sus amigos conectados.");
                    break;

                case CASE_2:
                    List<User> friendshipRequests = userClient.getPendingRequests(userByReflection);
                    if (friendshipRequests.Count > 0)
                    {
                        PrintFriends(friendshipRequests, ClassLibrary.CASE_2);
                    }
                    else
                    {
                        classLibrary.sendData(clientSocket, ClassLibrary.SECONDARY_MENU + ClassLibrary.PROTOCOL_SEPARATOR + "NULL");
                    }
                    theMessageLog.SendMessageLog("El usuario " + userByReflection.Username + " ha solicitado ver sus solicitudes de amistad pendientes.");
                    break;
                case CASE_3:
                    break;

                case CASE_4:
                    break;

                case CASE_5:
                    break;

                case CASE_6:
                    Context.Files.Clear();
                    string targetDirectory = "C:\\ejemplo\\";
                    string[] fileEntries = Directory.GetFiles(targetDirectory);
                    string returnString = "";
                    foreach (string fileName in fileEntries)
                    {
                        string shortFileName = Path.GetFileName(fileName);
                        userClient.AddFile(shortFileName);
                        returnString = returnString + shortFileName + ClassLibrary.LIST_SEPARATOR;
                    }
                    classLibrary.sendData(clientSocket, ClassLibrary.REQUEST_MEDIA + ClassLibrary.PROTOCOL_SEPARATOR + returnString);
                    break;

                case CASE_7:
                    classLibrary.sendData(clientSocket, ClassLibrary.DISCONNECT + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.DISCONNECT);
                    theMessageLog.SendMessageLog("El usuario " + userByReflection.Username + " se ha desconectado.");
                    DisconnectClient(userByReflection);
                    break;

                default:
                    classLibrary.sendData(clientSocket, ClassLibrary.PROTOCOL_ERROR_RESPONSE + ClassLibrary.PROTOCOL_SEPARATOR + "ERROR. Opcion incorrecta");
                    break;
            }
        }

        [STAThread]
        public void SendFriendRequest(User loggedInUser, User friendRequested)
        {
            User userByReflection = getUsersRemoting().Find(x => x.Username.Equals(loggedInUser.Username));
            User requestedByReflection = getUsersRemoting().Find(x => x.Username.Equals(friendRequested.Username));
            if (userByReflection != null && friendRequested != null)
            {
                userClient.AddFriendRequest(userByReflection, requestedByReflection);
                classLibrary.sendData(clientSocket, ClassLibrary.CASE_3 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE);
                theMessageLog.SendMessageLog("El usuario " + userByReflection.Username + " ha enviado una solicitud de amistad al usuario " + friendRequested.Username);
            }
            else
            {
                classLibrary.sendData(clientSocket, ClassLibrary.CASE_3 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_ERROR_RESPONSE);
                theMessageLog.SendMessageLog("El usuario " + userByReflection.Username + " ha enviado una solicitud de amistad a un usuario erroneo o inexistente.");
            }
        }
        [STAThread]
        public void SecondaryMenu(User loggedInUser, User userToAccept, string accept)
        {
            User userByReflection = getUsersRemoting().Find(x => x.Username.Equals(loggedInUser.Username));
            User userByReflection2 = getUsersRemoting().Find(x => x.Username.Equals(userToAccept.Username));
            if (accept.Equals(CASE_1))
            {
                userClient.AddFriend(userByReflection, userByReflection2);
                //userByReflection.AcceptFriendRequest(userToAccept);
                theMessageLog.SendMessageLog("El usuario " + userByReflection.Username + " ha aceptado la solicitud de amistad de " + userToAccept.Username);
            }
            else
            {
                if (accept.Equals(CASE_0))
                {
                    userByReflection.CancelFriendRequest(userToAccept);
                    theMessageLog.SendMessageLog("El usuario " + userByReflection.Username + " ha rechazado la solicitud de amistad de " + userToAccept.Username);
                }
            }
            classLibrary.sendData(clientSocket, ClassLibrary.SECONDARY_MENU + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE);
        }

        private void DisconnectClient(User theUser)
        {
            User userByReflection = getUsersRemoting().Find(x => x.Username.Equals(theUser.Username));
            clientSocket.Disconnect(false);
            userClient.RemoveConnectedUser(userByReflection);
        }

        [STAThread]
        public bool UserExists(string userId)
        {
            User theUser = new User();
            theUser.Username = userId;

            if (getUsersRemoting().Contains(theUser))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [STAThread]
        public void login(string loginInfo)
        {
            string[] loginInfoArray = loginInfo.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
            string userID = loginInfoArray[0];
            string password = loginInfoArray[1];

            if (!UserExists(userID))
            {
                // REGISTER

                User user = new User(userID, password);
                userClient.AddUser(userID, password);
                userClient.AddConnectedUser(user);
                Context.AddUserSocket(user, clientSocket);
                theMessageLog.SendMessageLog("Nuevo Cliente Conectado: " + user.Username);
                userClient.AddConectedTimes(user);
                userClient.AddConnectedTime(user, DateTime.Now);
                classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE + ". Bienvenido");
            }
            else
            {
                // LOGIN

                if (!userClient.UserAlreadyConnected(userID))
                {
                    User result = getUsersRemoting().Find(x => x.Username.Equals(userID));
                    if (Context.CorrectPassword(result, password))
                    {
                        userClient.AddConnectedUser(result);
                        Context.AddUserSocket(result, clientSocket);


                        result.ConnectedTimes++;
                        result.ConnectedTime = DateTime.Now;
                        if (userClient.GetMessages(result).Count > 0)
                        {
                            string unreadMessages = "";
                            foreach (ChatMessage m in userClient.GetMessages(result))
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

        [STAThread]
        public void Case4(string fromUsername, string toUsername, string message)
        {
            User userFrom = getUsersRemoting().Find(x => x.Username.Equals(toUsername));
            User userTo = getUsersRemoting().Find(x => x.Username.Equals(toUsername));
            if (userTo != null)
            {
                if (userClient.UserAlreadyConnected(toUsername))
                {
                    Socket toSocket = Context.UsersSockets[toUsername];
                    classLibrary.sendData(toSocket, ClassLibrary.NEW_MESSAGE + ClassLibrary.PROTOCOL_SEPARATOR + fromUsername + ClassLibrary.LIST_SEPARATOR + message);
                    classLibrary.sendData(clientSocket, ClassLibrary.CASE_4 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE);
                }
                else
                {
                    userClient.AddMessage(userTo, new ChatMessage(userFrom, message));
                }
                theMessageLog.SendMessageLog("El usuario " + fromUsername + " ha enviado un mensaje al usuario " + toUsername);
            }
            else
            {
                classLibrary.sendData(clientSocket, ClassLibrary.CASE_4 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_ERROR_RESPONSE);
            }
        }

        [STAThread]
        public void ClearUnreadMessages(string username)
        {
            User user = getUsersRemoting().Find(x => x.Username.Equals(username));
            userClient.ClearMessages(user);
        }



        [STAThread]
        public List<User> getUsersRemoting()
        {
            List<User> listOfUsers = userClient.ListUsers();
            return listOfUsers;
        }

        [STAThread]
        private void PrintListInConsole(List<User> listOfUsers, string message)
        {
            if (listOfUsers != null)
            {
                foreach (User user in listOfUsers)
                {
                    if (message.Equals("usuarios conectados "))
                    {
                        DateTime nowDate = DateTime.Now;
                        TimeSpan timespan = nowDate - user.ConnectedTime;
                        Console.WriteLine(user.Username + " Amigos: " + userClient.GetFriends(user).Count + " Veces conectado: " + user.ConnectedTimes + " Tiempo conectado: " + Math.Round(timespan.TotalMinutes) + " minutos " + Math.Round(timespan.TotalSeconds) + " segundos");
                    }
                    else
                    {
                        Console.WriteLine(user.Username + " Amigos: " + userClient.GetFriends(user).Count + " Veces conectado: " + user.ConnectedTimes);
                    }
                }
            }
            else
            {
                Console.WriteLine("No hay " + message + "en el momento");
            }
            Console.WriteLine();
        }

        [STAThread]
        public void ServerMenu()
        {
            Console.WriteLine("1) Mostrar todos los usuarios del sistema");
            Console.WriteLine("2) Mostrar los usuarios conectados");
            string option = Console.ReadLine();
            switch (option)
            {
                case CASE_1:
                    PrintListInConsole(getUsersRemoting(), "usuarios registrados ");
                    ServerMenu();
                    break;

                case CASE_2:
                    PrintListInConsole(userClient.ListConnectedUsers(), "usuarios conectados ");
                    ServerMenu();
                    break;

                default:
                    Console.WriteLine("Opcion incorrecta. Intentelo de nuevo.");
                    ServerMenu();
                    break;
            }
        }

        [STAThread]
        public void SaveFile(string file)
        {
            userClient.AddFile(file);
        }

        public void SendMedia(string username, string fileToDownload)
        {
            Socket socket = Context.UsersSockets[username];
            classLibrary.SendMedia(socket, fileToDownload);
        }

    }
}
