using SQLite;

namespace BGApp
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}

