using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Protocol
{
    public class ClassLibrary
    {
        private static int FIXED_SIZE = 4;

        //CONSTS
        public const string PROTOCOL_SEPARATOR = "&";
        public const string PROTOCOL_OK_RESPONSE = "OK";
        public const string PROTOCOL_ERROR_RESPONSE = "ERROR";
        public const string PROTOCOL_OK_MSJS_RESPONSE = "MSJS";
        public const string LIST_SEPARATOR = ";";
        public const string END_OF_MESSAGE = "FINISH";
        public const string CASE_1 = "CASE 1 ";
        public const string CASE_2 = "CASE 2 ";
        public const string CASE_2A = "CASE 2A ";
        public const string CASE_3 = "CASE 3 ";
        public const string CASE_4 = "CASE 4 ";
        public const string CASE_5 = "CASE 5 ";
        public const string NEW_MESSAGE = "NewMessage ";
        public const string CLEAR_UNREAD_MESSAGES = "ClearUnreadMessages";
        public const string LOGIN = "LOGIN";
        public const string MENU_OPTION = "MENUOPTION";
        public const string DISCONNECT = "DISCONNECT";
        public const string SECONDARY_MENU = "SECONDARY MENU";
        public const string EMPTY_STRING = "";

        //FLAGS
        public static bool LOGIN_FLAG = false;
        public static bool CASE1_FLAG = false;
        public static bool CASE2_FLAG = false;
        public static bool CASE2A_FLAG = false;
        public static bool CASE3_FLAG = false;
        public static bool CASE5_FLAG = false;

        public int getFixedSize()
        {
            return FIXED_SIZE;
        }

        public void sendData(Socket clientSocket, String textToSend)
        {
            try
            {
                var data = System.Text.Encoding.ASCII.GetBytes(textToSend);
                var length = data.Length;
                var dataLength = BitConverter.GetBytes(length);

                //aca mando el largo
                int sent = 0;
                while (sent < FIXED_SIZE)
                {
                    sent += clientSocket.Send(dataLength, sent, FIXED_SIZE - sent, SocketFlags.None);
                }
                //aca manda la data
                sent = 0;
                while (sent < data.Length)
                {
                    sent += clientSocket.Send(data, sent, data.Length - sent, SocketFlags.None);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error de conexion");
            }
        }

        public string receiveData(Socket clientSocket)
        {
            try
            {
                var dataReceived = new byte[FIXED_SIZE];
                int received = 0;
                while (received < FIXED_SIZE)
                {
                    received += clientSocket.Receive(dataReceived, received, FIXED_SIZE - received, SocketFlags.None);
                }

                var dataReceived2 = new byte[BitConverter.ToInt32(dataReceived, 0)];

                received = 0;
                while (received < dataReceived2.Length)
                {
                    received += clientSocket.Receive(dataReceived2, received, dataReceived2.Length - received, SocketFlags.None);
                }
                return System.Text.Encoding.ASCII.GetString(dataReceived2);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error de conexion");
                return EMPTY_STRING;
            }
        }
    }
}
