using BGApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

        List<string> selectedCategoriesNames = new List<string>();
        List<string> selectedMechanicsNames = new List<string>();

        public CatalogPage()
        {
            InitializeComponent();
            LoadData();
        }
        public void SetCategories(string categories)
        {
            categoriesName.Text = categories;
        }
        public void SetMechanics(string mechanics)
        {
            mechanicsName.Text = mechanics;
        }
        private async void SearchData(string searchText = null)
        {
            try
            {
                if (String.IsNullOrEmpty(searchText)) boardGamesListView.ItemsSource = _boardGames;
                else
                {
                    IsDataLoading(true);
                    string Url = "https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl&exact=true&name=" + searchText;
                    var content = await _client.GetStringAsync(Url);
                    var boardGames = JsonConvert.DeserializeObject<Games>(content);
                    boardGamesListView.ItemsSource = boardGames.games;
                    IsDataLoading(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void IsDataLoading(bool isLoading)
        {
            if (isLoading)
            {
                dataLoading.IsVisible = true;
                dataLoading.IsRunning = true;
            }
            else
            {
                dataLoading.IsVisible = false;
                dataLoading.IsRunning = false;
            }
               
        }

        private async void LoadData()
        {
            try
            {
                IsDataLoading(true);
                string Url = "https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl";
                var content = await _client.GetStringAsync(Url);
                var boardGames = JsonConvert.DeserializeObject<Games>(content);

                _boardGames = new InfiniteScrollCollection<BoardGame>(boardGames.games)
                {
                    OnLoadMore = async () =>
                    {
                        gamesLoaded += 30;
                        Url = "https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl&skip=" + gamesLoaded;
                        content = await _client.GetStringAsync(Url);
                        boardGames = JsonConvert.DeserializeObject<Games>(content);
                        return boardGames.games;
                    }
                };
                boardGamesListView.ItemsSource = _boardGames;
                IsDataLoading(false);
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

        private void addFilters_Clicked(object sender, EventArgs e)
        {
            if (filters.IsVisible)
            {
                addFilters.Source = "expandArrow.png";
                filters.IsVisible = false;
            }
            else
            {
                addFilters.Source = "collapseArrow.png";
                filters.IsVisible = true;
            }
        }

        private void categories_Tapped(object sender, EventArgs e)
        {
            selectedCategoriesNames.Clear();
            if (!categoriesName.Text.Equals("None"))
            {
                selectedCategoriesNames.AddRange(categoriesName.Text.Split(new string[] { " · " }, StringSplitOptions.None));
            }
            Navigation.PushAsync(new CategoriesPage(this, selectedCategoriesNames));
        }

        private void mechanics_Tapped(object sender, EventArgs e)
        {
            selectedCategoriesNames.Clear();
            if (!mechanicsName.Text.Equals("None"))
            {
                selectedMechanicsNames.AddRange(mechanicsName.Text.Split(new string[] { " · " }, StringSplitOptions.None));
            }
            Navigation.PushAsync(new MechanicsPage(this, selectedMechanicsNames));
        }

        private async void filter_Clicked(object sender, EventArgs e)
        {
            IsDataLoading(true);
            addFilters.Source = "expandArrow.png";
            filters.IsVisible = false;

            boardGamesListView.ItemsSource = null;
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(search.Text))
                sb.Append("https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl&exact=true&name=" + search.Text);
            else
                sb.Append("https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl");

            if (!String.IsNullOrEmpty(min_age.Text))
                sb.Append("&min_age=" + min_age.Text);

            if (!String.IsNullOrEmpty(min_players.Text))
                sb.Append("&min_players=" + min_players.Text);

            if (!String.IsNullOrEmpty(max_players.Text))
                sb.Append("&max_players=" + max_players.Text);

            if (!String.IsNullOrEmpty(min_playtime.Text))
                sb.Append("&min_playtime=" + min_playtime.Text);

            if (!String.IsNullOrEmpty(max_playtime.Text))
                sb.Append("&max_playtime=" + max_playtime.Text);

            if (!categoriesName.Text.Equals("None"))
            {
                selectedCategoriesNames.AddRange(categoriesName.Text.Split(new string[] { " · " }, StringSplitOptions.None));
                sb.Append("&categories=");
                string Url = "https://api.boardgameatlas.com/api/game/categories?pretty=true&client_id=5cTX7InZUl";
                var categoriesContent = await _client.GetStringAsync(Url);
                var categories = JsonConvert.DeserializeObject<Categories>(categoriesContent);
                foreach (string categoryName in selectedCategoriesNames)
                {
                    var category = categories.categories.Find(c => c.name.Equals(categoryName));
                    if (category != null)
                        sb.Append(category.id + ",");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            if (!mechanicsName.Text.Equals("None"))
            {
                selectedMechanicsNames.AddRange(mechanicsName.Text.Split(new string[] { " · " }, StringSplitOptions.None));
                sb.Append("&mechanics=");
                string Url = "https://api.boardgameatlas.com/api/game/mechanics?pretty=true&client_id=5cTX7InZUl";
                var mechanicsContent = await _client.GetStringAsync(Url);
                var mechanics = JsonConvert.DeserializeObject<Mechanics>(mechanicsContent);
                foreach (string mechanicName in selectedMechanicsNames)
                {
                    var mechanic = mechanics.mechanics.Find(m => m.name.Equals(mechanicName));
                    if (mechanic != null)
                        sb.Append(mechanic.id + ",");
                }
                sb.Remove(sb.Length - 1, 1);
            }

            //if (!String.IsNullOrEmpty(publisher.Text))
            //    sb.Append("&publisher=" + publisher.Text);

            //if (!String.IsNullOrEmpty(designer.Text))
            //    sb.Append("&designer=" + designer.Text);

            if (sort.SelectedItem != null)
            {
                switch (sort.SelectedItem.ToString())
                {
                    case "Name":
                        sb.Append("&order_by=name");
                        break;
                    case "Price":
                        sb.Append("&order_by=price");
                        break;
                    case "Year Published":
                        sb.Append("&order_by=year_published");
                        break;
                }
            }


            var content = await _client.GetStringAsync(sb.ToString());
            var boardGames = JsonConvert.DeserializeObject<Games>(content);
            gamesLoaded = 30;
            string _Url = sb.ToString();
            _boardGames = new InfiniteScrollCollection<BoardGame>(boardGames.games)
            {
                OnLoadMore = async () =>
                {
                    gamesLoaded += 30;
                    _Url = sb.ToString() + "&skip=" + gamesLoaded;
                    content = await _client.GetStringAsync(_Url);
                    boardGames = JsonConvert.DeserializeObject<Games>(content);
                    return boardGames.games;
                }
            };
            boardGamesListView.ItemsSource = _boardGames;
            IsDataLoading(false);
        }
    }
}