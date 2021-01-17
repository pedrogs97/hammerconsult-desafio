using desafio.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace desafio.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuyItem : ContentPage
    {
        private readonly string ID;
        private readonly BuyItemViewModel _viewModel;
        public BuyItem(string id)
        {
            InitializeComponent();
            ID = id;
            BindingContext = _viewModel = new BuyItemViewModel(this);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing(ID);
        }
    }
}