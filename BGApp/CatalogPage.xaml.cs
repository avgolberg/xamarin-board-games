using BGApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using Xamarin.Forms.Xaml;

namespace BGApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CatalogPage : ContentPage
    {
        private int gamesLoaded = 30;
        private HttpClient _client = new HttpClient();
        private InfiniteScrollCollection<BoardGame> _boardGames;
        public CatalogPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void SearchData(string searchText = null)
        {
            try
            {
                if (String.IsNullOrEmpty(searchText)) boardGamesListView.ItemsSource = _boardGames;
            else
            {
                string Url = "https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl&exact=true&name=" + searchText;
                var content = await _client.GetStringAsync(Url);
                var boardGames = JsonConvert.DeserializeObject<Games>(content);
                boardGamesListView.ItemsSource = boardGames.games;
            }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void LoadData()
        {
            try
            {
                string Url = "https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl&limit=30";
            var content = await _client.GetStringAsync(Url);
            var boardGames = JsonConvert.DeserializeObject<Games>(content);

            _boardGames = new InfiniteScrollCollection<BoardGame>(boardGames.games)
            {
                OnLoadMore = async () =>
                {
                    gamesLoaded += 30;
                    Url = "https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl&limit=30&skip=" + gamesLoaded;
                    content = await _client.GetStringAsync(Url);
                    boardGames = JsonConvert.DeserializeObject<Games>(content);
                    return boardGames.games;
                }
            };
            boardGamesListView.ItemsSource = _boardGames;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void boardGamesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var boardGame = e.SelectedItem as BoardGame;
            await Navigation.PushAsync(new BoardGameDetailPage(boardGame));
            boardGamesListView.SelectedItem = null;
        }

        private void search_SearchButtonPressed(object sender, System.EventArgs e)
        {
            SearchData((sender as SearchBar).Text);
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.NewTextValue)) boardGamesListView.ItemsSource = _boardGames;
        }
    }
}