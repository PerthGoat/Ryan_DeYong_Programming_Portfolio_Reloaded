using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ryan_DeYong_Programming_Portfolio_Reloaded
{
    class Program
    {

        static void Main(string[] args)
        {
            new NetworkManager(8080);
        }
    }
}
