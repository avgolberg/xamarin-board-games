using BGApp.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using Xamarin.Forms.Xaml;

namespace BGApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BoardGameDetailPage : ContentPage
    {
        public BoardGame boardGame;
        private HttpClient _client = new HttpClient();
        private SQLiteAsyncConnection _connection;
        private bool isOnWishList = false;

        public BoardGameDetailPage(BoardGame boardGame)
        {
            this.boardGame = boardGame ?? throw new ArgumentNullException();
            BindingContext = boardGame;
            LoadData();
            InitializeComponent();
        }
        private void LoadData()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            IsOnWishList();
            LoadCategories();
            LoadMechanics();
            //LoadDesigners();
            LoadImages();
            LoadVideos();
        }
        private async void IsOnWishList()
        {
            try
            {
                await _connection.CreateTableAsync<BGSQLite>();
                var wishListBoardGamesIds = await _connection.Table<BGSQLite>().ToListAsync();
                if (wishListBoardGamesIds.Where(bg => boardGame.id.Equals(bg.id)).FirstOrDefault() != null)
                {
                    isOnWishList = true;
                    wishListStar.Source = "star.png";
                }
                else
                    isOnWishList = false;

                if (isOnWishList)
                {
                    wishListStar.Source = "star.png";
                }
                else
                {
                    wishListStar.Source = "starOutline.png";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async void LoadCategories()
        {
            try
            {
                string Url = "https://api.boardgameatlas.com/api/game/categories?pretty=true&client_id=5cTX7InZUl";
                var content = await _client.GetStringAsync(Url);
                var categories = JsonConvert.DeserializeObject<Categories>(content);


                if (boardGame.categories.Count != 0)
                {
                    foreach (Category category in boardGame.categories)
                    {
                        category.name = categories.categories.Where(c => c.id == category.id).First().name;
                    }

                    int count = boardGame.categories.Count - 1;
                    StringBuilder sb = new StringBuilder();
                    if (boardGame.mechanics.Count > 1)
                    {

                        for (int i = 0; i < count; i++)
                        {
                            sb.Append(boardGame.categories[i].name + " · ");
                        }
                        sb.Append(boardGame.categories[count].name);
                        categoriesLabel.Text = "Categories: " + sb.ToString();
                    }
                    else if (boardGame.mechanics.Count == 1)
                    {
                        categoriesLabel.Text = "Categories: " + boardGame.categories[count].name;
                    }
                }
                else
                {
                    categoriesLabel.Text = "Categories: ";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void LoadMechanics()
        {
            try
            {
                string Url = "https://api.boardgameatlas.com/api/game/mechanics?pretty=true&client_id=5cTX7InZUl";
                var content = await _client.GetStringAsync(Url);
                var mechanics = JsonConvert.DeserializeObject<Mechanics>(content);

                if (boardGame.mechanics.Count != 0)
                {
                    foreach (Mechanic mechanic in boardGame.mechanics)
                    {
                        mechanic.name = mechanics.mechanics.Where(m => m.id == mechanic.id).First().name;
                    }

                    int count = boardGame.mechanics.Count - 1;
                    StringBuilder sb = new StringBuilder();
                    if (boardGame.mechanics.Count > 1)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            sb.Append(boardGame.mechanics[i].name + " · ");
                        }
                        sb.Append(boardGame.mechanics[count].name);
                        mechanicsLabel.Text = "Mechanics: " + sb.ToString();
                    }
                    else if (boardGame.mechanics.Count == 1)
                    {
                        mechanicsLabel.Text = "Mechanics: " + boardGame.mechanics[count].name;
                    }
                }
                else
                {
                    mechanicsLabel.Text = "Mechanics: ";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void LoadImages()
        {
            try
            {
                string Url = "https://api.boardgameatlas.com/api/game/images?pretty=true&client_id=5cTX7InZUl&game_id=";
                var content = await _client.GetStringAsync(Url + boardGame.id);
                var images = JsonConvert.DeserializeObject<BGImages>(content);
                imagesListView.ItemsSource = images.images;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void LoadVideos()
        {
            try
            {
                string Url = "https://api.boardgameatlas.com/api/game/videos?pretty=true&client_id=5cTX7InZUl&game_id=";
                var content = await _client.GetStringAsync(Url + boardGame.id);
                var videos = JsonConvert.DeserializeObject<BGVideos>(content);
                videosListView.ItemsSource = videos.videos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void videosListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var video = e.SelectedItem as BGVideo;
            await Launcher.OpenAsync(new Uri(video.url));
            videosListView.SelectedItem = null;
        }

        async void OnImageNameTapped(object sender, EventArgs args)
        {
            try
            {
                if (isOnWishList)
                {
                    var wishListBoardGamesIds = await _connection.Table<BGSQLite>().ToListAsync();
                    var gameToDel = wishListBoardGamesIds.Where(bg => boardGame.id.Equals(bg.id)).FirstOrDefault();
                    if (gameToDel != null)
                        await _connection.DeleteAsync(gameToDel);

                    var game = WishListPage._wishListBoardGames.Where(bg => boardGame.id.Equals(bg.id)).FirstOrDefault();
                    if (game != null)
                        WishListPage._wishListBoardGames.Remove(game);

                    isOnWishList = false;
                    wishListStar.Source = "starOutline.png";
                }
                else
                {
                    BGSQLite gameId = new BGSQLite { id = boardGame.id };
                    await _connection.InsertAsync(gameId);
                    WishListPage._wishListBoardGames.Add(gameId);
                    isOnWishList = true;
                    wishListStar.Source = "star.png";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //private async void LoadDesigners()
        //{
        //    int count = boardGame.designers.Count - 1;
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < count; i++)
        //    {
        //        sb.Append(boardGame.designers[i].id + " · ");
        //    }
        //    sb.Append(boardGame.mechanics[count].name);
        //    mechanicsLabel.Text = "designers: " + sb.ToString();

        //}
    }
}