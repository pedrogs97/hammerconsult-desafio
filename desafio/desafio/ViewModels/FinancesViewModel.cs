using desafio.Models;
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

        public Command CloseModalCommand { get; set; }

        public FinancesViewModel(Page page)
        {
            Page = page;
            CloseModalCommand = new Command(CloseModal);
        }
        
        private async void CloseModal()
        {
            await Page.Navigation.PopModalAsync();
        }
        public Barbecue Barbecue
        {
            get => _barbecue;
            set => SetProperty(ref _barbecue, value);
        }
        public void OnAppearing(string id)
        {
            Barbecue = ServiceBarbecue.GetItem(id);
            if (ServicePerson.GetItem(App.Current.Properties["user"].ToString()).Id == Barbecue.Creator.Id)
            {

            }
            
        }
    }
}
