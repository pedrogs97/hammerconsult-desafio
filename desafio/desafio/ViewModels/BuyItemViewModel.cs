using desafio.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class BuyItemViewModel :BaseViewModel
    {
        private Page Page { get; }
        private bool _drink;
        private bool _food;
        private bool _others;
        private Item _item;
        private Barbecue Barbecue;

        public Command CloseModalCommand { get; set; }
        public Command AddCommand { get; set; }
        public bool Drink
        {
            get => _drink;
            set => SetProperty(ref _drink, value);
        }
        public bool Food
        {
            get => _food;
            set => SetProperty(ref _food, value);
        }
        public bool Others
        {
            get => _others;
            set => SetProperty(ref _others, value);
        }
        public Item Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }
        public BuyItemViewModel(Page page)
        {
            Page = page;
            Item = new Item();
            CloseModalCommand = new Command(CloseModal);
            AddCommand = new Command(Add);
        }
        private void Add()
        {
            if (Drink)
            {
                Item.Type = "drink";
                Barbecue.TotalDrink += Item.Value;
                Barbecue.TotalSpent += Item.Value;
            }
            else if (Food)
            {
                Item.Type = "food";
                Barbecue.TotalFood += Item.Value;
                Barbecue.TotalSpent += Item.Value;
            }
            else
            { 
                Item.Type = "Others";
                Barbecue.TotalOthers += Item.Value;
                Barbecue.TotalSpent += Item.Value;
            }
            Barbecue.Items.Add(Item);
            ServiceBarbecue.UpdateItem(Barbecue);
            CloseModal();
        }
        private async void CloseModal()
        {
            await Page.Navigation.PopModalAsync();
        }
        public void OnAppearing(string id)
        {
            Barbecue = ServiceBarbecue.GetItem(id);
        }
    }
}
