using BGApp.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BGApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CollectionPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        public static ObservableCollection<BGCollectionSQLite> _collectionBoardGames;
        private HttpClient _client = new HttpClient();
        public CollectionPage()
        {
            LoadData();
            InitializeComponent();
        }
        private async void LoadData()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            await _connection.CreateTableAsync<BGCollectionSQLite>();

            var collectionBoardGamesIds = await _connection.Table<BGCollectionSQLite>().ToListAsync();
            _collectionBoardGames = new ObservableCollection<BGCollectionSQLite>(collectionBoardGamesIds);
            _collectionBoardGames.CollectionChanged += _collectionBoardGames_CollectionChanged;

            if (collectionBoardGamesIds.Count != 0)
            {
                GetGames(GetWishListGamesIds());
            }

        }
        private void _collectionBoardGames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            GetGames(GetWishListGamesIds());
        }
        private async void GetGames(string ids)
        {
            string Url = "https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl&ids=" + ids;
            var content = await _client.GetStringAsync(Url);
            var boardGames = JsonConvert.DeserializeObject<Games>(content);
            boardGamesListView.ItemsSource = boardGames.games;
        }

        private string GetWishListGamesIds()
        {
            StringBuilder sb = new StringBuilder();
            foreach (BGCollectionSQLite boardGameId in _collectionBoardGames)
            {
                sb.Append(boardGameId.id + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }


        private async void addGame_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddGamePage());
        }

        private async void boardGamesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var boardGame = e.SelectedItem as BoardGame;
            await Navigation.PushAsync(new BoardGameDetailPage(boardGame));
            boardGamesListView.SelectedItem = null;
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            var boardGame = (sender as MenuItem).CommandParameter as BoardGame;
            var collectionBoardGamesIds = await _connection.Table<BGCollectionSQLite>().ToListAsync();
            var gameToDel = collectionBoardGamesIds.Find(bg => boardGame.id.Equals(bg.id));
            if (gameToDel != null)
                await _connection.DeleteAsync(gameToDel);

            var game = _collectionBoardGames.Where(bg => boardGame.id.Equals(bg.id)).FirstOrDefault();
            if (game != null)
                _collectionBoardGames.Remove(game);
        }
    }
}