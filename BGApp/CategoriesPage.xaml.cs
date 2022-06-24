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
    
    public partial class CategoriesPage : ContentPage
    {
        private HttpClient _client = new HttpClient();
        private List<SelectableData<Category>> categoriesList = new List<SelectableData<Category>>();
        private List<Category> selectedCategories = new List<Category>();
        List<string> selectedCategoriesNames;
        private CatalogPage page;

        public CategoriesPage(CatalogPage page, List<string> selectedCategoriesNames)
        {
            this.page = page;
            this.selectedCategoriesNames = selectedCategoriesNames;
            LoadData();
            InitializeComponent();
        }

        private async void LoadData()
        {
            string Url = "https://api.boardgameatlas.com/api/game/categories?pretty=true&client_id=5cTX7InZUl";
            var content = await _client.GetStringAsync(Url);
            var categories = JsonConvert.DeserializeObject<Categories>(content);
            if (selectedCategoriesNames.Count!=0)
            {
                selectedCategories = categories.categories.Join(selectedCategoriesNames, c => c.name, sc => sc, (c, sc) => c).ToList();
            }
            foreach (Category category in categories.categories)
            {
                if(selectedCategories.Contains(category))
                    categoriesList.Add(new SelectableData<Category> { Data = category, Selected = true });
                else
                    categoriesList.Add(new SelectableData<Category> { Data = category, Selected = false });
            }
            categoriesListView.ItemsSource = categoriesList;
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.NewTextValue)) categoriesListView.ItemsSource = categoriesList;
            else
            {
                categoriesListView.ItemsSource = categoriesList.FindAll(c => c.Data.name.Contains(e.NewTextValue) || c.Data.name.ToLower().Contains(e.NewTextValue) || c.Data.name.ToLower().StartsWith(e.NewTextValue) || c.Data.name.StartsWith(e.NewTextValue));
            }

        }

        private void select_Clicked(object sender, EventArgs e)
        {
            selectedCategories.Clear();
            var isSelected = categoriesList.FindAll(c => c.Selected == true);
            if (isSelected != null)
            {
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
                    page.SetCategories(sb.ToString());
                }
                else if (selectedCategories.Count == 1)
                {
                    page.SetCategories(selectedCategories[count].name);
                }
                else
                {
                    page.SetCategories("None");
                }
            }
            Navigation.PopAsync();
        }

        private void categoriesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            categoriesListView.SelectedItem = null;
        }
    }
}