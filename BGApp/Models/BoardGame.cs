using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BGApp.Models
{
    public class BoardGame
    {
        public string id { get; set; }
        public string name { get; set; }
        public int? year_published { get; set; }
        public int? min_players { get; set; }
        public int? max_players { get; set; }
        public int? min_playtime { get; set; }
        public int? max_playtime { get; set; }
        public int? min_age { get; set; }
        public string description_preview { get; set; }
        public string image_url { get; set; }
        public double? price { get; set; }
        public double? msrp { get; set; }
        public PrimaryPublisher primary_publisher { get; set; }
        public PrimaryDesigner primary_designer { get; set; }
        public IList<Mechanic> mechanics { get; set; }
        public IList<Category> categories { get; set; }

        //public string[] names { get; set; }
        //public string description { get; set; }
        //public string thumb_url { get; set; }
        //public string url { get; set; }
        //public IList<Publishers> publishers { get; set; }
        //public double? discount { get; set; }
        //public IList<Designers> designers { get; set; }
        //public IList<string> developers { get; set; }
        //public IList<string> artists { get; set; }
        //public int? reddit_all_time_count { get; set; }
        //public int? reddit_week_count { get; set; }
        //public int? reddit_day_count { get; set; }
    }
}
