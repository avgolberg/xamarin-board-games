using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using BGApp.iOS;

[assembly: Dependency(typeof(SQLiteDb))]

namespace BGApp.iOS
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); 
            var path = Path.Combine(documentsPath, "BGSQLite.db3");

            return new SQLiteAsyncConnection(path);
        }
    }
}

