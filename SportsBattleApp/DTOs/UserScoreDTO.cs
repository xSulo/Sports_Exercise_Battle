using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBattleApp.DTOs
{
    public class UserScoreDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Elo { get; set; }
        public int TotalPushUps { get; set; }

    }
}
