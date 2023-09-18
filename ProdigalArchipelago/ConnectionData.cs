using System;

namespace ProdigalArchipelago
{
    [Serializable]
    public class ConnectionData
    {
        public string HostName;
        public int Port;
        public string SlotName;
        public string Password;

        public ConnectionData(string hostName, int port, string slotName, string password)
        {
            HostName = hostName;
            Port = port;
            SlotName = slotName;
            Password = password;
        }
    }
}