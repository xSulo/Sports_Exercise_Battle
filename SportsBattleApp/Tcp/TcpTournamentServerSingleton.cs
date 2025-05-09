using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBattleApp.Tcp
{
    public class TcpTournamentServerSingleton
    {
        private static readonly Lazy<TcpTournamentServer> _instance =
            new(() => new TcpTournamentServer(10002));

        public static TcpTournamentServer Instance => _instance.Value;
    }
}
