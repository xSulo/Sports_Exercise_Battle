using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBattleApp.DTOs
{
    public class TournamentGetStatusDTO
    {
        public bool IsTournamentRunning { get; set; }
        public int? Participants { get; set; }
        public string? leader { get; set; }
        public string? TournamentStart { get; set; }

    }
}
