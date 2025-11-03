using System;
using System.Net.Sockets;
using System.Text;

namespace ItemSorterRobot;

public class ItemSorterRobot
{
    private string IpAddress { get; }
    private int Port { get; }
    private string UrScriptTemplate { get; }

    public ItemSorterRobot(string ipAddress, int port, string urScriptTemplate)
    {
        IpAddress = ipAddress;
        Port = port;
        UrScriptTemplate = urScriptTemplate;
    }

    private void SendUrscript(string script)
    {
        using var client = new TcpClient(IpAddress, Port);
        using var stream = client.GetStream();
        var data = Encoding.ASCII.GetBytes(script);
        stream.Write(data, 0, data.Length);
    }

    public void PickUp(uint itemId)
    {
        var script = UrScriptTemplate.Replace("{id}", itemId.ToString());
        Console.WriteLine($"Robot henter item {itemId}");
        SendUrscript(script);
    }
}