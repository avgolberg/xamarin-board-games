using System;
using System.Collections.Generic;
using System.Text;

namespace BGApp.Models
{
    public class BGImage
    {
        public string id { get; set; }
        public string medium { get; set; }
        public Game game { get; set; }
        public bool isPrimary { get; set; }
    }
}
