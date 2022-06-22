using BGApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BGApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RandomPage : ContentPage
    {
        private HttpClient _client = new HttpClient();
        public RandomPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                string Url = "https://api.boardgameatlas.com/api/search?random=true&client_id=5cTX7InZUl";
                var content = await _client.GetStringAsync(Url);
                var boardGame = JsonConvert.DeserializeObject<Games>(content);

                await Navigation.PushAsync(new BoardGameDetailPage(boardGame.games.FirstOrDefault()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void random_Clicked(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}