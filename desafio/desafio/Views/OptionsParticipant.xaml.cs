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
    public partial class OptionsParticipant : ContentPage
    {
        private readonly string IdPerson, IdBarbecue;
        private readonly OptionsParticipantViewModel _viewModel;
        public OptionsParticipant(string idPerson, string idBarbecue)
        {
            InitializeComponent();
            IdPerson = idPerson;
            IdBarbecue = idBarbecue;
            BindingContext = _viewModel  = new OptionsParticipantViewModel(this);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing(IdPerson, IdBarbecue);
        }
    }
}