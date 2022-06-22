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
    public class SelectableData<T>
    {
        public T Data { get; set; }
        public bool Selected { get; set; }
    }
    public partial class CategoriesPage : ContentPage
    {
        private HttpClient _client = new HttpClient();
        private List<SelectableData<Category>> categoriesList { get; set; }
        private List<Category> selectedCategories { get; set; }

        public Button selectButton { get { return selectButton; } }

        public CategoriesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            string Url = "https://api.boardgameatlas.com/api/game/categories?pretty=true&client_id=5cTX7InZUl";
            var content = await _client.GetStringAsync(Url);
            var categories = JsonConvert.DeserializeObject<Categories>(content);
            foreach (Category category in categories.categories)
            {
                categoriesList.Add(new SelectableData<Category> { Data = category, Selected = false });
            }
            categoriesListView.ItemsSource = categoriesList;
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.NewTextValue)) categoriesListView.ItemsSource = categoriesList;
            else
            {
                categoriesListView.ItemsSource = categoriesList.FindAll(c => c.Data.name.StartsWith(e.NewTextValue));
            }

        }

        private void select_Clicked(object sender, EventArgs e)
        {
            var isSelected = categoriesList.FindAll(c => c.Selected == true);
            foreach (var category in isSelected)
            {
                selectedCategories.Add(category.Data);
            }

            int count = selectedCategories.Count - 1;
            StringBuilder sb = new StringBuilder();
            if (selectedCategories.Count > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    sb.Append(selectedCategories[i].name + " · ");
                }
                sb.Append(selectedCategories[count].name);
                CatalogPage.selectedCategories = "Categories: " + sb.ToString();
            }
            else if (selectedCategories.Count == 1)
            {
                CatalogPage.selectedCategories = "Categories: " + selectedCategories[count].name;
            }

            Navigation.PopAsync();
        }
    }
}