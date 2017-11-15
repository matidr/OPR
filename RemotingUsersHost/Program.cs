using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace RemotingUsersHost
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Collections.IDictionary dict = new System.Collections.Hashtable();
            dict["name"] = "UserServerTcp";
            dict["port"] = 5000;
            dict["authenticationMode"] = "IdentifyCallers";

            // Set up the server channel.
            var serverChannel = new TcpChannel(dict, null, null);
            try
            {
                // Specify the properties for the server channel.
                ChannelServices.RegisterChannel(serverChannel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(Sockets.UserOperations),
                    "UserOperations",
                    WellKnownObjectMode.Singleton);
                Console.WriteLine("Remoting server started...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                //log exception
            }
            finally
            {
                ChannelServices.UnregisterChannel(serverChannel);
            }

        }
    }
}
