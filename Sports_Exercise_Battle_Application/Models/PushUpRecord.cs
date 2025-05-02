using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sports_Exercise_Battle_Application.Models
{
    public class PushUpRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TournamentNumber { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int Duration { get; set; }
        
        public PushUpRecord() { }
        public PushUpRecord(int userId, int tournamentNumber, string name, int count, int duration)
        {
            UserId = userId;
            TournamentNumber = tournamentNumber;
            Name = name;
            Count = count;
            Duration = duration;
        }
    }
}
