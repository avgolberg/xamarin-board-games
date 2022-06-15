using System;
using System.Collections.Generic;
using System.Text;

namespace BGApp.Models
{
    public class Publishers
    {
        public string id { get; set; }
        public string num_games { get; set; }
        public string score { get; set; }
        public Game game { get; set; }
        public string url { get; set; }
        public PeopleImage images { get; set; }
    }
}
