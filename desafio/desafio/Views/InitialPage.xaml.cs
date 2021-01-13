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
    public partial class InitialPage : ContentPage
    {
        private readonly InitialViewModel _viewModel;
        public InitialPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new InitialViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Content = _viewModel.OnAppearing();
        }
    }
}