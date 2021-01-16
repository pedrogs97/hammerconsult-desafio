using desafio.Models;
using desafio.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class FinancesViewModel : BaseViewModel
    {
        private Barbecue _barbecue;
        private readonly Page Page;
        private bool _isVisible;

        public Command CloseModalCommand { get; set; }
        public Command BuyCommand { get; set; }
        public Barbecue Barbecue
        {
            get => _barbecue;
            set => SetProperty(ref _barbecue, value);
        }
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }
        public FinancesViewModel(Page page)
        {
            Page = page;
            CloseModalCommand = new Command(CloseModal);
            BuyCommand = new Command(Buy);
            IsVisible = false;
        }
        
        private async void CloseModal()
        {
            await Page.Navigation.PopModalAsync();
        }
        private async void Buy()
        {
            await Page.Navigation.PushModalAsync(new BuyItem());
        }

        public void OnAppearing(string id)
        {
            Barbecue = ServiceBarbecue.GetItem(id);
            if (ServicePerson.GetItem(App.Current.Properties["user"].ToString()).Id == Barbecue.Creator.Id)
            {
                IsVisible = true;
            }
            
        }
    }
}
