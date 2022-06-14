using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BGApp
{
    public class Games
    {
        public List<BoardGame> games { get; set; }
        public int count { get; set; }
    }
    public class BoardGame
    {
        public string id { get; set; }
        public string name { get; set; }
        public string[] names { get; set; }
        public int year_published { get; set; }
        public int min_players { get; set; }
        public int max_players { get; set; }
        public int min_playtime { get; set; }
        public int max_playtime { get; set; }
        public int min_age { get; set; }
        public string description { get; set; }
        public string description_preview { get; set; }
        public string thumb_url { get; set; }
        public string image_url { get; set; }
        public string url { get; set; }
        public double price { get; set; }
        public double msrp { get; set; }
        public double discount { get; set; }
        public PrimaryPublisher primary_publisher { get; set; }
        public IList<Publishers> publishers { get; set; }
        public Mechanic[] mechanics { get; set; }
        public Category[] categories { get; set; }
        public IList<Designers> designers { get; set; }
        public IList<string> developers { get; set; }
        public IList<string> artists { get; set; }
        public int reddit_all_time_count { get; set; }
        public int reddit_week_count { get; set; }
        public int reddit_day_count { get; set; }
    }

    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Mechanic
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Publishers
    {
        public string id { get; set; }
        public string num_games { get; set; }
        public string score { get; set; }
        public Game game { get; set; }
        public string url { get; set; }
        public BGImage images { get; set; }
    }

    public class Designers
    {
        public string id { get; set; }
        public string num_games { get; set; }
        public string score { get; set; }
        public Game game { get; set; }
        public string url { get; set; }
        public BGImage images { get; set; }
    }


    public class PrimaryDesigner
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class PrimaryPublisher
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Game
    {

    }

    public class BGImage
    {
        public string thumb { get; set; }
        public string small { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
        public string original { get; set; }
    }


    public partial class MainPage : ContentPage
    {
        const string Url = "https://api.boardgameatlas.com/api/search?client_id=5cTX7InZUl";
        private HttpClient _client = new HttpClient();
        private ObservableCollection<BoardGame> _boardGames;
        public MainPage()
        {
            InitializeComponent();
            //BGTitle.Text = Resource.BGTitle;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var content = await _client.GetStringAsync(Url);
            var boardGames = JsonConvert.DeserializeObject<Games>(content);

            _boardGames = new ObservableCollection<BoardGame>(boardGames.games);

            boardGamesListView.ItemsSource = _boardGames;

        }
    }
}
