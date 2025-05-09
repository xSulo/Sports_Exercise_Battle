using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBattleApp.DTOs
{
    public class TokenValidationResultDTO
    {
        public bool IsValid { get; set; }
        public string? TokenHash{ get; set; }
    }
}
