using desafio.Models;
using desafio.Views;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class FinancesViewModel : BaseViewModel
    {
        private ObservableCollection<Item> _items;
        private float _totalOthers;
        private float _totalDrink;
        private float _totalFood;
        private float _total;
        private Barbecue barbecue;
        private readonly Page Page;
        private bool _isVisible;

        public Command CloseModalCommand { get; set; }
        public Command BuyCommand { get; set; }
        public Command RemoveBuyCommand { get; set; }
        public ObservableCollection<Item> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
        public float TotalOthers
        {
            get => _totalOthers;
            set => SetProperty(ref _totalOthers, value);
        }
        public float TotalDrink
        {
            get => _totalDrink;
            set => SetProperty(ref _totalDrink, value);
        }
        public float TotalFood
        {
            get => _totalFood;
            set => SetProperty(ref _totalFood, value);
        }
        public float Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
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
            RemoveBuyCommand = new Command<Item>(RemoveBuy);
            IsVisible = false;
            barbecue = new Barbecue();
            Items = new ObservableCollection<Item>();
        }
        private async void CloseModal()
        {
            await Page.Navigation.PopModalAsync();
        }
        private async void Buy()
        {
            await Page.Navigation.PushModalAsync(new BuyItem(barbecue.Id));
        }
        private async void RemoveBuy(Item item)
        {
            if (await Page.DisplayAlert("","Deseja remover o item?", "Sim", "Não"))
            {
                barbecue.Items.Remove(item);
                var tDrink = 0.00f;
                var tFood = 0.00f;
                var tOthers = 0.00f;
                barbecue.Items.ForEach((i) =>
                {
                    if (i.Type == "drink")
                        tDrink += i.Value;
                    else if (i.Type == "food")
                        tFood += i.Value;
                    else
                        tOthers += i.Value;
                });
                TotalDrink = barbecue.TotalDrink = tDrink;
                TotalFood = barbecue.TotalFood = tFood;
                TotalOthers = barbecue.TotalOthers = tOthers;
                Total = barbecue.TotalSpent = tDrink + tFood + tOthers;
                ServiceBarbecue.UpdateItem(barbecue);
                UpdateItems();
            }
        }
        private void UpdateItems()
        {
            Items.Clear();
            Items = new ObservableCollection<Item>(barbecue.Items);
        }
        public void OnAppearing(string id)
        {
            barbecue = ServiceBarbecue.GetItem(id);
            TotalDrink = barbecue.TotalDrink;
            TotalFood = barbecue.TotalFood;
            TotalOthers = barbecue.TotalOthers;
            Total = barbecue.TotalSpent;
            Items = new ObservableCollection<Item>(barbecue.Items);
            if (ServicePerson.GetItem(App.Current.Properties["user"].ToString()).Id == barbecue.Creator.Id)
            {
                IsVisible = true;
            }
        }
    }
}
