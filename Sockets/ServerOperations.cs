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
    public class ServerOperations
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

        public ServerOperations(Context context, Socket socket, ClassLibrary classLibrary)
        {
            myContext = context;
            clientSocket = socket;
            this.classLibrary = classLibrary;
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

        public void PrintMessages(List<Message> messages, String theCase)
        {
            string returnData = "";
            foreach (Message m in messages)
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
                        //classLibrary.sendData(clientSocket, ClassLibrary.SECONDARY_MENU + ClassLibrary.PROTOCOL_SEPARATOR + "NULL");
                    }
                    //TODO - mandar al cliente un msj cndo no hay usuarios, para que el cliente vacíe su lista
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
                    break;
                case CASE_3:
                    //no saquemos este case 3 porque se rompe al entrar al default :)
                    break;

                case CASE_4:
                    //no saquemos este case 4 porque se rompe al entrar al default :) 
                    break;

                case CASE_5:

                    List<Message> unreadMessages = theUser.UnreadMessages;
                    if (unreadMessages.Count > 0)
                    {
                        PrintMessages(unreadMessages, ClassLibrary.CASE_5);
                    }
                    //validar cuando no hay datos, mandar algo.
                    break;

                case CASE_6:
                    classLibrary.sendData(clientSocket, ClassLibrary.DISCONNECT + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.DISCONNECT);
                    DisconnectClient(theUser);
                    break;

                default:
                    classLibrary.sendData(clientSocket, ClassLibrary.PROTOCOL_ERROR_RESPONSE + ". Opcion incorrecta");
                    break;
            }
        }

        public void SendFriendRequest(User loggedInUser, User friendRequested)
        {
            friendRequested.AddFriendRequest(loggedInUser);
            classLibrary.sendData(clientSocket, ClassLibrary.CASE_3 + ClassLibrary.PROTOCOL_SEPARATOR + ClassLibrary.PROTOCOL_OK_RESPONSE);
        }

        public void SecondaryMenu(User loggedInUser, User userToAccept, string accept)
        {
            if (accept.Equals(CASE_1))
            {
                loggedInUser.AcceptFriendRequest(userToAccept);
            }
            else
            {
                if (accept.Equals(CASE_0))
                {
                    loggedInUser.CancelFriendRequest(userToAccept);
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
                        if (user.UnreadMessages.Count > 0)
                        {
                            string unreadMessages = "";
                            foreach (Message m in user.UnreadMessages)
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
                    user.UnreadMessages.Add(new Message(fromUsername, message, myContext));
                }
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
                    Console.WriteLine(user.Username);
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
    }
}
