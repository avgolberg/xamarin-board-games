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
    public partial class MechanicsPage : ContentPage
    {
        private HttpClient _client = new HttpClient();
        private List<SelectableData<Mechanic>> mechanicsList = new List<SelectableData<Mechanic>>();
        private List<Mechanic> selectedMechanics = new List<Mechanic>();
        List<string> selectedMechanicsNames;
        private CatalogPage page;
        public MechanicsPage(CatalogPage page, List<string> selectedMechanicsNames)
        {
            this.page = page;
            this.selectedMechanicsNames = selectedMechanicsNames;
            LoadData();
            InitializeComponent();
        }
        private async void LoadData()
        {
            string Url = "https://api.boardgameatlas.com/api/game/mechanics?pretty=true&client_id=5cTX7InZUl";
            var content = await _client.GetStringAsync(Url);
            var mechanics = JsonConvert.DeserializeObject<Mechanics>(content);
            if (selectedMechanicsNames.Count != 0)
            {
                selectedMechanics = mechanics.mechanics.Join(selectedMechanicsNames, c => c.name, sc => sc, (c, sc) => c).ToList();
            }
            foreach (Mechanic mechanic in mechanics.mechanics)
            {
                if (selectedMechanics.Contains(mechanic))
                    mechanicsList.Add(new SelectableData<Mechanic> { Data = mechanic, Selected = true });
                else
                    mechanicsList.Add(new SelectableData<Mechanic> { Data = mechanic, Selected = false });
            }
            mechanicsListView.ItemsSource = mechanicsList;
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.NewTextValue)) mechanicsListView.ItemsSource = mechanicsList;
            else
            {
                mechanicsListView.ItemsSource = mechanicsList.FindAll(c => c.Data.name.Contains(e.NewTextValue) || c.Data.name.ToLower().Contains(e.NewTextValue) || c.Data.name.ToLower().StartsWith(e.NewTextValue) || c.Data.name.StartsWith(e.NewTextValue));
            }

        }

        private void select_Clicked(object sender, EventArgs e)
        {
            selectedMechanics.Clear();
            var isSelected = mechanicsList.FindAll(c => c.Selected == true);
            if (isSelected != null)
            {
                foreach (var category in isSelected)
                {
                    selectedMechanics.Add(category.Data);
                }

                int count = selectedMechanics.Count - 1;
                StringBuilder sb = new StringBuilder();
                if (selectedMechanics.Count > 1)
                {
                    for (int i = 0; i < count; i++)
                    {
                        sb.Append(selectedMechanics[i].name + " · ");
                    }
                    sb.Append(selectedMechanics[count].name);
                    page.SetMechanics(sb.ToString());
                }
                else if (selectedMechanics.Count == 1)
                {
                    page.SetMechanics(selectedMechanics[count].name);
                }
                else
                {
                    page.SetMechanics("None");
                }
            }
            Navigation.PopAsync();
        }

        private void mechanicsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            mechanicsListView.SelectedItem = null;
        }
    }
}