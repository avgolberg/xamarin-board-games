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
    public partial class WishListPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        public static ObservableCollection<BGSQLite> _wishListBoardGames;
        private HttpClient _client = new HttpClient();
        public WishListPage()
        {
            LoadData();
            InitializeComponent();
        }

        private async void LoadData()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            await _connection.CreateTableAsync<BGSQLite>();

            var wishListBoardGamesIds = await _connection.Table<BGSQLite>().ToListAsync();
            _wishListBoardGames = new ObservableCollection<BGSQLite>(wishListBoardGamesIds);
            _wishListBoardGames.CollectionChanged += _wishListBoardGames_CollectionChanged;

            if (wishListBoardGamesIds.Count != 0)
            {
                GetGames(GetWishListGamesIds());
            }

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
            foreach (BGSQLite boardGameId in _wishListBoardGames)
            {
                sb.Append(boardGameId.id + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private void _wishListBoardGames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            GetGames(GetWishListGamesIds());
        }

        private async void boardGamesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var boardGame = e.SelectedItem as BoardGame;
            await Navigation.PushAsync(new BoardGameDetailPage(boardGame));
            boardGamesListView.SelectedItem = null;
        }
    }
}