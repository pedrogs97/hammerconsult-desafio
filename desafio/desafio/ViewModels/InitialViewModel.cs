using desafio.Models;
using desafio.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class InitialViewModel : BaseViewModel
    {
        private bool _isVisible;
        private ObservableCollection<Barbecue> _barbecues;
        public ObservableCollection<Barbecue> Barbecues
        {
            get => _barbecues;
            set => SetProperty(ref _barbecues, value);
        }
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }
        private INavigation Navigation { get; }
        public Command ToAddCommand { get; }
        public Command ToDetailCommand { get; }
        public Command ToJoinCommand { get; }
        public InitialViewModel(INavigation navigation)
        {
            Title = "Home";
            Navigation = navigation;
            ToJoinCommand = new Command(ToJoin);
            ToAddCommand = new Command(ToAdd);
            ToDetailCommand = new Command<string>(ToDetail);
            Barbecues = new ObservableCollection<Barbecue>();
        }
        private async void ToJoin()
        {
            await Navigation.PushModalAsync(new JoinBarbecuePage());
        }
        private async void ToAdd()
        {
            await Navigation.PushAsync(new AddBarbecuePage());
        }
        private async void ToDetail(string idBarbecue)
        {
            await Navigation.PushAsync(new BarbecueDetail(idBarbecue), true);
        }
        public async void OnAppearing()
        {
            Barbecues = LoadBarbecue();
            if (!App.Current.Properties.ContainsKey("user"))
            {
                var currentUser = new Person { Id = Guid.NewGuid().ToString(), Name = "Pedro Gustavo" };
                if (ServicePerson.AddItem(currentUser))
                {
                    App.Current.Properties.Add("user", currentUser.Id);
                    await App.Current.SavePropertiesAsync();
                }
                else
                    throw new NotImplementedException();
            }
        }
        private ObservableCollection<Barbecue> LoadBarbecue()
        {
            if (App.Current.Properties.ContainsKey("user"))
            {
                var user = ServicePerson.GetItem(App.Current.Properties["user"].ToString());
                var barbecues = new ObservableCollection<Barbecue>(ServiceBarbecue.GetItems().Where((barbecue) => barbecue.Participants.Contains(user)));
                if (barbecues.Count > 0)
                    IsVisible = true;
                else
                    IsVisible = false;
                return barbecues;
            }
            else
            {
                IsVisible = false;
                return new ObservableCollection<Barbecue>();
            }
        }
    }
}
