using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalc.Models
{
    public class Client
    {
        public string ClientId { get; set; }
        public ClientConnectionType ClientConnectionType { get; set; }
    }

    public enum ClientConnectionType
    {
        SignalR,
        GenericWebSocket,
        Http
    }
}
