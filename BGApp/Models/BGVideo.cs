using System;
using System.Collections.Generic;
using System.Text;

namespace BGApp.Models
{
    public class BGVideo
    {
        public string id { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string channel_name { get; set; }
        public string image_url { get; set; }
        public Game game { get; set; }
    }
}
