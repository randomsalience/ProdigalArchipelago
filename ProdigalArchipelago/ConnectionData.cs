using System;

namespace ProdigalArchipelago;

[Serializable]
public class ConnectionData(string hostName, int port, string slotName, string password)
{
    public string HostName = hostName;
    public int Port = port;
    public string SlotName = slotName;
    public string Password = password;
}