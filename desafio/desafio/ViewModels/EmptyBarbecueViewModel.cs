using desafio.Models;
using desafio.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class EmptyBarbecueViewModel : BaseViewModel
    {
        public Command AddCommad { get; }
        private INavigation Navigation { get; }
        public EmptyBarbecueViewModel(INavigation navigation)
        {
            Navigation = navigation;
            AddCommad = new Command(async () => {
                await Navigation.PushAsync(new AddBarbecuePage());
            });
        }

    }
}
