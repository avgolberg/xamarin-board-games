using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BGApp.Models
{
    public class BGSQLite
    {
        [PrimaryKey]
        public string id { get; set; }
    }
}
