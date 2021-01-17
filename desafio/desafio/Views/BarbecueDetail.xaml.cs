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
    public partial class BarbecueDetail : ContentPage
    {
        private readonly BarbecueDetailViewModel _viewModel;
        private readonly string id;
        public BarbecueDetail(string idBarbecue)
        {
            InitializeComponent();
            BindingContext = _viewModel = new BarbecueDetailViewModel(this);
            id = idBarbecue;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing(id);
        }
    }
}