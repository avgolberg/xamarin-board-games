using BGApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public BoardGameDetailPage(BoardGame boardGame)
        {
            this.boardGame = boardGame ?? throw new ArgumentNullException();

            BindingContext = boardGame;          

            InitializeComponent();

            LoadData();
        }
        private void LoadData()
        {
            LoadCategories();
            LoadMechanics();
            //LoadDesigners();
            LoadImages();
            LoadVideos();
        }

        private async void LoadCategories()
        {
            string Url = "https://api.boardgameatlas.com/api/game/categories?pretty=true&client_id=5cTX7InZUl";
            var content = await _client.GetStringAsync(Url);
            var categories = JsonConvert.DeserializeObject<Categories>(content);
            
            foreach (Category category in boardGame.categories)
            {
                category.name = categories.categories.Where(c => c.id==category.id).First().name;
            }

            int count = boardGame.categories.Count - 1;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(boardGame.categories[i].name + " · ");
            }
            sb.Append(boardGame.categories[count].name);
            categoriesLabel.Text = "Categories: " + sb.ToString();

        }

        private async void LoadMechanics()
        {
            string Url = "https://api.boardgameatlas.com/api/game/mechanics?pretty=true&client_id=5cTX7InZUl";
            var content = await _client.GetStringAsync(Url);
            var mechanics = JsonConvert.DeserializeObject<Mechanics>(content);

            foreach (Mechanic mechanic in boardGame.mechanics)
            {
                mechanic.name = mechanics.mechanics.Where(m => m.id == mechanic.id).First().name;
            }

            int count = boardGame.mechanics.Count - 1;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(boardGame.mechanics[i].name + " · ");
            }
            sb.Append(boardGame.mechanics[count].name);
            mechanicsLabel.Text = "Mechanics: " + sb.ToString();

        }

        private async void LoadImages()
        {
            string Url = "https://api.boardgameatlas.com/api/game/images?pretty=true&client_id=5cTX7InZUl&game_id=";
            var content = await _client.GetStringAsync(Url + boardGame.id);
            var images = JsonConvert.DeserializeObject<BGImages>(content);
            imagesListView.ItemsSource = images.images;
        }

        private async void LoadVideos()
        {
            string Url = "https://api.boardgameatlas.com/api/game/videos?pretty=true&client_id=5cTX7InZUl&game_id=";
            var content = await _client.GetStringAsync(Url + boardGame.id);
            var videos = JsonConvert.DeserializeObject<BGVideos>(content);
            videosListView.ItemsSource = videos.videos;
        }

        private async void videosListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var video = e.SelectedItem as BGVideo;
            await Launcher.OpenAsync(new Uri(video.url));
            videosListView.SelectedItem = null;
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