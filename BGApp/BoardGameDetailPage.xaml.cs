using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BGApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BoardGameDetailPage : ContentPage
    {
        public BoardGame boardGame;
        public BoardGameDetailPage(BoardGame boardGame)
        {
            this.boardGame = boardGame ?? throw new ArgumentNullException();

            BindingContext = boardGame;

            InitializeComponent();
           
        }
    }
}