using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBattleApp.DTOs
{
    public class HttpResponseDTO
    {
        public int StatusCode { get; set; }
        public string JsonContent { get; set; } = "";

    }
}
