using BGApp.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BGApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddGamePage : ContentPage
    {
        private HttpClient _client = new HttpClient();
        private SQLiteAsyncConnection _connection;
        public AddGamePage()
        {
            LoadData();
            InitializeComponent();
        }

        private async void LoadData()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            await _connection.CreateTableAsync<BGCollectionSQLite>();
        }

        private async void SearchData(string searchText = null)
        {
            try
            {
                if (String.IsNullOrEmpty(searchText))
                {
                    addNewGame.IsVisible = true;
                    boardGamesListView.ItemsSource = null;
                    return;
                }
                else
                {
                    addNewGame.IsVisible = false;
                    string Url = "https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl&exact=true&name=" + searchText;
                    var content = await _client.GetStringAsync(Url);
                    var boardGames = JsonConvert.DeserializeObject<Games>(content);
                    var collectionBoardGamesIds = await _connection.Table<BGCollectionSQLite>().ToListAsync();
                    var searchedBG = boardGames.games.Join(collectionBoardGamesIds, s => s.id, bg => bg.id, (s, bg) => s).ToList();
                    for (int i = 0; i < searchedBG.Count; i++)
                    {
                        boardGames.games.Remove(searchedBG[i]);
                    }                   
                    if (boardGames.games.Count == 0)
                        addNewGame.IsVisible = true;
                    boardGamesListView.ItemsSource = boardGames.games;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void search_TextChanged(object sender, System.EventArgs e)
        {
            SearchData((sender as SearchBar).Text);
        }

        private async void boardGamesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var boardGame = e.SelectedItem as BoardGame;
            BGCollectionSQLite gameId = new BGCollectionSQLite { id = boardGame.id };
            await _connection.InsertAsync(gameId);
            CollectionPage._collectionBoardGames.Add(gameId);
            await Navigation.PopAsync();
        }

        private async void addNewGame_Clicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(new Uri("https://www.boardgameatlas.com/game/add"));
        }
    }
}